
using System.Numerics;

namespace dotNetFractal.Logic;

public class FractalArea<T> : IFractalArea where T : INumber<T>
{
    private readonly IDisplayArea m_area;
    private readonly FractalPixels m_pixels;

    public bool JuliaSet { get; set; } = false;

    public FractalPixels Pixels => m_pixels;

    public IDisplayArea DisplayArea => m_area;

    public FractalArea(IDisplayArea area)
    {
        m_area = area;
        m_pixels = new FractalPixels(area.PixelsHorizontal, area.PixelsVertical);
    }

    public IFractalPixel? GetPixel(int i, int j)
    {
        return m_pixels.GetPixel(i, j);
    }
}
