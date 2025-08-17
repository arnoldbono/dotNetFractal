using System;

namespace dotNetFractal
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
    }
}
