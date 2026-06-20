using System;
using System.Drawing;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Compute a Fractal from left to right.
    /// </summary>
    abstract public class Fractal<T> : Worker, IFractal where T : IFractalUnit<T>, new()
    {
        private readonly FractalColorMap m_colorMap = FractalColorMap.GetInstance();

        private IFractalArea m_area = null;
        private FractalAreaPatch m_areaPatch = null;
        private double m_maxRadius = 4.0;
        private int m_maxIterations = 256;
        private int m_maxColors = 16;
        private bool m_smoothColoring = true;

        public IFractalArea Area
        {
            get { return m_area; }
            set
            {
                if (m_area != null)
                    base.StopThread();

                m_area = value;
            }
        }

        public FractalAreaPatch AreaPatch
        {
            get { return m_areaPatch; }
            set
            {
                if (m_areaPatch != null)
                    base.StopThread();

                m_areaPatch = value;
            }
        }

        public double MaxRadius
        {
            get { return m_maxRadius; }
            set { m_maxRadius = value; }
        }

        public int MaxIterations
        {
            get { return m_maxIterations; }
            set { m_maxIterations = value; }
        }

        public int MaxColors
        {
            get { return m_maxColors; }
            set { m_maxColors = value; }
        }

        public bool SmoothColoring
        {
            get { return m_smoothColoring; }
            set { m_smoothColoring = value; }
        }

        public Fractal()
        {
        }

        public Color ComputeColor(IFractalPixel pixel)
        {
            var iteration = pixel.Iteration;
            if (iteration >= MaxIterations)
                return Color.Black;

            GetColor(iteration, out var red, out var green, out var blue);

            if (iteration != 0 && SmoothColoring)
            {
                var fraction = pixel.GetEscapeFraction((double)MaxRadius);
                System.Diagnostics.Debug.Assert(fraction < 1.0);

                GetColor(iteration - 1, out var red1, out var green1, out var blue1);

                red = (int)((double)red1 + fraction * (red - red1));
                green = (int)((double)green1 + fraction * (green - green1));
                blue = (int)((double)blue1 + fraction * (blue - blue1));
            }

            return Color.FromArgb(red, green, blue);
        }

        public void GetColor(int index, out int red, out int green, out int blue)
        {
            var fraction = (index % MaxColors) / (double)MaxColors;
            var color = m_colorMap[fraction];
            red = color.Red;
            green = color.Green;
            blue = color.Blue;
        }

        public override void StartThread(Action<Action> threadPoolExecutor = null)
        {
            base.StartThread(threadPoolExecutor);
        }

        protected void UpdateAreaPatchFractalImage()
        {
            var fractalArea = Area;
            var areaPatch = AreaPatch;

            var fractalImage = areaPatch.FractalImage;
            var image = (Bitmap)fractalImage.Image;
            var size = fractalImage.Size;

            for (var i = 0; i < size && !Stop; ++i)
            {
                for (var j = 0; j < size && !Stop; ++j)
                {
                    var pixel = fractalArea.GetPixel(areaPatch.StartIndexWidth + i, areaPatch.StartIndexHeight + j);
                    if (pixel != null)
                    {
                        image.SetPixel(i, j, ComputeColor(pixel));
                    }
                }
            }
        }
    }
}
