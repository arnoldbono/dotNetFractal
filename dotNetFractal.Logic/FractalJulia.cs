using System;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Summary description for JuliaFractal.
    /// </summary>
    public class FractalJulia : Fractal
    {
        private double m_startingPointX = 0.0;
        private double m_startingPointY = 0.0;

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
            double Cx = m_startingPointX;
            double Cy = m_startingPointY;
            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                double x0 = displayArea.GetX(i);

                for (var j = startIndexHeight; j < stopIndexHeight; ++j)
                {
                    double y0 = displayArea.GetY(j);
                    double x = x0;
                    double y = y0;
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

        public double StartingPointX
        {
            get { return m_startingPointX; }
        }
        public double StartingPointY
        {
            get { return m_startingPointY; }
        }

        public void SetStartingPoint(double x, double y)
        {
            m_startingPointX = x;
            m_startingPointY = y;
        }
    }
}
