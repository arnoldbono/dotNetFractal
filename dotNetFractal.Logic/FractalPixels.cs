using System;
using System.Diagnostics;

namespace dotNetFractal.Logic
{
    public class FractalPixels
    {
        private readonly FractalPixel[,] m_pixels;

        public int Width { get; set; }

        public int Height { get; set; }

        public FractalPixels(int width, int height)
        {
            Width = width;
            Height = height;
            m_pixels = new FractalPixel[Width, Height];
        }

        public bool Inside(int i, int j)
        {
            return i >= 0 && i < Width && j >= 0 && j < Height;
        }

        public FractalPixel GetPixel(int i, int j)
        {
            return Inside(i, j) ? m_pixels[i, j] : null;
        }

        public void SetPixel(int i, int j, FractalPixel pixel)
        {
            if (i >= 0 && i < Width && j >= 0 && j < Height)
            {
                m_pixels[i, j] = pixel;
            }
            else
            {
                Debug.Assert(false);
            }
        }

    }
}
