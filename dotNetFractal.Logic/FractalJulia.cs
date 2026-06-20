using System;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Compute a Julia fractal.
    /// </summary>
    public class FractalJulia<T> : Fractal<T> where T : IFractalUnit<T>, new()
    {
        private T m_startingPointX = new();
        private T m_startingPointY = new();

        public T StartingPointX => m_startingPointX;
        public T StartingPointY => m_startingPointY;

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
                    T Radius2 = new();
                    T PrevRadius2 = new();
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

                    Area.Pixels.SetPixel(i, j, new FractalPixel<T>(teller, Radius2, PrevRadius2));
                }
            }

            UpdateAreaPatchFractalImage();

            Stopped = true;
        }

        public void SetStartingPoint(T x, T y)
        {
            m_startingPointX = x;
            m_startingPointY = y;
        }
    }
}
