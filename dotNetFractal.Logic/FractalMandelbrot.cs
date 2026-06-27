using Microsoft.VisualBasic;
using System;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Computes the Mandelbrot fractal.
    /// </summary>
    public class FractalMandelbrot<T> : Fractal<T> where T : IFractalUnit<T>, new()
    {
        public FractalMandelbrot(FractalSettings settings) : base(settings)
        {
            ; // Empty
        }

        private static FractalPixel<T> Compute(T maxRadius, int maxIterations, DisplayArea<T> displayArea, int i, int j)
        {
            var Cx = displayArea.GetX(i);
            var Cy = displayArea.GetY(j);
            var x = Cx;
            var y = Cy;

            int iteration = 0;
            var radius2 = new T();
            var prevRadius2 = new T();
            while (iteration++ < maxIterations)
            {
                prevRadius2 = radius2;

                var xx = x * x;      // x²
                var yy = y * y;      // y²

                if ((radius2 = xx + yy) > maxRadius)
                {
                    break;
                }

                y *= x;              // y = x*y (temporary)
                y += y + Cy;         // y = (x*y) + (x*y) + Cy = 2*x*y + Cy
                x = xx - yy + Cx;    // x = x² - y² + Cx
            }

            return new FractalPixel<T>(iteration, radius2, prevRadius2);
        }

        /// <summary>
        /// Compute Mandelbrot fractal for the given area patch
        /// For each pixel at position (Cx, Cy):
        /// Start with z₀ = (Cx, Cy)
        /// Iterate: z_{n+1} = z_n² + c, where c = (Cx, Cy)
        /// </summary>
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

                            var pixel = Compute(maxRadius, maxIterations, displayArea, i, j);
                            Area.Pixels.SetPixel(i, j, pixel);
                        }
                    }
                }
                else
                {
                    m_state = someMaxIteractionReached && AreaPatch.Size > 16 ? ComputationState.SomeMaxIterationsReached : ComputationState.NoneMaxIterationsReached;
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
    }
}
