using System;
using System.Numerics;

namespace dotNetFractal.Logic;

/// <summary>
/// Compute a fractal from left to right, top to bottom, pixel by pixel, for the given area and settings, and store the result in a pixel array.
/// </summary>
abstract public class Fractal<T> : Worker, IFractal where T : INumber<T>, new()
{
    private readonly FractalSettings m_settings;
    private readonly FractalColorist m_colorist;
    private readonly FractalAreaPatch m_areaPatch;
    protected ComputationState m_state = ComputationState.NotStarted;

    public FractalAreaPatch AreaPatch => m_areaPatch;

    public ComputationState State => m_state;
    
    public IFractalColorist Colorist => m_colorist;
    
    public Fractal(FractalSettings settings, FractalAreaPatch areaPatch)
    {
        m_settings = settings;
        m_colorist = new FractalColorist(m_settings);
        m_areaPatch = areaPatch;
    }

    public override void StartThread(Action<Action>? threadPoolExecutor)
    {
        base.StartThread(threadPoolExecutor);
    }

    protected abstract FractalPixel<T> Compute(T maxRadius, int maxIterations, DisplayArea<T> displayArea, int i, int j);

    private void IncrementDistributionGraph(int iteration)
    {
        --iteration; // 1...maxIterations, but DistributionGraph is 0...maxIterations-1, so decrement by 1
        if (m_settings.DistributionGraph != null)
        {
            if (iteration < 0 || iteration >= m_settings.DistributionGraph.Length)
                throw new ArgumentOutOfRangeException(nameof(iteration));
            System.Threading.Interlocked.Increment(ref m_settings.DistributionGraph[iteration]);
        }
    }

    private void UpdatePixel(int i, int j, IFractalPixel pixel)
    {
        m_settings.FractalArea.Pixels.SetPixel(i, j, pixel);
        IncrementDistributionGraph(pixel.Iteration);
    }

    protected override void ThreadProc()
    {
        if (AreaPatch == null)
            throw new InvalidOperationException("AreaPatch is not set.");

        Stop = false;
        Stopped = false;

        var pixels = m_settings.FractalArea.Pixels;

        var startIndexWidth = AreaPatch.StartIndexWidth;
        var stopIndexWidth = Math.Min(AreaPatch.StopIndexWidth, pixels.Width);
        var startIndexHeight = AreaPatch.StartIndexHeight;
        var stopIndexHeight = Math.Min(AreaPatch.StopIndexHeight, pixels.Height);

        var displayArea = (DisplayArea<T>)m_settings.FractalArea.DisplayArea;
        var maxRadius = T.CreateChecked(FractalSettings.MaxRadius);
        var maxIterations = m_settings.MaxIterations;

        bool allMaxIteractionReached = true;
        bool someMaxIteractionReached = false;
        m_state = ComputationState.Running;
        for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
        {
            for (var j = startIndexHeight; j < stopIndexHeight && !Stop; ++j)
            {
                if (i > startIndexWidth && i < stopIndexWidth - 1 && j > startIndexHeight && j < stopIndexHeight - 1)
                {
                    continue;
                }

                var pixel = Compute(maxRadius, maxIterations, displayArea, i, j);
                UpdatePixel(i, j, pixel);

                if (pixel.Iteration < maxIterations)
                {
                    allMaxIteractionReached = false;
                }
                else
                {
                    someMaxIteractionReached = true;
                }
            }
        }

        ++startIndexWidth;
        --stopIndexWidth;
        ++startIndexHeight;
        --stopIndexHeight;

        if (allMaxIteractionReached)
        {
            for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
            {
                for (var j = startIndexHeight; j < stopIndexHeight && !Stop; ++j)
                {
                    UpdatePixel(i, j, new FractalPixel<T>(maxIterations, maxRadius, maxRadius));
                }
            }
            m_state = ComputationState.AllMaxIterationsReached;
        }
        else
        {
            var state = someMaxIteractionReached && AreaPatch.Size > 16 ?
                ComputationState.SomeMaxIterationsReached :
                ComputationState.NoneMaxIterationsReached;
            if (state == ComputationState.NoneMaxIterationsReached)
            {
                for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
                {
                    for (var j = startIndexHeight; j < stopIndexHeight && !Stop; ++j)
                    {
                        var pixel = Compute(maxRadius, maxIterations, displayArea, i, j);
                        UpdatePixel(i, j, pixel);
                    }
                }
            } // else fractal gets subdivided later on and skip the inner pixels for now
            m_state = state;
        }

        m_colorist.UpdateAreaPatch(m_settings.FractalArea, AreaPatch);

        Stopped = true;
    }

    public IFractal[] Subdivide()
    {
        var areaPatch = AreaPatch ?? throw new InvalidOperationException("AreaPatch is not set.");

        var patchSize = areaPatch.Size / 2;
        var fractals = new IFractal[4];

        (int startLocationX, int startLocationY)[] sizes = 
        [
            (areaPatch.StartIndexWidth, areaPatch.StartIndexHeight),
            (areaPatch.StartIndexWidth + patchSize, areaPatch.StartIndexHeight),
            (areaPatch.StartIndexWidth, areaPatch.StartIndexHeight + patchSize),
            (areaPatch.StartIndexWidth + patchSize, areaPatch.StartIndexHeight + patchSize)
        ];

        int i = 0;
        foreach (var (startLocationX, startLocationY) in sizes)
        {
            var subAreaPatch = new FractalAreaPatch(startLocationX, startLocationY, patchSize);
            var fractal = FractalFactory.CreateFractal(m_settings, subAreaPatch);
            fractals[i++] = fractal;
        }
        return fractals;
    }
}
