using System;
using SkiaSharp;

namespace dotNetFractal.Logic;

public enum ComputationState
{
    NotStarted,
    Running,
    AllMaxIterationsReached,
    SomeMaxIterationsReached,
    NoneMaxIterationsReached
}

/// <summary>
/// Compute a Fractal from left to right.
/// </summary>
public interface IFractal
{
    FractalSettings Settings { get; }

    ComputationState State { get; }

    IFractalArea Area { get; }

    FractalAreaPatch AreaPatch { get; set; }

    bool Stopped { get; }

    double MaxRadius { get; }

    SKColor ComputeColor(IFractalPixel pixel);

    void GetColor(int index, out int red, out int green, out int blue);

    void StartThread(Action<Action> threadPoolExecutor);

    IFractal[] Subdivide();
}
