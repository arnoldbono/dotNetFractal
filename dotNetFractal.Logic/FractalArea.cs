using System;
using System.IO;

namespace dotNetFractal.Logic
{
    public class FractalArea<T> where T : IFractalUnit<T>, new()
    {
        private DisplayArea<T> m_area;
        private FractalPixels<T> m_pixels;

        public string FileName { get; set; }

        public FractalPixels<T> Pixels => m_pixels;

        private FractalArea()
        {
            // for serialization only
        }

        public FractalArea(DisplayArea<T> area)
        {
            m_area = area;
            m_pixels = new FractalPixels<T>(area.PixelsHorizontal, area.PixelsVertical);
        }

        public DisplayArea<T> DisplayArea => m_area;

        public FractalPixel<T> GetPixel(int i, int j)
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

        public static FractalArea<T> Read(string path)
        {
            FractalArea<T> retval = null;
            if (File.Exists(path))
            {
                retval = new FractalArea<T>();
                var br = new BinaryReader(File.Open(path, FileMode.Open));
                retval.Read(br);
                br.Close();
            }
            return retval;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((decimal)m_area.CenterX);
            bw.Write((decimal)m_area.CenterY);
            bw.Write((decimal)m_area.Width);
            bw.Write((decimal)m_area.Height);

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
            var refX = (T)br.ReadDecimal();
            var refY = (T)br.ReadDecimal();
            var areaWidth = (T)br.ReadDecimal();
            var areaHeight = (T)br.ReadDecimal();
            var width = br.ReadInt32();
            var height = br.ReadInt32();

            m_area = new DisplayArea<T>(refX, refY, areaWidth, areaHeight, width, height);
            m_pixels = new FractalPixels<T>(width, height);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    m_pixels.SetPixel(i, j, FractalPixel<T>.Read(br));
                }
            }
        }
    }
}
