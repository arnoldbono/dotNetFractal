
using System;

namespace dotNetFractal.Logic;

public static class FractalFactory
{
    public static IFractal CreateFractal(FractalSettings fractalSettings)
    {
        if (fractalSettings.HighPrecision)
        {
            return CreateFractal<FractalDecimal>(fractalSettings);
        }
        else
        {
            return CreateFractal<FractalDouble>(fractalSettings);
        }
    }

    private static IFractal CreateFractal<T>(FractalSettings fractalSettings) where T : IFractalUnit<T>, new()
    {
        if (fractalSettings.FractalArea.JuliaSet)
        {
            var juliaSet = new FractalJulia<T>(fractalSettings);
            var fractalArea = fractalSettings.FractalArea as FractalArea<T> ?? throw new InvalidOperationException("Unsupported fractal area type.");
            juliaSet.SetStartingPoint(fractalArea.JuliaSetX, fractalArea.JuliaSetY);
            return juliaSet;
        }
        else
        {
            return new FractalMandelbrot<T>(fractalSettings);
        }
    }
}
