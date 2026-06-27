using Microsoft.VisualBasic;
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
        protected override FractalPixel<T> Compute(T maxRadius, int maxIterations, DisplayArea<T> displayArea, int i, int j)
        {
            var Cx = displayArea.GetX(i);
            var Cy = displayArea.GetY(j);
            var x = Cx;
            var y = Cy;

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
