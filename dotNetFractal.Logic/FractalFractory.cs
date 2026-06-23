
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
            return new FractalJuliaSet<T>(fractalSettings);
        }
        else
        {
            return new FractalMandelbrot<T>(fractalSettings);
        }
    }
}
