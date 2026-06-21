
using System.Diagnostics;

namespace dotNetFractal.Logic
{
    public class FractalSettings
    {
        private readonly IFractalArea m_fractalArea;

        public IFractalArea FractalArea => m_fractalArea;

        public int MaxIterations { get; private set; }

        public int MaxColorSteps { get; private set; }

        public bool SmoothColoring { get; private set; }

        public bool HighPrecision { get; private set; }

        public bool JuliaSet { get; private set; }

        public FractalSettings(IDisplayArea displayArea, int maxIterations, int maxColorSteps, bool smoothColoring, bool highPrecision, bool juliaSet)
        {
            Debug.Assert(displayArea != null);

            // Here is where upgrade the from FractalDouble to FractalDecimal, when 'highPrecision' is set.
            var displayAreaConverted = DisplayAreaFactory.Convert(displayArea, highPrecision);
            m_fractalArea = DisplayAreaFactory.CreateFractalArea(displayAreaConverted);

            MaxIterations = maxIterations;
            MaxColorSteps = maxColorSteps;
            SmoothColoring = smoothColoring;
            HighPrecision = highPrecision;
            JuliaSet = juliaSet;
        }
    }
}
