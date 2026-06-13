using System;
using System.Drawing;

namespace dotNetFractal.Logic
{
    public class FractalAreaPatch
    {
        public int StartIndexWidth { get; }

        public int StopIndexWidth { get; }

        public int StartIndexHeight { get; }

        public int StopIndexHeight { get; }

        public FractalAreaPatch(int startIndexWidth, int startIndexHeight, int stopIndexWidth, int stopIndexHeight)
        {
            StartIndexWidth = startIndexWidth;
            StartIndexHeight = startIndexHeight;
            StopIndexWidth = stopIndexWidth;
            StopIndexHeight = stopIndexHeight;
        }

        public Rectangle GetRectangle()
        {
            var x = StartIndexWidth;
            var y = StartIndexHeight;
            var width = StopIndexWidth - x;
            var height = StopIndexHeight - y;
            return new Rectangle(x, y, width, height);
        }
    }
}
