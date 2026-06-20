
using System.IO;

namespace dotNetFractal.Logic
{
    public interface IDisplayArea
    {
        int PixelsHorizontal { get; }

        int PixelsVertical { get; }

        void Write(BinaryWriter bw);

        IDisplayArea ZoomIn(int i1, int j1, int i2, int j2, int horizontal, int vertical);
    }
}
