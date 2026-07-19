
using System.Diagnostics;

namespace dotNetFractal.Logic;

public class FractalSettings
{
    private readonly IFractalArea m_fractalArea;

    public IFractalArea FractalArea => m_fractalArea;

    public int MaxIterations { get; private set; }

    public int MaxColorSteps { get; private set; }

    public int FirstColorStep { get; private set; }

    public bool SmoothColoring { get; private set; }

    public bool HighPrecision { get; private set; }

    public int[] DistributionGraph { get; private set; }

    public FractalSettings(IDisplayArea displayArea, int maxIterations, int maxColorSteps, int firstColorStep, bool smoothColoring, bool highPrecision, int[]? distributionGraph = null)
    {
        Debug.Assert(displayArea != null);

        // Here is where upgrade the from double to decimal, when 'highPrecision' is set.
        var displayAreaConverted = DisplayAreaFactory.Convert(displayArea, highPrecision) ?? throw new InvalidOperationException("Failed to convert display area.");
        m_fractalArea = DisplayAreaFactory.CreateFractalArea(displayAreaConverted) ?? throw new InvalidOperationException("Failed to create fractal area.");

        MaxIterations = maxIterations;
        MaxColorSteps = maxColorSteps;
        FirstColorStep = firstColorStep;
        SmoothColoring = smoothColoring;
        HighPrecision = highPrecision;
        DistributionGraph = distributionGraph ?? [];
    }
}
