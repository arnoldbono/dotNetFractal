using System;
using System.IO;

namespace dotNetFractal.Logic
{
    public class FractalPixel<T> where T : IFractalUnit<T>, new()
    {
        public int Iteration { get; set; }

        public T Radius { get; set; }

        public T PreviousRadius { get; set; }

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

        public static FractalPixel<T> Read(BinaryReader br)
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
