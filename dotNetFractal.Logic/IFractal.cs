using System;
using System.Drawing;

namespace dotNetFractal
{
	/// <summary>
	/// Compute a Fractal from left to right.
	/// </summary>
	public interface IFractal
	{
        FractalArea Area { get; set; }

        FractalAreaPatch AreaPatch { get; set; }

        int ComputedWidth { get; }

        bool Stopped { get; }

        double MaxRadius { get; set; }

        int MaxIterations { get; set; }

        Color ComputeColor(int iteration, double previousRadius, double radius);

        void GetColor(int index, out int red, out int green, out int blue);

        void StartThread();
    }
}
