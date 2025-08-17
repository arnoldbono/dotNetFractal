using System;
using System.Drawing;

namespace dotNetFractal
{
	/// <summary>
	/// Compute a Fractal from left to right.
	/// </summary>
	abstract public class Fractal : Worker, IFractal
	{
        private readonly FractalColorMap m_colorMap = new FractalColorMap();

        private FractalArea m_area = null;
        private FractalAreaPatch m_areaPatch = null;
        private double m_maxRadius = 4.0;
        private int m_maxIterations = 256;
        private int m_computedWidth = 0;

        public FractalArea Area
        {
            get { return m_area; }
            set
            {
                base.StopThread();
                m_area = value;
            }
        }

        public FractalAreaPatch AreaPatch
        {
            get { return m_areaPatch; }
            set
            {
                base.StopThread();
                m_areaPatch = value;
            }
        }

        public int ComputedWidth
        {
            get { return m_computedWidth; }
            protected set { m_computedWidth = value; }
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

		public Fractal()
		{
        }

        public Color ComputeColor(int iteration, double previousRadius, double radius)
        {
            int red = 0;
            int green = 0;
            int blue = 0;
            if (iteration < MaxIterations)
            {
                GetColor(iteration, out red, out green, out blue);

                if (iteration != 0)
                {
                    var fraction = Math.Sqrt((MaxRadius - previousRadius) / (radius - previousRadius));
                    if (fraction < 1.0)
                    {
                        GetColor(iteration - 1, out var red1, out var green1, out var blue1);

                        red = (int)((double)red1 + fraction * (red - red1));
                        green = (int)((double)green1 + fraction * (green - green1));
                        blue = (int)((double)blue1 + fraction * (blue - blue1));
                    }
                }
            }
            return Color.FromArgb(red, green, blue);
        }

        public void GetColor(int index, out int red, out int green, out int blue)
        {
            index = index % m_colorMap.Length;
            var color = m_colorMap[index];
            red = color.red;
            green = color.green;
            blue = color.blue;
        }

        public override void StartThread()
		{
            base.StartThread();
            m_computedWidth = 0;
        }

    }
}
