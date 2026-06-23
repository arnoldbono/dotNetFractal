using System;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Compute a Julia Set fractal.
    /// </summary>
    public class FractalJuliaSet<T>(FractalSettings settings) : Fractal<T>(settings) where T : IFractalUnit<T>, new()
    {
        /// <summary>
        /// Compute the Julia Set fractal for the given area patch.
        /// z_{n+1} = z_n² + c
        /// In component form:
        /// Real:      x_{n+1} = x_n² - y_n² + Cx
        /// Imaginary: y_{n+1} = 2x_n y_n + Cy
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

            var Cx = displayArea.Cx;
            var Cy = displayArea.Cy;

            var maxRadius = (T)MaxRadius;
            var maxIterations = Settings.MaxIterations;

            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                var x0 = displayArea.GetX(i);

                for (var j = startIndexHeight; j < stopIndexHeight && !Stop; ++j)
                {
                    if (!Area.Pixels.Inside(i, j))
                    {
                        continue;
                    }

                    var y0 = displayArea.GetY(j);
                    var x = x0;
                    var y = y0;
                    int teller = 0;
                    T Radius2 = new();
                    T PrevRadius2 = new();
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
