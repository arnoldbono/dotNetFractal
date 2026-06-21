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

            // Compute Mandelbrot fractal for the given area patch
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

                        var xx = x * x;
                        var yy = y * y;

                        if ((Radius2 = xx + yy) > maxRadius)
                        {
                            break;
                        }

                        y *= x;
                        y += y + Cy;
                        x = xx - yy + Cx;
                    }

                    Area.Pixels.SetPixel(i, j, new FractalPixel<T>(teller, Radius2, PrevRadius2));
                }
            }

            UpdateAreaPatchFractalImage();

            Stopped = true;
        }
    }
}
