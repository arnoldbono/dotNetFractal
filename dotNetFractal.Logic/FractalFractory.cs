
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
        IFractal fractal;
        if (fractalSettings.JuliaSet)
        {
            var juliaSet = new FractalJulia<T>
            {
                MaxIterations = fractalSettings.MaxIterations,
                MaxColors = fractalSettings.MaxColorSteps,
                SmoothColoring = fractalSettings.SmoothColoring
            };
            var displayArea = fractalSettings.FractalArea.DisplayArea as DisplayArea<T> ?? throw new InvalidOperationException("Unsupported display area type.");
            juliaSet.SetStartingPoint(displayArea.CenterX, displayArea.CenterY);
            fractal = juliaSet;
        }
        else
        {
            fractal = new FractalMandelbrot<T>
            {
                MaxIterations = fractalSettings.MaxIterations,
                MaxColors = fractalSettings.MaxColorSteps,
                SmoothColoring = fractalSettings.SmoothColoring
            };
        }
        fractal.Area = fractalSettings.FractalArea;
        // TODO fractal.Settings = fractalSettings;
        return fractal;
    }
}
