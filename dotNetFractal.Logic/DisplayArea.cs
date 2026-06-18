using System;

namespace dotNetFractal.Logic
{
    public class DisplayArea
    {
        public double CenterX { get; }

        public double CenterY { get; }

        public int PixelsHorizontal { get; }

        public int PixelsVertical { get; }

        public double Width { get; }

        public double Height { get; }

        public double Right
        {
            get { return CenterX + Width / 2.0; }
        }

        public double Left
        {
            get { return CenterX - Width / 2.0; }
        }

        public double Top
        {
            get { return CenterY + Height / 2.0; }
        }

        public double Bottom
        {
            get { return CenterY - Height / 2.0; }
        }

        public DisplayArea(double centerX, double centerY, double width, double height, int horizontal, int vertical)
        {
            CenterX = centerX;
            CenterY = centerY;
            PixelsHorizontal = horizontal;
            PixelsVertical = vertical;
            var ratio = (double)PixelsVertical / (double)PixelsHorizontal;
            var length = Math.Max(width, height);
            Width = length;
            Height = ratio * length;
            //Width = width;
            //Height = height;
        }

        public double GetCenterX(int i1, int i2)
        {
            return CenterX + ((double)(i1 + i2 - PixelsHorizontal) * Width / (double)PixelsHorizontal) / 2.0;
        }

        public double GetCenterY(int j1, int j2)
        {
            return CenterY + ((double)(PixelsVertical - (j1 + j2)) * Height / (double)PixelsVertical) / 2.0;
        }

        public double GetWidth(int i1, int i2)
        {
            return Math.Abs((double)(i2 - i1) * Width / (double)PixelsHorizontal);
        }

        public double GetHeight(int j1, int j2)
        {
            return Math.Abs((double)(j2 - j1) * Height / (double)PixelsVertical);
        }

        public double GetX(int i)
        {
            return CenterX + (double)(i - PixelsHorizontal / 2) * Width / (double)PixelsHorizontal;
        }

        public double GetY(int j)
        {
            return CenterY + (double)(PixelsVertical / 2 - j) * Height / (double)PixelsVertical;
        }

        public int GetI(double x)
        {
            return (int)Math.Floor((double)PixelsHorizontal * (1.0 + (x - CenterX) / Width) / 2.0);
        }

        public int GetJ(double y)
        {
            return (int)Math.Floor((double)PixelsVertical * (1.0 + (CenterY - y) / Height) / 2.0);
        }
    }
}
