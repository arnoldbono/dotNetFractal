using System;
using System.Collections.Generic;
using System.Drawing;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Compute a Fractal from left to right.
    /// </summary>
    abstract public class Fractal<T> : Worker, IFractal where T : IFractalUnit<T>, new()
    {
        private readonly FractalColorMap m_colorMap = FractalColorMap.GetInstance();

        private readonly FractalSettings m_settings;
        private FractalAreaPatch m_areaPatch = null;
        private double m_maxRadius = 4.0;
        protected ComputationState m_state = ComputationState.NotStarted;

        public IFractalArea Area => m_settings.FractalArea;

        public FractalAreaPatch AreaPatch
        {
            get { return m_areaPatch; }
            set
            {
                if (m_areaPatch != null)
                    base.StopThread();

                m_areaPatch = value;
            }
        }

        public double MaxRadius
        {
            get { return m_maxRadius; }
            set { m_maxRadius = value; }
        }

        public FractalSettings Settings => m_settings;

        public ComputationState State => m_state;

        public Fractal(FractalSettings settings)
        {
            m_settings = settings;
        }

        public Color ComputeColor(IFractalPixel pixel)
        {
            var iteration = pixel.Iteration;
            if (iteration >= Settings.MaxIterations)
                return Color.Black;

            GetColor(iteration, out var red, out var green, out var blue);

            if (iteration != 0 && Settings.SmoothColoring)
            {
                var fraction = pixel.GetEscapeFraction((double)MaxRadius);
                System.Diagnostics.Debug.Assert(fraction < 1.0);

                GetColor(iteration - 1, out var red1, out var green1, out var blue1);

                red = (int)((double)red1 + fraction * (red - red1));
                green = (int)((double)green1 + fraction * (green - green1));
                blue = (int)((double)blue1 + fraction * (blue - blue1));
            }

            return Color.FromArgb(red, green, blue);
        }

        public void GetColor(int index, out int red, out int green, out int blue)
        {
            var fraction = (index % Settings.MaxColorSteps) / (double)Settings.MaxColorSteps;
            var color = m_colorMap.GetColor(fraction);
            red = color.Red;
            green = color.Green;
            blue = color.Blue;
        }

        public override void StartThread(Action<Action> threadPoolExecutor = null)
        {
            base.StartThread(threadPoolExecutor);
        }

        protected abstract FractalPixel<T> Compute(T maxRadius, int maxIterations, DisplayArea<T> displayArea, int i, int j);

        protected override void ThreadProc()
        {
            Stop = false;
            Stopped = false;

            var startIndexWidth = AreaPatch.StartIndexWidth;
            var stopIndexWidth = AreaPatch.StopIndexWidth;
            var startIndexHeight = AreaPatch.StartIndexHeight;
            var stopIndexHeight = AreaPatch.StopIndexHeight;

            var displayArea = (DisplayArea<T>)Area.DisplayArea;
            var maxRadius = (T)MaxRadius;
            var maxIterations = Settings.MaxIterations;

            bool allMaxIteractionReached = true;
            bool someMaxIteractionReached = false;
            m_state = ComputationState.Running;
            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                for (var j = startIndexHeight; j < stopIndexHeight && !Stop; ++j)
                {
                    if (!Area.Pixels.Inside(i, j) ||
                        i > startIndexWidth && i < stopIndexWidth - 1 && j > startIndexHeight && j < stopIndexHeight - 1)
                    {
                        continue;
                    }

                    var pixel = Compute(maxRadius, maxIterations, displayArea, i, j);
                    Area.Pixels.SetPixel(i, j, pixel);

                    if (pixel.Iteration < maxIterations)
                    {
                        allMaxIteractionReached = false;
                    }
                    else
                    {
                        someMaxIteractionReached = true;
                    }
                }
            }
            {
                if (allMaxIteractionReached)
                {
                    m_state = ComputationState.AllMaxIterationsReached;
                    for (var i = startIndexWidth + 1; i < stopIndexWidth - 1 && !Stop; ++i)
                    {
                        for (var j = startIndexHeight + 1; j < stopIndexHeight - 1 && !Stop; ++j)
                        {
                            if (!Area.Pixels.Inside(i, j))
                            {
                                continue;
                            }

                            Area.Pixels.SetPixel(i, j, new FractalPixel<T>(maxIterations, maxRadius, maxRadius));
                        }
                    }
                }
                else
                {
                    m_state = someMaxIteractionReached && AreaPatch.Size > 16 ?
                        ComputationState.SomeMaxIterationsReached :
                        ComputationState.NoneMaxIterationsReached;
                    if (m_state == ComputationState.NoneMaxIterationsReached)
                    {
                        for (var i = startIndexWidth + 1; i < stopIndexWidth - 1 && !Stop; ++i)
                        {
                            for (var j = startIndexHeight + 1; j < stopIndexHeight - 1 && !Stop; ++j)
                            {
                                if (!Area.Pixels.Inside(i, j))
                                {
                                    continue;
                                }

                                var pixel = Compute(maxRadius, maxIterations, displayArea, i, j);
                                Area.Pixels.SetPixel(i, j, pixel);
                            }
                        }
                    } // else fractal gets subdivided later on and skip the inner pixels for now
                }
            }

            UpdateAreaPatchFractalImage();

            Stopped = true;
        }

        protected void UpdateAreaPatchFractalImage()
        {
            var fractalArea = Area;
            var areaPatch = AreaPatch;

            var fractalImage = areaPatch.FractalImage;
            var image = (Bitmap)fractalImage.Image;
            var size = fractalImage.Size;

            for (var i = 0; i < size && !Stop; ++i)
            {
                for (var j = 0; j < size && !Stop; ++j)
                {
                    var pixel = fractalArea.GetPixel(areaPatch.StartIndexWidth + i, areaPatch.StartIndexHeight + j);
                    if (pixel != null)
                    {
                        image.SetPixel(i, j, ComputeColor(pixel));
                    }
                }
            }
        }

        public IFractal[] Subdivide()
        {
            var patchSize = AreaPatch.Size / 2;
            var fractals = new IFractal[4];

            (int startLocationX, int startLocationY)[] sizes = 
            [
                (AreaPatch.StartIndexWidth, AreaPatch.StartIndexHeight),
                (AreaPatch.StartIndexWidth + patchSize, AreaPatch.StartIndexHeight),
                (AreaPatch.StartIndexWidth, AreaPatch.StartIndexHeight + patchSize),
                (AreaPatch.StartIndexWidth + patchSize, AreaPatch.StartIndexHeight + patchSize)
            ];

            int i = 0;
            foreach (var size in sizes)
            {
                var fractal = FractalFactory.CreateFractal(m_settings);
                fractal.AreaPatch = new FractalAreaPatch(size.startLocationX, size.startLocationY, patchSize);
                fractals[i++] = fractal;
            }
            return fractals;
        }
    }
}
