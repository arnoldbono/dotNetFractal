using System;
using System.Numerics;

namespace dotNetFractal.Logic
{
    public class FractalPixel<T> : IFractalPixel where T : INumber<T>, new()
    {
        public int Iteration { get; private set; }

        public T Radius { get; private set; }

        public T PreviousRadius { get; private set; }

        public double GetEscapeFraction(double maxRadius)
        {
            // PRE: radius > MaxRadius (otherwise the fractal computation loop should not have stopped).
            // PRE: previousRadius < MaxRadius.
            return Math.Sqrt(double.CreateChecked((T.CreateChecked(maxRadius) - PreviousRadius) / (Radius - PreviousRadius)));
        }

        private FractalPixel()
        {
            ; // serialization only
        }

        public FractalPixel(int iteration, T radius, T previousRadius)
        {
            Iteration = iteration;
            Radius = radius;
            PreviousRadius = previousRadius;
        }
    }
}
