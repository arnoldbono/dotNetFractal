using System;

namespace dotNetFractal
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
            if (width > height)
            {
                Width = width;
                Height = ratio * Width;
            }
            else
            {
                Height = height;
                Width = Height / ratio;
            }
        }

        public double GetX(int i)
        {
            return CenterX + (double)(i - PixelsHorizontal / 2) * Width / (double)PixelsHorizontal;
        }

        public double GetY(int j)
        {
            return CenterY + (double)(PixelsVertical / 2 - j) * Height / (double)PixelsVertical;
        }

        public double GetWidth(int w)
        {
            return (double)w / (double)PixelsHorizontal * Width;
        }

        public double GetHeight(int h)
        {
            return (double)h / (double)PixelsVertical * Height;
        }

        public void GetPosition(int i, int j, out double x, out double y)
        {
            x = GetX(i);
            y = GetY(j);
        }
     }
}
