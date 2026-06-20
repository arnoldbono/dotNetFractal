
using System.IO;

namespace dotNetFractal.Logic
{
    public interface IDisplayArea
    {
        int PixelsHorizontal { get; }

        int PixelsVertical { get; }

        void Write(BinaryWriter bw);
    }
}
