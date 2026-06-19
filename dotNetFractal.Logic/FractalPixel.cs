using System;
using System.IO;

namespace dotNetFractal.Logic
{
    public class FractalPixel
    {
        public int Iteration { get; set; }

        public decimal Radius { get; set; }

        public decimal PreviousRadius { get; set; }

        private FractalPixel()
        {
            ; // serialization only
        }

        public FractalPixel(int iteration, decimal radius, decimal previousRadius)
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
                Radius = br.ReadDecimal(),
                PreviousRadius = br.ReadDecimal()
            };
        }
    }
}
