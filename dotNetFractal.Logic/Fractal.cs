using System;
using System.Drawing;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Compute a Fractal from left to right.
    /// </summary>
    abstract public class Fractal : Worker, IFractal
    {
        private readonly FractalColorMap m_colorMap = FractalColorMap.GetInstance();

        private FractalArea m_area = null;
        private FractalAreaPatch m_areaPatch = null;
        private double m_maxRadius = 4.0;
        private int m_maxIterations = 4096;
        private int m_maxColors = 256;

        public FractalArea Area
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

        public Fractal()
        {
        }

        public Color ComputeColor(int iteration, double previousRadius, double radius)
        {
            if (iteration >= MaxIterations)
                return Color.Black;

            GetColor(iteration, out var red, out var green, out var blue);

            if (iteration != 0)
            {
                // PRE: radius > MaxRadius (otherwise the fractal computation loop should not have stopped)
                var fraction = Math.Sqrt((MaxRadius - previousRadius) / (radius - previousRadius));
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
    }
}
