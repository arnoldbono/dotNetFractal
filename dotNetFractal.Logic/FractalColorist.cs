using SkiaSharp;

namespace dotNetFractal.Logic;

internal sealed class FractalColorist : IFractalColorist
{
    private readonly FractalColorMap m_colorMap;
    private readonly FractalSettings m_settings;

    public FractalColorist(FractalSettings settings)
    {
        m_colorMap = FractalColorMap.GetInstance();
        m_settings = settings;
    }

    public void UpdateAreaPatch(IFractalArea area, FractalAreaPatch areaPatch)
    {
        var fractalImage = areaPatch.FractalImage;
        var image = fractalImage.Image ?? throw new InvalidOperationException("FractalImage is not set.");
        var size = fractalImage.Size;

        using var canvas = new SKCanvas(image); // We get more performance by only getting the canvas once other than for each image.SetPixel() call
        for (var i = 0; i < size; ++i)
        {
            for (var j = 0; j < size; ++j)
            {
                var pixel = area.GetPixel(areaPatch.StartIndexWidth + i, areaPatch.StartIndexHeight + j);
                if (pixel == null)
                    continue;

                var color = GetColor(pixel);
                canvas.DrawPoint(i, j, color);
            }
        }
    }

    public void Update(IFractal fractal)
    {
        UpdateAreaPatch(m_settings.FractalArea, fractal.AreaPatch);
    }

    private SKColor GetColor(IFractalPixel pixel)
    {
        var iteration = pixel.Iteration;
        if (iteration >= m_settings.MaxIterations)
            return SKColors.Black;

        GetColor(iteration, out var red, out var green, out var blue);

        if (iteration != 0 && m_settings.SmoothColoring)
        {
            var fraction = pixel.GetEscapeFraction(FractalSettings.MaxRadius);
            System.Diagnostics.Debug.Assert(fraction < 1.0);

            GetColor(iteration - 1, out var red1, out var green1, out var blue1);

            red = (int)((double)red1 + fraction * (red - red1));
            green = (int)((double)green1 + fraction * (green - green1));
            blue = (int)((double)blue1 + fraction * (blue - blue1));
        }

        return new SKColor((byte)red, (byte)green, (byte)blue);
    }

    private void GetColor(int index, out int red, out int green, out int blue)
    {
        if (index < m_settings.FirstColorStep)
        {
            red = 255;
            green = 255;
            blue = 255;
            return;
        }

        index -= m_settings.FirstColorStep;

        var fraction = (index % m_settings.MaxColorSteps) / (double)m_settings.MaxColorSteps;
        var color = m_colorMap.GetColor(fraction);
        red = color.Red;
        green = color.Green;
        blue = color.Blue;
    }

}
