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
        protected override FractalPixel<T> Compute(T maxRadius, int maxIterations, DisplayArea<T> displayArea, int i, int j)
        {
            var Cx = displayArea.Cx;
            var Cy = displayArea.Cy;
            var x = displayArea.GetX(i);
            var y = displayArea.GetY(j);

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
    }
}
