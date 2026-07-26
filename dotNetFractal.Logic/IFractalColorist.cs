using SkiaSharp;

namespace dotNetFractal.Logic;

/// <summary>
/// A colorist is responsible for mapping an iteration count to a color. The colorist can be used to create different color schemes for the fractal.
/// </summary>
public interface IFractalColorist
{
    void UpdateAreaPatch(IFractalArea area, FractalAreaPatch areaPatch);

    void Update(IFractal fractal);
}
