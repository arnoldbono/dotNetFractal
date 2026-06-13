using System;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Summary description for FractalColorMap.
    /// </summary>
    public class FractalColorMap
    {
        private static FractalColorMap m_instance;

        public static FractalColorMap GetInstance()
        {
            return m_instance ??= new FractalColorMap();
        }

        private FractalColorMap()
        {
        }

        public FractalColor this[double fraction]
        {
            get
            {
                var color1 = Colors[0];
                var color2 = color1;
                int i = 0;
                while (true)
                {
                    color1 = color2;
                    color2 = Colors[++i];
                    if (color2.Fraction >= fraction)
                    {
                        break;
                    }
                }
                var color = new FractalColor();
                double factor = (double)(color2.Fraction - fraction) / (double)(color2.Fraction - color1.Fraction);
                color.Red = (int)((double)color2.Red + (double)(color1.Red - color2.Red) * factor);
                color.Green = (int)((double)color2.Green + (double)(color1.Green - color2.Green) * factor);
                color.Blue = (int)((double)color2.Blue + (double)(color1.Blue - color2.Blue) * factor);
                color.Fraction = fraction;
                return color;
            }
        }

        public FractalColor[] Colors =
        {
            new(  3,  0,  0, 0.0),
            new( 30,  0,  0, 1.0 / 255.0),
            new(255,  0,  0, 10.0 / 255.0),
            new(255,255,  0, 63.0 / 255.0),
            new(  0,255,  0, 95.0 / 255.0),
            new(  0,255,255, 127.0 / 255.0),
            new(  0,  0,255, 159.0 / 255.0),
            new(255,  0,255, 191.0 / 255.0),
            new(255,255,255, 223.0 / 255.0),
            new(  3,  3,  3, 1.0)
        };
    }
}
