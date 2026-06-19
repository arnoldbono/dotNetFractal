using System;
using System.IO;

namespace dotNetFractal.Logic
{
    public class FractalArea
    {
        private DisplayArea m_area;
        private FractalPixels m_pixels;

        public string FileName { get; set; }

        public FractalPixels Pixels => m_pixels;

        private FractalArea()
        {
            // for serialization only
        }

        public FractalArea(DisplayArea area)
        {
            m_area = area;
            m_pixels = new FractalPixels(area.PixelsHorizontal, area.PixelsVertical);
        }

        public DisplayArea DisplayArea => m_area;

        public FractalPixel GetPixel(int i, int j)
        {
            return m_pixels.GetPixel(i, j);
        }

        public void Write(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var bw = new BinaryWriter(File.Open(path, FileMode.CreateNew));
            Write(bw);
            bw.Close();
        }

        public static FractalArea Read(string path)
        {
            FractalArea retval = null;
            if (File.Exists(path))
            {
                retval = new FractalArea();
                var br = new BinaryReader(File.Open(path, FileMode.Open));
                retval.Read(br);
                br.Close();
            }
            return retval;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(m_area.CenterX);
            bw.Write(m_area.CenterY);
            bw.Write(m_area.Width);
            bw.Write(m_area.Height);

            var width = m_area.PixelsHorizontal;
            var height = m_area.PixelsVertical;

            bw.Write(width);
            bw.Write(height);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    m_pixels.GetPixel(i, j).Write(bw);
                }
            }
        }

        public void Read(BinaryReader br)
        {
            var refX = br.ReadDecimal();
            var refY = br.ReadDecimal();
            var areaWidth = br.ReadDecimal();
            var areaHeight = br.ReadDecimal();
            var width = br.ReadInt32();
            var height = br.ReadInt32();

            m_area = new DisplayArea(refX, refY, areaWidth, areaHeight, width, height);
            m_pixels = new FractalPixels(width, height);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    m_pixels.SetPixel(i, j, FractalPixel.Read(br));
                }
            }
        }
    }
}
