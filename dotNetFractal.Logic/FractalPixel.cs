using System;
using System.IO;

namespace dotNetFractal
{
    public class FractalPixel
    {
        public int Iteration { get; set; }

        public double Radius { get; set; }

        public double PreviousRadius { get; set; }

        private FractalPixel()
        {
            ; // serialization only
        }

        public FractalPixel(int iteration, double radius, double previousRadius)
        {
            Iteration = iteration;
            Radius = radius;
            PreviousRadius = previousRadius;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((Int32)Iteration);
            bw.Write(Radius);
            bw.Write(PreviousRadius);
        }

        public static FractalPixel Read(BinaryReader br)
        {
            return new FractalPixel
            {
                Iteration = br.ReadInt32(),
                Radius = br.ReadDouble(),
                PreviousRadius = br.ReadDouble()
            };
        }
    }
}
