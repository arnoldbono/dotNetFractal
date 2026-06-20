using System;
using System.IO;

namespace dotNetFractal.Logic
{
    public class FractalPixel<T> : IFractalPixel where T : IFractalUnit<T>, new()
    {
        public int Iteration { get; private set; }

        public T Radius { get; private set; }

        public T PreviousRadius { get; private set; }

        public double GetEscapeFraction(double maxRadius)
        {
            // PRE: radius > MaxRadius (otherwise the fractal computation loop should not have stopped).
            // PRE: previousRadius < MaxRadius.
            return Math.Sqrt((double)(((T)maxRadius - PreviousRadius) / (Radius - PreviousRadius)));
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

        public void Write(BinaryWriter bw)
        {
            bw.Write((Int32)Iteration);
            bw.Write((decimal)Radius);
            bw.Write((decimal)PreviousRadius);
        }

        public static IFractalPixel Read(BinaryReader br)
        {
            return new FractalPixel<T>
            {
                Iteration = br.ReadInt32(),
                Radius = (T)br.ReadDecimal(),
                PreviousRadius = (T)br.ReadDecimal()
            };
        }
    }
}
