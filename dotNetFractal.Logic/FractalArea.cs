
using System.IO;

namespace dotNetFractal.Logic
{
    public class FractalArea<T> : IFractalArea where T : IFractalUnit<T>, new()
    {
        private IDisplayArea m_area;
        private FractalPixels m_pixels;

        public FractalPixels Pixels => m_pixels;

        private FractalArea()
        {
            // for serialization only
        }

        public FractalArea(IDisplayArea area)
        {
            m_area = area;
            m_pixels = new FractalPixels(area.PixelsHorizontal, area.PixelsVertical);
        }

        public IDisplayArea DisplayArea => m_area;

        public IFractalPixel GetPixel(int i, int j)
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

        public static IFractalArea Read(string path)
        {
            if (!File.Exists(path))
                return default;

            var retval = new FractalArea<T>();
            var br = new BinaryReader(File.Open(path, FileMode.Open));
            retval.Read(br);
            br.Close();
            return retval;
        }

        public void Write(BinaryWriter bw)
        {
            m_area.Write(bw);

            var width = m_area.PixelsHorizontal;
            var height = m_area.PixelsVertical;

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
            m_area = DisplayArea<T>.Read(br);

            var width = m_area.PixelsHorizontal;
            var height = m_area.PixelsVertical;

            m_pixels = new FractalPixels(width, height);

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
