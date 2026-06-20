
using System.IO;

namespace dotNetFractal.Logic
{
    public interface IFractalArea
    {
        IDisplayArea DisplayArea { get; }

        FractalPixels Pixels { get; }

        IFractalPixel GetPixel(int i, int j);

        void Read(BinaryReader br);

        void Write(BinaryWriter bw);
    }
}
