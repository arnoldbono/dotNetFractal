using System;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Summary description for FractalColor.
    /// </summary>
    public struct FractalColor(int red, int green, int blue, double fraction)
    {
        private int m_red = red;
        private int m_green = green;
        private int m_blue = blue;
        private double m_fraction = fraction;

        public override readonly string ToString()
        {
            return @"({Red}, {Green}, {Blue}, {Fraction})";
        }

        public int Red
        {
            get { return m_red; }
            set { m_red = value & 0xFF; }
        }

        public int Green
        {
            get { return m_green; }
            set { m_green = value & 0xFF; }
        }

        public int Blue
        {
            get { return m_blue; }
            set { m_blue = value; }
        }

        public double Fraction
        {
            get { return m_fraction; }
            set { m_fraction = value; }
        }
    };
}
