using System;
using SkiaSharp;

namespace dotNetFractal.Logic;

public class FractalAreaPatch : IDisposable
{
    public int StartIndexWidth => FractalImage.IndexI;

    public int StopIndexWidth => StartIndexWidth + Size;

    public int Size => FractalImage.Size;

    public int StartIndexHeight => FractalImage.IndexJ;

    public int StopIndexHeight => StartIndexHeight + Size;

    public FractalCachedImage FractalImage { get; private set; }

    public FractalAreaPatch(int startIndexWidth, int startIndexHeight, int size)
    {
        FractalImage = new FractalCachedImage(startIndexWidth, startIndexHeight, size, 0);
    }

    public SKRectI GetTargetRectangle(int width, int height)
    {
        var x = StartIndexWidth;
        var y = StartIndexHeight;
        var rectWidth = Math.Min(Size, width - x);
        var rectHeight = Math.Min(Size, height - y);
        return new SKRectI(x, y, x + rectWidth, y + rectHeight);
    }

    public SKRectI GetSourceRectangle(int width, int height)
    {
        var x = StartIndexWidth;
        var y = StartIndexHeight;
        var rectWidth = Math.Min(Size, width - x);
        var rectHeight = Math.Min(Size, height - y);
        return new SKRectI(0, 0, rectWidth, rectHeight);
    }

    public void Dispose()
    {
        FractalImage.Dispose();

        GC.SuppressFinalize(this);
    }
}
