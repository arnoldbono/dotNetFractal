using System;

namespace dotNetFractal.Logic
{
    public class DisplayArea
    {
        public decimal CenterX { get; }

        public decimal CenterY { get; }

        public int PixelsHorizontal { get; }

        public int PixelsVertical { get; }

        public decimal Width { get; }

        public decimal Height { get; }

        public decimal Right => CenterX + Width / 2.0m;

        public decimal Left => CenterX - Width / 2.0m;

        public decimal Top => CenterY + Height / 2.0m;

        public decimal Bottom => CenterY - Height / 2.0m;

        public DisplayArea(DisplayArea displayArea)
        {
            CenterX = displayArea.CenterX;
            CenterY = displayArea.CenterY;
            PixelsHorizontal = displayArea.PixelsHorizontal;
            PixelsVertical = displayArea.PixelsVertical;
            Width = displayArea.Width;
            Height = displayArea.Height;
        }

        public DisplayArea(decimal centerX, decimal centerY, decimal width, decimal height, int horizontal, int vertical)
        {
            CenterX = centerX;
            CenterY = centerY;
            PixelsHorizontal = horizontal;
            PixelsVertical = vertical;
            var ratio = (decimal)PixelsVertical / (decimal)PixelsHorizontal;
            var length = Math.Max(width, height);
            Width = length;
            Height = ratio * length;
            //Width = width;
            //Height = height;
        }

        public static bool operator == (DisplayArea a, DisplayArea b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;
            return a.CenterX == b.CenterX &&
                   a.CenterY == b.CenterY &&
                   a.PixelsHorizontal == b.PixelsHorizontal &&
                   a.PixelsVertical == b.PixelsVertical;
            // Width and Height are not considered in equality.
        }

        public static bool operator != (DisplayArea a, DisplayArea b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is DisplayArea other)
                return this == other;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CenterX, CenterY, PixelsHorizontal, PixelsVertical);
        }

        public decimal GetCenterX(int i1, int i2)
        {
            return CenterX + ((i1 + i2 - PixelsHorizontal) * Width / PixelsHorizontal) / 2.0m;
        }

        public decimal GetCenterY(int j1, int j2)
        {
            return CenterY + ((PixelsVertical - (j1 + j2)) * Height / PixelsVertical) / 2.0m;
        }

        public decimal GetWidth(int i1, int i2)
        {
            return Math.Abs((i2 - i1) * Width / PixelsHorizontal);
        }

        public decimal GetHeight(int j1, int j2)
        {
            return Math.Abs((j2 - j1) * Height / PixelsVertical);
        }

        public decimal GetX(int i)
        {
            return CenterX + (i - PixelsHorizontal / 2) * Width / (decimal)PixelsHorizontal;
        }

        public decimal GetY(int j)
        {
            return CenterY + (PixelsVertical / 2 - j) * Height / PixelsVertical;
        }

        public int GetI(decimal x)
        {
            return (int)Math.Floor(PixelsHorizontal * (1.0m + (x - CenterX) / Width) / 2.0m);
        }

        public int GetJ(decimal y)
        {
            return (int)Math.Floor(PixelsVertical * (1.0m + (CenterY - y) / Height) / 2.0m);
        }
    }
}
