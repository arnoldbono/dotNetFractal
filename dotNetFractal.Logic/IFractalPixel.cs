
using System.IO;

namespace dotNetFractal.Logic
{
    public interface IFractalPixel
    {
        int Iteration { get; }

        double GetEscapeFraction(double maxRadius);

        void Write(BinaryWriter bw);
    }
}
