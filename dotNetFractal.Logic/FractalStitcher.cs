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
    /// Subdivides FractalQuarters until the DisplayArea is no longer completely within one
    /// whole FractalQuarter.
    /// Also, subdivides those FractalQuarters whose pixels per Width is lower than the number
    /// of pixels of the DisplayArea per Width.
    /// </summary>
    public class FractalStitcher : Worker
    {
        private readonly Func<IFractal> m_fractalFunc;
        private readonly List<IFractal> m_fractalsToUpdate = [];
        private readonly AutoResetEvent m_bitmapUpdateEvent = new (false);
        private DisplayArea m_area;

        private static int PatchSize => 128;

        public DisplayArea Area
        {
            get { return m_area; }
            set
            {
                StopThread();
                m_area = value;
            }
        }

        public WaitHandle BitmapUpdateEvent => m_bitmapUpdateEvent;

        public FractalStitcher(Func<Fractal> fractalFunc, DisplayArea area)
        {
            Debug.Assert(fractalFunc != null && area != null);
            m_fractalFunc = fractalFunc;
            m_area = area;
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

        private List<IFractal> GetPatches(DisplayArea area)
        {
            var fractalArea = new FractalArea(area);

            var width = Area.PixelsHorizontal;
            var height = Area.PixelsVertical;

            var horizontalPatches = width / PatchSize + (width % PatchSize != 0 ? 1 : 0);
            var vertitalPatches = height / PatchSize + (height % PatchSize != 0 ? 1 : 0);

            var patches = new List<IFractal>();

            for (int i = 0; i < horizontalPatches; ++i)
            {
                var startIndexWidth = i * PatchSize;
                var stopIndexWidth = Math.Min(startIndexWidth + PatchSize, width);

                for (int j = 0; j < vertitalPatches; ++j)
                {
                    var startIndexHeight = j * PatchSize;
                    var stopIndexHeight = Math.Min(startIndexHeight + PatchSize, height);

                    var fractal = m_fractalFunc();
                    fractal.Area = fractalArea;
                    fractal.AreaPatch = new FractalAreaPatch(startIndexWidth, startIndexHeight, stopIndexWidth, stopIndexHeight);
                    patches.Add(fractal);
                }
            }

            return patches;
        }

        protected override void ThreadProc()
        {
            Stop = false;

            var waitingFractals = GetPatches(Area);

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
                        m_fractalsToUpdate.Add(fractal);
                        UnlockMutex();

                        startedFractals.Remove(fractal);

                        fractalsStopped = true;
                    }
                }

                if (fractalsStopped)
                {
                    m_bitmapUpdateEvent.Set();
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
            LockMutex();
            var fractal = m_fractalsToUpdate.FirstOrDefault();
            UnlockMutex();

            if (fractal == null)
            {
                return false;
            }

            UpdateBitmap(bitmap, fractal);

            LockMutex();
            m_fractalsToUpdate.Remove(fractal);
            UnlockMutex();

            return true;
        }

        public Bitmap GetBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            DefaultFill(bitmap);
            return bitmap;
        }

        private void DefaultFill(Bitmap bitmap)
        {
            Graphics grfx = Graphics.FromImage(bitmap);
            grfx.FillRectangle(new SolidBrush(Color.Azure), new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            var patches = GetPatches(Area);

            foreach (var fractal in patches)
            {
                var fractalArea = fractal.Area;
                var areaPatch = fractal.AreaPatch;
                var startIndexWidth = areaPatch.StartIndexWidth;
                var stopIndexWidth = areaPatch.StopIndexWidth;
                var startIndexHeight = areaPatch.StartIndexHeight;
                var stopIndexHeight = areaPatch.StopIndexHeight;

                for (var i = startIndexWidth; i < stopIndexWidth && !Stop; i += stopIndexWidth - 1)
                {
                    for (var j = startIndexHeight; j < stopIndexHeight; j += stopIndexWidth - 1)
                    {
                        var pixel = fractalArea.GetPixel(i, j);
                        if (pixel != null)
                        {
                            bitmap.SetPixel(i, j, Color.Black);
                        }
                    }
                }
            }
        }

        public void UpdateBitmap(Bitmap bitmap, IFractal fractal)
        {
            var fractalArea = fractal.Area;
            var areaPatch = fractal.AreaPatch;
            var startIndexWidth = areaPatch.StartIndexWidth;
            var stopIndexWidth = areaPatch.StopIndexWidth;
            var startIndexHeight = areaPatch.StartIndexHeight;
            var stopIndexHeight = areaPatch.StopIndexHeight;

            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                for (var j = startIndexHeight; j < stopIndexHeight; ++j)
                {
                    var pixel = fractalArea.GetPixel(i, j);
                    if (pixel != null)
                    {
                        bitmap.SetPixel(i, j,
                            fractal.ComputeColor(pixel.Iteration, pixel.PreviousRadius, pixel.Radius));
                    }
                }
            }
        }
    }
}
