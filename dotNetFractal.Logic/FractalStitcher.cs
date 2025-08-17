using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace dotNetFractal
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
        private readonly List<IFractal> m_fractalsToUpdate = new List<IFractal>();
        private DisplayArea m_area;

        public DisplayArea Area
        {
            get { return m_area; }
            set
            {
                StopThread();
                m_area = value;
            }
        }

        public FractalArea FractalArea { get; set; }

        public FractalStitcher(Func<Fractal> fractalFunc, DisplayArea area)
        {
            Debug.Assert(fractalFunc != null && area != null);
            m_fractalFunc = fractalFunc;
            m_area = area;
       }

        protected override void ThreadProc()
        {
            Stop = false;

            var width = m_area.PixelsHorizontal;
            var height = m_area.PixelsVertical;

            var patchSize = 128;
            var horizontalPatches = width / patchSize + (width % patchSize != 0 ? 1 : 0);
            var vertitalPatches = height / patchSize + (height % patchSize != 0 ? 1 : 0);

            var processorCount = Environment.ProcessorCount;
            var startedFractals = new List<IFractal>();
            var waitingFractals = new List<IFractal>();

            FractalArea = new FractalArea(m_area.CenterX, m_area.CenterY, m_area.Width, m_area.PixelsHorizontal, m_area.PixelsVertical);

            for (int i = 0; i < horizontalPatches; ++i)
            {
                var startIndexWidth = i * patchSize;
                var stopIndexWidth = Math.Min(startIndexWidth + patchSize, width);

                for (int j = 0; j < vertitalPatches; ++j)
                {
                    var startIndexHeight = j * patchSize;
                    var stopIndexHeight = Math.Min(startIndexHeight + patchSize, height);

                    var fractal = m_fractalFunc();
                    fractal.Area = FractalArea;
                    fractal.AreaPatch = new FractalAreaPatch(startIndexWidth, startIndexHeight, stopIndexWidth, stopIndexHeight);
                    waitingFractals.Add(fractal);
                }
            }

            while (!Stop)
            {
                while (startedFractals.Count() < processorCount - 1)
                {
                    var fractal = waitingFractals.FirstOrDefault();
                    if (fractal == null)
                    {
                        break;
                    }

                    waitingFractals.Remove(fractal);
                    startedFractals.Add(fractal);
                    fractal.StartThread();
                }

                var fractals = startedFractals.ToArray();

                foreach (var fractal in fractals)
                {
                    if (fractal.Stopped)
                    {
                        LockMutex();
                        m_fractalsToUpdate.Add(fractal);
                        UnlockMutex();

                        startedFractals.Remove(fractal);
                    }
                }

                if (!startedFractals.Any() && !waitingFractals.Any())
                {
                    break;
                }

                Thread.Sleep(100);
            }

            startedFractals.Clear();
            waitingFractals.Clear();

            LockMutex();
            Stopped = true;
            UnlockMutex();
        }

        public Rectangle Update(Bitmap bitmap)
        {
            Rectangle retval = Rectangle.Empty;

            LockMutex();
            var fractal = m_fractalsToUpdate.FirstOrDefault();
            UnlockMutex();

            if (fractal != null)
            {
                UpdateBitmap(bitmap, fractal);

                LockMutex();
                m_fractalsToUpdate.Remove(fractal);
                UnlockMutex();

                var areaPatch = fractal.AreaPatch;
                var x = areaPatch.StartIndexWidth;
                var y = areaPatch.StartIndexHeight;
                var width = areaPatch.StopIndexWidth - x;
                var height = areaPatch.StopIndexHeight - y;

                retval = new Rectangle(x, y, width, height);
            }

            return retval;
        }

        public Bitmap GetBitmap(int width, int height)
        {
            return new Bitmap(width, height, PixelFormat.Format32bppArgb);
        }

        public void DefaultFill(Bitmap bitmap)
        {
            Graphics grfx = Graphics.FromImage(bitmap);
            grfx.FillRectangle(new SolidBrush(Color.Azure), new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        }

        public void UpdateBitmap(Bitmap bitmap, IFractal fractal)
        {
            var areaPatch = fractal.AreaPatch;
            var startIndexWidth = areaPatch.StartIndexWidth;
            var stopIndexWidth = areaPatch.StopIndexWidth;
            var startIndexHeight = areaPatch.StartIndexHeight;
            var stopIndexHeight = areaPatch.StopIndexHeight;

            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                double x = Area.GetX(i);
                for (var j = startIndexHeight; j < stopIndexHeight; ++j)
                {
                    double y = Area.GetY(j);
                    var pixel = FractalArea.GetPixel(x, y);
                    if (pixel != null)
                    {
                        bitmap.SetPixel(i, j,
                            fractal.ComputeColor(pixel.Iteration, pixel.PreviousRadius, pixel.Radius));
                    }
                }
                Thread.Sleep(0);
            }
        }
    }
}
