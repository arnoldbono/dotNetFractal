
namespace dotNetFractal.Logic
{
    public interface IFractalArea
    {
        IDisplayArea DisplayArea { get; }

        FractalPixels Pixels { get; }

        IFractalPixel GetPixel(int i, int j);

        bool JuliaSet { get; set; }
    }
}
