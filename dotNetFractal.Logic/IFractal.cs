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
/// Compute a fractal from left to right, top to bottom, pixel by pixel, for the given area and settings, and store the result in a pixel array.
/// </summary>
public interface IFractal
{
    ComputationState State { get; }

    FractalAreaPatch AreaPatch { get; }

    IFractalColorist Colorist { get; }

    bool Stopped { get; }

    void StartThread(Action<Action> threadPoolExecutor);

    IFractal[] Subdivide();
}
