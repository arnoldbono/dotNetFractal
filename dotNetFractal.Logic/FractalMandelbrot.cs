using System;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Computes the Mandelbrot fractal.
    /// </summary>
    public class FractalMandelbrot : Fractal
    {
        public FractalMandelbrot()
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

            var displayArea = Area.DisplayArea;

            // plot Mandelbrot formula
            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                var Cx = displayArea.GetX(i);

                for (var j = startIndexHeight; j < stopIndexHeight; ++j)
                {
                    if (!Area.Pixels.Inside(i, j))
                    {
                        continue;
                    }

                    var Cy = displayArea.GetY(j);
                    var x = Cx;
                    var y = Cy;
                    int teller = 0;
                    var Radius2 = 0.0m;
                    var PrevRadius2 = 0.0m;
                    while (++teller < MaxIterations)
                    {
                        PrevRadius2 = Radius2;

                        var xx = x * x;
                        var yy = y * y;

                        if ((Radius2 = xx + yy) > MaxRadius)
                        {
                            break;
                        }

                        y *= x;
                        y += y + Cy;
                        x = xx - yy + Cx;
                    }

                    Area.Pixels.SetPixel(i, j, new FractalPixel(teller, Radius2, PrevRadius2));
                }
            }

            UpdateAreaPatchFractalImage();

            Stopped = true;
        }
    }
}
