using System;
using System.Numerics;
using SkiaSharp;

namespace dotNetFractal.Logic;

/// <summary>
/// Compute a Fractal from left to right.
/// </summary>
abstract public class Fractal<T> : Worker, IFractal where T : INumber<T>, new()
{
    private readonly FractalColorMap m_colorMap = FractalColorMap.GetInstance();

    private readonly FractalSettings m_settings;
    private FractalAreaPatch? m_areaPatch = null;
    private double m_maxRadius = 4.0;
    protected ComputationState m_state = ComputationState.NotStarted;

    public IFractalArea Area => m_settings.FractalArea;

    public FractalAreaPatch AreaPatch
    {
        get { return m_areaPatch ?? throw new InvalidOperationException("AreaPatch is not set."); }
        set
        {
            if (m_areaPatch != null)
                base.StopThread();

            m_areaPatch = value;
        }
    }

    public double MaxRadius
    {
        get { return m_maxRadius; }
        set { m_maxRadius = value; }
    }

    public FractalSettings Settings => m_settings;

    public ComputationState State => m_state;

    public Fractal(FractalSettings settings)
    {
        m_settings = settings;
    }

    public SKColor ComputeColor(IFractalPixel pixel)
    {
        var iteration = pixel.Iteration;
        if (iteration >= Settings.MaxIterations)
            return SKColors.Black;

        GetColor(iteration, out var red, out var green, out var blue);

        if (iteration != 0 && Settings.SmoothColoring)
        {
            var fraction = pixel.GetEscapeFraction((double)MaxRadius);
            System.Diagnostics.Debug.Assert(fraction < 1.0);

            GetColor(iteration - 1, out var red1, out var green1, out var blue1);

            red = (int)((double)red1 + fraction * (red - red1));
            green = (int)((double)green1 + fraction * (green - green1));
            blue = (int)((double)blue1 + fraction * (blue - blue1));
        }

        return new SKColor((byte)red, (byte)green, (byte)blue);
    }

    public void GetColor(int index, out int red, out int green, out int blue)
    {
        if (index < Settings.FirstColorStep)
        {
            red = 255;
            green = 255;
            blue = 255;
            return;
        }

        index -= Settings.FirstColorStep;

        var fraction = (index % Settings.MaxColorSteps) / (double)Settings.MaxColorSteps;
        var color = m_colorMap.GetColor(fraction);
        red = color.Red;
        green = color.Green;
        blue = color.Blue;
    }

    public override void StartThread(Action<Action> threadPoolExecutor = null)
    {
        base.StartThread(threadPoolExecutor);
    }

    protected abstract FractalPixel<T> Compute(T maxRadius, int maxIterations, DisplayArea<T> displayArea, int i, int j);

    private void IncrementDistributionGraph(int iteration)
    {
        --iteration; // 1...maxIterations, but DistributionGraph is 0...maxIterations-1, so decrement by 1
        if (Settings.DistributionGraph != null)
        {
            if (iteration < 0 || iteration >= Settings.DistributionGraph.Length)
                throw new ArgumentOutOfRangeException(nameof(iteration));
            System.Threading.Interlocked.Increment(ref Settings.DistributionGraph[iteration]);
        }
    }

    private void UpdatePixel(int i, int j, IFractalPixel pixel)
    {
        Area.Pixels.SetPixel(i, j, pixel);
        IncrementDistributionGraph(pixel.Iteration);
    }

    protected override void ThreadProc()
    {
        if (AreaPatch == null)
            throw new InvalidOperationException("AreaPatch is not set.");

        Stop = false;
        Stopped = false;

        var pixels = Area.Pixels;

        var startIndexWidth = AreaPatch.StartIndexWidth;
        var stopIndexWidth = Math.Min(AreaPatch.StopIndexWidth, pixels.Width);
        var startIndexHeight = AreaPatch.StartIndexHeight;
        var stopIndexHeight = Math.Min(AreaPatch.StopIndexHeight, pixels.Height);

        var displayArea = (DisplayArea<T>)Area.DisplayArea;
        var maxRadius = T.CreateChecked(MaxRadius);
        var maxIterations = Settings.MaxIterations;

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

        UpdateAreaPatchFractalImage();

        Stopped = true;
    }

    protected void UpdateAreaPatchFractalImage()
    {
        var fractalArea = Area;
        var areaPatch = AreaPatch ?? throw new InvalidOperationException("AreaPatch is not set.");

        var fractalImage = areaPatch.FractalImage;
        var image = fractalImage.Image;
        var size = fractalImage.Size;

        for (var i = 0; i < size && !Stop; ++i)
        {
            for (var j = 0; j < size && !Stop; ++j)
            {
                var pixel = fractalArea.GetPixel(areaPatch.StartIndexWidth + i, areaPatch.StartIndexHeight + j);
                if (pixel != null)
                {
                    image.SetPixel(i, j, ComputeColor(pixel));
                }
            }
        }
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
            var fractal = FractalFactory.CreateFractal(m_settings);
            fractal.AreaPatch = new FractalAreaPatch(startLocationX, startLocationY, patchSize);
            fractals[i++] = fractal;
        }
        return fractals;
    }
}
