using System;
using dotNetFractal.Cuda;

namespace dotNetFractal.Logic;

public class FractalMandelbrotCuda(FractalSettings settings, FractalAreaPatch areaPatch) : Fractal<double>(settings, areaPatch)
{
    protected override FractalPixel<double> Compute(double maxRadius, int maxIterations, DisplayArea<double> displayArea, int i, int j)
    {
        var Cx = displayArea.GetX(i);
        var Cy = displayArea.GetY(j);
        var x = Cx;
        var y = Cy;

        int iteration = 0;
        var radius2 = 0.0;
        var prevRadius2 = 0.0;
        do
        {
            ++iteration;
            prevRadius2 = radius2;

            var xx = x * x;
            var yy = y * y;

            if ((radius2 = xx + yy) > maxRadius)
            {
                break;
            }

            y *= x;
            y += y + Cy;
            x = xx - yy + Cx;
        }
        while (iteration < maxIterations);

        return new FractalPixel<double>(iteration, radius2, prevRadius2);
    }

    protected override void ThreadProc()
    {
        var areaPatch = AreaPatch ?? throw new InvalidOperationException("AreaPatch is not set.");

        Stop = false;
        Stopped = false;

        var pixels = Settings.FractalArea.Pixels;

        var startIndexWidth = areaPatch.StartIndexWidth;
        var stopIndexWidth = Math.Min(areaPatch.StopIndexWidth, pixels.Width);
        var startIndexHeight = areaPatch.StartIndexHeight;
        var stopIndexHeight = Math.Min(areaPatch.StopIndexHeight, pixels.Height);

        var displayArea = (DisplayArea<double>)Settings.FractalArea.DisplayArea;
        var maxRadius = FractalSettings.MaxRadius;
        var maxIterations = Settings.MaxIterations;

        int tileWidth = stopIndexWidth - startIndexWidth;
        int tileHeight = stopIndexHeight - startIndexHeight;
        if (tileWidth <= 0 || tileHeight <= 0)
        {
            Stopped = true;
            return;
        }

        int elementCount = tileWidth * tileHeight;
        int[] borderIterations = new int[elementCount];
        double[] borderRadius2 = new double[elementCount];
        double[] borderPrevRadius2 = new double[elementCount];

        int cudaResult = CudaMandelbrotNative.TryComputeMandelbrotTile(
            startIndexWidth,
            startIndexHeight,
            tileWidth,
            tileHeight,
            displayArea.CenterX,
            displayArea.CenterY,
            displayArea.Width,
            displayArea.Height,
            displayArea.PixelsHorizontal,
            displayArea.PixelsVertical,
            maxIterations,
            maxRadius,
            borderIterations,
            borderRadius2,
            borderPrevRadius2);

        if (cudaResult != 0)
        {
            base.ThreadProc();
            return;
        }

        m_state = ComputationState.Running;

        for (var i = startIndexWidth; i < stopIndexWidth && !Stop; ++i)
        {
            for (var j = startIndexHeight; j < stopIndexHeight && !Stop; ++j)
            {
                int idx = GetIndex(i, j, startIndexWidth, startIndexHeight, tileWidth);
                int iteration = borderIterations[idx];
                var pixel = new FractalPixel<double>(iteration, borderRadius2[idx], borderPrevRadius2[idx]);
                UpdatePixel(i, j, pixel);
            }
        }

        m_state = ComputationState.NoneMaxIterationsReached; // Fake, but works

        UpdateColorsForAreaPatch();
        Stopped = true;
    }

    private static int GetIndex(int i, int j, int startI, int startJ, int tileWidth)
    {
        return (j - startJ) * tileWidth + (i - startI);
    }
}
