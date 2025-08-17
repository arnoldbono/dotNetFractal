using System;

namespace dotNetFractal
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
            ComputedWidth = 0;

            var startIndexWidth = AreaPatch.StartIndexWidth;
            var stopIndexWidth = AreaPatch.StopIndexWidth;
            var startIndexHeight = AreaPatch.StartIndexHeight;
            var stopIndexHeight = AreaPatch.StopIndexHeight;

            // plot Mandelbrot formula
            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
			{
				ComputedWidth = i - startIndexWidth;
                double Cx = Area.GetX(i);

                for (var j = startIndexHeight; j < stopIndexHeight; ++j)
				{
                    double Cy = Area.GetY(j);
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

			if (!Stop)
			{
                ComputedWidth = stopIndexWidth - startIndexWidth;
            }
            
            Stopped = true;
        }
	}
}
