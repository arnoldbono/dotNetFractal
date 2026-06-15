using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// A FractalCachedImage is a square image with Size pixels horizontally and vertically.
    /// The zoom level is n, meaning that it the original image has been subdivided (n-1) times.
    /// There are 2^(2 * (n-1)) FractalCachedImage at zoom level is n.
    /// </summary>
    public class FractalCachedImage : IDisposable
    {
        private uint m_zoomLevel = 0;
        private UInt64 m_indexI = 0;
        private UInt64 m_indexJ = 0;
        private Image m_image = null;

        /// <summary>
        /// The horizontal index.
        /// PRE: (value < ((UInt64)2 << (iZoomLevel - 1)))
        /// </summary>
        public UInt64 IndexI
        {
            get { return m_indexI; }
            set { m_indexI = value; }
        }

        /// <summary>
        /// The vertical index.
        /// PRE: (value < ((UInt64)2 << (iZoomLevel - 1)))
        /// </summary>
        public UInt64 IndexJ
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

        public Image Image
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

        public FractalCachedImage(UInt64 indexI, UInt64 indexJ, int size, uint zoomLevel)
        {
            m_indexI = indexI;
            m_indexJ = indexJ;
            Size = size;
            m_zoomLevel = zoomLevel;
            m_image = new Bitmap(Size, Size, PixelFormat.Format32bppArgb);
        }

        public FractalCachedImage(string folder, uint zoomLevel, UInt64 indexI, UInt64 indexJ, int size)
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
            m_image.Save(Path.Combine(folder, FileName), ImageFormat.Png);
        }

        public Image Load(string folder)
        {
            return Bitmap.FromFile(Path.Combine(folder, FileName));
        }

        public void Dispose()
        {
            m_image?.Dispose();
            m_image = null;

            GC.SuppressFinalize(this);
        }
    }
}
