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

            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                var Cx = displayArea.GetX(i);

                for (var j = startIndexHeight; j < stopIndexHeight && !Stop; ++j)
                {
                    if (!Area.Pixels.Inside(i, j))
                    {
                        continue;
                    }

                    var Cy = displayArea.GetY(j);
                    var x = Cx;
                    var y = Cy;
              
                    int teller = 0;
                    var Radius2 = new T();
                    var PrevRadius2 = new T();
                    while (++teller < maxIterations)
                    {
                        PrevRadius2 = Radius2;

                        var xx = x * x;      // x²
                        var yy = y * y;      // y²

                        if ((Radius2 = xx + yy) > maxRadius)
                        {
                            break;
                        }

                        y *= x;              // y = x*y (temporary)
                        y += y + Cy;         // y = (x*y) + (x*y) + Cy = 2*x*y + Cy
                        x = xx - yy + Cx;    // x = x² - y² + Cx
                    }

                    Area.Pixels.SetPixel(i, j, new FractalPixel<T>(teller, Radius2, PrevRadius2));
                }
            }

            UpdateAreaPatchFractalImage();

            Stopped = true;
        }
    }
}
