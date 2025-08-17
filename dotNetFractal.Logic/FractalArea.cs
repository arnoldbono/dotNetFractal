using System;
using System.Diagnostics;
using System.IO;

namespace dotNetFractal
{
    public class FractalArea
    {
        private int m_refI;
        private int m_refJ;
        private FractalPixels m_pixels;

        public double RefX { get; set; }

        public double RefY { get; set; }

        public double Length { get; set; }

        public string FileName { get; set; }

        public FractalPixels Pixels => m_pixels;

        private FractalArea()
        {
            // for serialization only
        }

        public FractalArea(double x, double y, double length, int width, int height)
        {
            RefX = x;
            RefY = y;
            Length = length;
            m_pixels = new FractalPixels(width, height);
            m_refI = m_pixels.Width / 2;
            m_refJ = m_pixels.Height / 2;
        }

        public double GetX(int i)
        {
            return RefX + (double)(i - m_refI) / (double)m_refI * (Length / 2.0);
        }

        public double GetY(int j)
        {
            return RefY + (double)(m_refJ - j) / (double)m_refJ * (Length / 2.0);
        }

        public FractalPixel GetPixel(double x, double y)
        {
            double l = Length / 2.0;
            x = (double)m_refI * (1.0 + (x - RefX) / l);
            y = (double)m_refJ * (1.0 + (RefY - y) / l);
            return m_pixels.GetPixel((int)Math.Floor(x), (int)Math.Floor(y));
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
            bw.Write(RefX);
            bw.Write(RefY);
            bw.Write(Length);
            bw.Write(m_refI);
            bw.Write(m_refJ);
            var width = m_pixels.Width;
            var height = m_pixels.Height;
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
            RefX = br.ReadDouble();
            RefY = br.ReadDouble();
            Length = br.ReadDouble();
            m_refI = br.ReadInt32();
            m_refJ = br.ReadInt32();
            var width = br.ReadInt32();
            var height = br.ReadInt32();
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
