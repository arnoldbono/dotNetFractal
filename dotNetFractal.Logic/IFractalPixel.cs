
namespace dotNetFractal.Logic
{
    public interface IFractalPixel
    {
        int Iteration { get; }

        double GetEscapeFraction(double maxRadius);
    }
}
