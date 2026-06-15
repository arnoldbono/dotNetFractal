using System;
using System.Drawing;

namespace dotNetFractal.Logic
{
    public class FractalAreaPatch : IDisposable
    {
        public int StartIndexWidth => (int)FractalImage.IndexI;

        public int StopIndexWidth => StartIndexWidth + Size;

        public int Size => FractalImage.Size;

        public int StartIndexHeight => (int)FractalImage.IndexJ;

        public int StopIndexHeight => StartIndexHeight + Size;

        public FractalCachedImage FractalImage { get; private set; }

        public FractalAreaPatch(int startIndexWidth, int startIndexHeight, int size)
        {
            FractalImage = new FractalCachedImage((UInt64)startIndexWidth, (UInt64)startIndexHeight, size, 0);
        }

        public Rectangle GetTargetRectangle(int width, int height)
        {
            var x = StartIndexWidth;
            var y = StartIndexHeight;
            var rectWidth = Math.Min(Size, width - x);
            var rectHeight = Math.Min(Size, height - y);
            return new Rectangle(x, y, rectWidth, rectHeight);
        }

        public Rectangle GetSourceRectangle(int width, int height)
        {
            var x = StartIndexWidth;
            var y = StartIndexHeight;
            var rectWidth = Math.Min(Size, width - x);
            var rectHeight = Math.Min(Size, height - y);
            return new Rectangle(0, 0, rectWidth, rectHeight);
        }

        public void Dispose()
        {
            FractalImage?.Dispose();
            FractalImage = null;

            GC.SuppressFinalize(this);
        }
    }
}
