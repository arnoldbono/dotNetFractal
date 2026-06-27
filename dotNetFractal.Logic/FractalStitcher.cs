using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Subdivides the display area into patches and computes these using worker threads.
    /// </summary>
    public class FractalStitcher : Worker
    {
        private readonly FractalSettings m_fractalSettings;
        private readonly List<IFractal> m_fractalsToUpdate = [];
        private readonly AutoResetEvent m_bitmapUpdateEvent = new (false);

        private static int PatchSize => 128;

        public FractalSettings FractalSettings => m_fractalSettings;

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

        private List<IFractal> GetPatches(IDisplayArea area)
        {
            var width = area.PixelsHorizontal;
            var height = area.PixelsVertical;

            var horizontalPatches = width / PatchSize + (width % PatchSize != 0 ? 1 : 0);
            var vertitalPatches = height / PatchSize + (height % PatchSize != 0 ? 1 : 0);

            var patches = new List<IFractal>();

            for (int i = 0; i < horizontalPatches; ++i)
            {
                var startIndexWidth = i * PatchSize;

                for (int j = 0; j < vertitalPatches; ++j)
                {
                    var startIndexHeight = j * PatchSize;
                    var stopIndexHeight = Math.Min(startIndexHeight + PatchSize, height);

                    var fractal = FractalFactory.CreateFractal(m_fractalSettings);
                    fractal.AreaPatch = new FractalAreaPatch(startIndexWidth, startIndexHeight, PatchSize);
                    patches.Add(fractal);
                }
            }

            return patches;
        }

        protected override void ThreadProc()
        {
            Stop = false;

            var waitingFractals = GetPatches(m_fractalSettings.FractalArea.DisplayArea);

            var processorCount = Environment.ProcessorCount;
            var startedFractals = new List<IFractal>();

            // Create a reset event that fractals will signal when they complete
            var completionEvent = new ManualResetEventSlim(false);

            // Create a semaphore to limit the number of concurrent threads from the pool
            var maxConcurrentThreads = processorCount - 1;
            var semaphore = new SemaphoreSlim(maxConcurrentThreads, maxConcurrentThreads);

            // Create a thread pool executor that manages concurrency
            void threadPoolExecutor(Action work)
            {
                semaphore.Wait();
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        work();
                    }
                    finally
                    {
                        LockMutex();
                        semaphore?.Release();
                        completionEvent?.Set();
                        UnlockMutex();
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
                        bool subdivide = fractal.State == ComputationState.SomeMaxIterationsReached;
                        if (subdivide)
                        {
                            var subdividedFractals = fractal.Subdivide();
                            waitingFractals.InsertRange(0, subdividedFractals);
                        }

                        m_fractalsToUpdate.Add(fractal);
                        ++fractalCount;

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

            LockMutex();
            startedFractals.Clear();
            waitingFractals.Clear();
            completionEvent.Dispose();
            completionEvent = null;
            semaphore.Dispose();
            semaphore = null;
            Stopped = true;
            UnlockMutex();
        }

        public bool Update(Bitmap bitmap)
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
                UnlockMutex();

                fractal.AreaPatch.Dispose();
                fractal.AreaPatch = null;

                updated = true;
            }

            return updated;
        }

        public static Bitmap GetBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            DefaultFill(bitmap);
            return bitmap;
        }

        private static void DefaultFill(Bitmap bitmap)
        {
            Graphics grfx = Graphics.FromImage(bitmap);
            grfx.FillRectangle(new SolidBrush(Color.Azure), new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        }

        public void UpdateBitmap(Bitmap bitmap, IFractal fractal)
        {
            var areaPatch = fractal.AreaPatch;

            var fractalImage = areaPatch.FractalImage;
            var image = (Bitmap)fractalImage.Image;

            Graphics grfx = Graphics.FromImage(bitmap);
            grfx.DrawImage(image, areaPatch.GetTargetRectangle(bitmap.Width, bitmap.Height), areaPatch.GetSourceRectangle(bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
        }
    }
}
