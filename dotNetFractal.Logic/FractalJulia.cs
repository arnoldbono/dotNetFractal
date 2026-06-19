using System;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Compute a Julia fractal.
    /// </summary>
    public class FractalJulia : Fractal
    {
        private decimal m_startingPointX = 0.0m;
        private decimal m_startingPointY = 0.0m;

        public FractalJulia()
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

            // plot Julia Set using Mandelbrot formula
            var Cx = m_startingPointX;
            var Cy = m_startingPointY;
            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                var x0 = displayArea.GetX(i);

                for (var j = startIndexHeight; j < stopIndexHeight && !Stop; ++j)
                {
                    var y0 = displayArea.GetY(j);
                    var x = x0;
                    var y = y0;
                    int teller = 0;
                    decimal Radius2 = 0.0m;
                    decimal PrevRadius2 = 0.0m;
                    while (++teller < MaxIterations)
                    {
                        PrevRadius2 = Radius2;

                        decimal xx = x * x;
                        decimal yy = y * y;

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

        public decimal StartingPointX => m_startingPointX;
        public decimal StartingPointY => m_startingPointY;

        public void SetStartingPoint(decimal x, decimal y)
        {
            m_startingPointX = x;
            m_startingPointY = y;
        }
    }
}
