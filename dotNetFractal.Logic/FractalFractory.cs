
using System.Numerics;

namespace dotNetFractal.Logic;

public static class FractalFactory
{
    public static IFractal CreateFractal(FractalSettings fractalSettings, FractalAreaPatch areaPatch)
    {
        if (fractalSettings.HighPrecision)
            return CreateFractal<decimal>(fractalSettings, areaPatch);
        return CreateFractal<double>(fractalSettings, areaPatch);
    }

    private static IFractal CreateFractal<T>(FractalSettings fractalSettings, FractalAreaPatch areaPatch) where T : INumber<T>, new()
    {
        if (fractalSettings.FractalArea.JuliaSet)
            return new FractalJuliaSet<T>(fractalSettings, areaPatch);
        return new FractalMandelbrot<T>(fractalSettings, areaPatch);
    }
}
