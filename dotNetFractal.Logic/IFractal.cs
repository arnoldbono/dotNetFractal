using System;
using System.Drawing;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Compute a Fractal from left to right.
    /// </summary>
    public interface IFractal
    {
        IFractalArea Area { get; set; }

        FractalAreaPatch AreaPatch { get; set; }

        bool Stopped { get; }

        double MaxRadius { get; set; }

        int MaxIterations { get; set; }

        bool SmoothColoring { get; set; }

        Color ComputeColor(IFractalPixel pixel);

        void GetColor(int index, out int red, out int green, out int blue);

        void StartThread(Action<Action> threadPoolExecutor = null);
    }
}
