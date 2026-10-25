using System.Diagnostics;
using SkiaSharp;

namespace dotNetFractal.Logic;

/// <summary>
/// Subdivides the display area into patches and computes these using worker threads.
/// </summary>
public class FractalStitcher : Worker, IDisposable
{
    private readonly FractalSettings m_fractalSettings;
    private readonly List<IFractal> m_fractalsToUpdate = [];
    private readonly List<IFractal> m_fractalsCompleted = [];
    private readonly AutoResetEvent m_bitmapUpdateEvent = new (false);
    private double m_progress = 0.0;

    public event EventHandler ComputationCompleted = delegate { };

    public FractalSettings FractalSettings => m_fractalSettings;

    public bool UpdateColors { get; set; }

    public WaitHandle BitmapUpdateEvent => m_bitmapUpdateEvent;

    public FractalStitcher(FractalSettings fractalSettings)
    {
        Debug.Assert(fractalSettings != null && fractalSettings.FractalArea != null);
        m_fractalSettings = fractalSettings;
    }

    public bool HasFractalsToUpdate
    {
        get
        {
            LockMutex();
            var hasFractals = m_fractalsToUpdate.Count > 0;
            UnlockMutex();
            return hasFractals;
        }
    }

    public double Progress
    {
        get
        {
            LockMutex();
            var progress = m_progress;
            UnlockMutex();
            return progress;
        }
    }

    private List<IFractal> GetPatches(IDisplayArea area)
    {
        var width = area.PixelsHorizontal;
        var height = area.PixelsVertical;
        var patchSize = m_fractalSettings.PatchSize;

        var horizontalPatches = width / patchSize + (width % patchSize != 0 ? 1 : 0);
        var vertitalPatches = height / patchSize + (height % patchSize != 0 ? 1 : 0);

        var patches = new List<IFractal>();

        for (int i = 0; i < horizontalPatches; ++i)
        {
            var startIndexWidth = i * patchSize;

            for (int j = 0; j < vertitalPatches; ++j)
            {
                var startIndexHeight = j * patchSize;
                var stopIndexHeight = Math.Min(startIndexHeight + patchSize, height);

                var areaPatch = new FractalAreaPatch(startIndexWidth, startIndexHeight, patchSize);
                var fractal = FractalFactory.CreateFractal(m_fractalSettings, areaPatch);
                patches.Add(fractal);
            }
        }

        return patches;
    }

