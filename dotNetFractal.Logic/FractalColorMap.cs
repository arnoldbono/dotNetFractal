using System;

namespace dotNetFractal
{
	/// <summary>
	/// Summary description for FractalColorMap.
	/// </summary>
	public class FractalColorMap
	{
		public FractalColorMap()
		{
			;
		}

		public int Length
		{
			get
            {
                return FractalColorMap.colorMap[FractalColorMap.colorMap.Length - 1].index;
            }
		}

		public FractalColor this[int index]
		{
			get
            {
                FractalColor color1 = FractalColorMap.colorMap[0];
                FractalColor color2 = color1;
                int i = 0;
                while (true)
                {
                    color1 = color2;
                    color2 = FractalColorMap.colorMap[++i];
                    if (color2.index >= index)
                    {
                        break;
                    }
                }
                FractalColor color = new FractalColor();
                double factor = (double)(color2.index - index) / (double)(color2.index - color1.index);
                color.red = (int)((double)color2.red + (double)(color1.red - color2.red) * factor);
                color.green = (int)((double)color2.green + (double)(color1.green - color2.green) * factor);
                color.blue = (int)((double)color2.blue + (double)(color1.blue - color2.blue) * factor);
                color.index = index;
                return color;
            }
		}

		private static FractalColor[] colorMap =
		{
            new FractalColor(  7,  0,  0, 0),
            new FractalColor(255,  0,  0, 31),
            new FractalColor(255,255,  0, 63),
            new FractalColor(  0,255,  0, 95),
            new FractalColor(  0,255,255, 127),
            new FractalColor(  0,  0,255, 159),
            new FractalColor(255,  0,255, 191),
            new FractalColor(255,255,255, 223),
            new FractalColor(  3,  3,  3, 255)
		};
	}
}
