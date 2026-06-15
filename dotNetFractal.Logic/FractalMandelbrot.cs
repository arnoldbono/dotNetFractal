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
                double Cx = displayArea.GetX(i);

                for (var j = startIndexHeight; j < stopIndexHeight; ++j)
                {
                    if (!Area.Pixels.Inside(i, j))
                    {
                        continue;
                    }

                    double Cy = displayArea.GetY(j);
                    double x = Cx;
                    double y = Cy;
                    int teller = 0;
                    double Radius2 = 0.0;
                    double PrevRadius2 = 0.0;
                    while (++teller < MaxIterations)
                    {
                        PrevRadius2 = Radius2;

                        double xx = x * x;
                        double yy = y * y;

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
            
            Stopped = true;
        }
    }
}