    protected override void ThreadProc()
    {
        Stop = false;
        LockMutex();
        m_progress = 0.0;
        UnlockMutex();

        if (UpdateColors)
        {
            UpdateColors = false;
            RunColorist();
        }
        else
        {
            ComputeFractals();
        }

        LockMutex();
        m_progress = 100.0; // Ensure progress is set to 100% when complete
        Stopped = true;
        UnlockMutex();

        // Raise the completion event
        ComputationCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void RunColorist()
    {
        LockMutex();
        var fractals = m_fractalsCompleted.ToArray();
        m_fractalsCompleted.Clear();
        var count = fractals.Length;
        UnlockMutex();

        int index = 0;
        foreach (var fractal in fractals)
        {
            fractal.Colorist.Update(fractal);

            LockMutex();
            m_fractalsToUpdate.Add(fractal);
            m_progress = ++index * 100.0 / count;
            UnlockMutex();
        }
    }

    private void ComputeFractals()
    {
        // Initialize DistributionGraph to zero
        if (m_fractalSettings.DistributionGraph != null)
        {
            for (int i = 0; i < m_fractalSettings.DistributionGraph.Length; i++)
            {
                m_fractalSettings.DistributionGraph[i] = 0;
            }
        }

        var waitingFractals = GetPatches(m_fractalSettings.FractalArea.DisplayArea);
        var totalFractals = waitingFractals.Count;
        var completedFractals = 0;

        var processorCount = Environment.ProcessorCount;
        var startedFractals = new List<IFractal>();

        // Create a reset event that fractals will signal when they complete
        var completionEvent = new ManualResetEventSlim(false);

        // Create a semaphore to limit the number of concurrent threads from the pool
        var maxConcurrentThreads = processorCount - 1;
        var semaphore = new SemaphoreSlim(maxConcurrentThreads, maxConcurrentThreads);

        // Track queued/running callbacks so cleanup can wait for them to finish
        var pendingWorkItems = 0;
        var allWorkItemsCompleted = new ManualResetEventSlim(true);

        // Create a thread pool executor that manages concurrency
        void threadPoolExecutor(Action work)
        {
            semaphore.Wait();

            Interlocked.Increment(ref pendingWorkItems);
            allWorkItemsCompleted.Reset();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    work();
                }
                finally
                {
                    LockMutex();
                    try
                    {
                        semaphore?.Release();
                        completionEvent?.Set();
                    }
                    finally
                    {
                        UnlockMutex();
                    }

                    if (Interlocked.Decrement(ref pendingWorkItems) == 0)
                    {
                        allWorkItemsCompleted.Set();
                    }
                }
            });
        }

        int fractalCount = 0;

        while (!Stop)
        {
            while (startedFractals.Count < maxConcurrentThreads)
            {
                var fractal = waitingFractals.FirstOrDefault();
                if (fractal == null)
                {
                    break;
                }

                waitingFractals.Remove(fractal);
                startedFractals.Add(fractal);
                completionEvent.Reset();
                fractal.StartThread(threadPoolExecutor);
            }

            completionEvent.Wait();

            var fractals = startedFractals.ToArray();
            var fractalsStopped = false;

            foreach (var fractal in fractals)
            {
                if (fractal.Stopped)
                {
                    LockMutex();
                    if (fractal.State == ComputationState.SomeMaxIterationsReached)
                    {
                        var subdividedFractals = fractal.Subdivide();
                        waitingFractals.InsertRange(0, subdividedFractals);
                        totalFractals += subdividedFractals.Length;
                    }

                    m_fractalsToUpdate.Add(fractal);
                    ++fractalCount;
                    ++completedFractals;

                    // Update progress percentage
                    m_progress = completedFractals * 100.0 / totalFractals;

                    UnlockMutex();

                    startedFractals.Remove(fractal);

                    fractalsStopped = true;
                }
            }

            if (fractalsStopped && fractalCount >= 4)
            {
                m_bitmapUpdateEvent.Set(); // Wake up the main thread to update the bitmap
                fractalCount = 0;
            }

            if (startedFractals.Count == 0 && waitingFractals.Count == 0)
            {
                break;
            }
        }

        startedFractals.Clear();
        waitingFractals.Clear();

        // Ensure all queued callbacks have exited before disposing synchronization primitives
        allWorkItemsCompleted.Wait();

        completionEvent.Dispose();
        completionEvent = null;
        semaphore.Dispose();
        semaphore = null;
        allWorkItemsCompleted.Dispose();
    }

    public bool Update(SKBitmap bitmap)
    {
        var updated = false;

        while (true)
        {
            LockMutex();
            var fractal = m_fractalsToUpdate.FirstOrDefault();
            UnlockMutex();

            if (fractal == null)
            {
                break;
            }

            UpdateBitmap(bitmap, fractal);

            LockMutex();
            m_fractalsToUpdate.Remove(fractal);
            m_fractalsCompleted.Add(fractal);
            UnlockMutex();

            updated = true;
        }

        return updated;
    }

    public void Dispose()
    {
        LockMutex();
        foreach (var fractal in m_fractalsCompleted)
        {
            fractal.AreaPatch.Dispose();
        }
        m_fractalsCompleted.Clear();
        UnlockMutex();

        GC.SuppressFinalize(this);
    }

    public static SKBitmap GetBitmap(int width, int height)
    {
        var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        DefaultFill(bitmap);
        return bitmap;
    }

    private static void DefaultFill(SKBitmap bitmap)
    {
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Azure);
    }

    public static void UpdateBitmap(SKBitmap bitmap, IFractal fractal)
    {
        var areaPatch = fractal.AreaPatch;

        var fractalImage = areaPatch.FractalImage;
        var image = fractalImage.Image;
        if (image == null)
            return;

        using var canvas = new SKCanvas(bitmap);
        var targetRect = areaPatch.GetTargetRectangle(bitmap.Width, bitmap.Height);
        var sourceRect = areaPatch.GetSourceRectangle(bitmap.Width, bitmap.Height);

        // Convert rectangles to SKRect
        var skTargetRect = new SKRect(targetRect.Left, targetRect.Top, targetRect.Right, targetRect.Bottom);
        var skSourceRect = new SKRect(sourceRect.Left, sourceRect.Top, sourceRect.Right, sourceRect.Bottom);

        canvas.DrawBitmap(image, skSourceRect, skTargetRect, SKSamplingOptions.Default);
    }
}
