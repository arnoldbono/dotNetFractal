
using System.Numerics;

namespace dotNetFractal.Logic;

public static class FractalFactory
{
    public static IFractal CreateFractal(FractalSettings fractalSettings)
    {
        if (fractalSettings.HighPrecision)
            return CreateFractal<decimal>(fractalSettings);
        return CreateFractal<double>(fractalSettings);
    }

    private static IFractal CreateFractal<T>(FractalSettings fractalSettings) where T : INumber<T>, new()
    {
        if (fractalSettings.FractalArea.JuliaSet)
            return new FractalJuliaSet<T>(fractalSettings);
        return new FractalMandelbrot<T>(fractalSettings);
    }
}
