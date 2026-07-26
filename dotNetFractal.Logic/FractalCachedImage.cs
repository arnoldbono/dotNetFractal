using System;
using System.Diagnostics;
using SkiaSharp;

namespace dotNetFractal.Logic;

/// <summary>
/// A FractalCachedImage is a square image with Size pixels horizontally and vertically.
/// The zoom level is n, meaning that it the original image has been subdivided (n-1) times.
/// There are 2^(2 * (n-1)) FractalCachedImage at zoom level is n.
/// </summary>
public class FractalCachedImage : IDisposable
{
    private uint m_zoomLevel = 0;
    private int m_indexI = 0;
    private int m_indexJ = 0;
    private SKBitmap? m_image = null;

    /// <summary>
    /// The horizontal index.
    /// PRE: (value < ((int)2 << (iZoomLevel - 1)))
    /// </summary>
    public int IndexI
    {
        get { return m_indexI; }
        set { m_indexI = value; }
    }

    /// <summary>
    /// The vertical index.
    /// PRE: (value < ((int)2 << (iZoomLevel - 1)))
    /// </summary>
    public int IndexJ
    {
        get { return m_indexJ; }
        set { m_indexJ = value; }
    }

    /// <summary>
    /// The zoom level is n >= 1, meaning that it the original image has been subdivided (n-1) times.
    /// </summary>
    public uint ZoomLevel
    {
        get { return m_zoomLevel; }
        set { m_zoomLevel = value; }
    }

    public int Size { get; }

    public SKBitmap? Image
    {
        get { return m_image; }
    }

    public string FileName
    {
        get { return m_zoomLevel.ToString() + "_" + m_indexI.ToString() + "_" + m_indexJ.ToString() + ".fci"; }
    }

    public FractalCachedImage(int size) : this(0, 0, size, 0)
    {
    }

    public FractalCachedImage(int indexI, int indexJ, int size, uint zoomLevel)
    {
        m_indexI = indexI;
        m_indexJ = indexJ;
        Size = size;
        m_zoomLevel = zoomLevel;
        m_image = new SKBitmap(Size, Size, SKColorType.Bgra8888, SKAlphaType.Premul);
    }

    public FractalCachedImage(string folder, uint zoomLevel, int indexI, int indexJ, int size)
    {
        m_zoomLevel = zoomLevel;
        m_indexI = indexI;
        m_indexJ = indexJ;
        Size = size;
        m_image = Load(folder);
    }

    public void Save(string folder)
    {
        Debug.Assert(m_image != null);
        var path = Path.Combine(folder, FileName);
        using var image = SKImage.FromBitmap(m_image);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(path);
        data.SaveTo(stream);
    }

    public SKBitmap Load(string folder)
    {
        var path = Path.Combine(folder, FileName);
        using var stream = File.OpenRead(path);
        return SKBitmap.Decode(stream);
    }

    public void Dispose()
    {
        m_image?.Dispose();
        m_image = null;

        GC.SuppressFinalize(this);
    }
}
