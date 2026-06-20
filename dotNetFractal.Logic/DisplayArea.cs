using System;

namespace dotNetFractal.Logic
{
    public class DisplayArea<T> where T : IFractalUnit<T>, new()
    {
        private readonly T m_half = (T)0.5;
        private readonly T m_one = (T)1.0;

        public T CenterX { get; }

        public T CenterY { get; }

        public int PixelsHorizontal { get; }

        public int PixelsVertical { get; }

        public T Width { get; }

        public T Height { get; }

        public T Right => CenterX + Width * m_half;

        public T Left => CenterX - Width * m_half;

        public T Top => CenterY + Height * m_half;

        public T Bottom => CenterY - Height * m_half;

        public DisplayArea(DisplayArea<T> displayArea)
        {
            CenterX = displayArea.CenterX;
            CenterY = displayArea.CenterY;
            PixelsHorizontal = displayArea.PixelsHorizontal;
            PixelsVertical = displayArea.PixelsVertical;
            Width = displayArea.Width;
            Height = displayArea.Height;
        }

        public DisplayArea(T centerX, T centerY, T width, T height, int horizontal, int vertical)
        {
            CenterX = centerX;
            CenterY = centerY;
            PixelsHorizontal = horizontal;
            PixelsVertical = vertical;
            var ratio = (T)PixelsVertical / (T)PixelsHorizontal;
            var length = T.Max(width, height);
            Width = length;
            Height = ratio * length;
        }

        public static bool operator == (DisplayArea<T> a, DisplayArea<T> b)
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

        public static bool operator != (DisplayArea<T> a, DisplayArea<T> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is DisplayArea<T> other)
                return this == other;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CenterX, CenterY, PixelsHorizontal, PixelsVertical);
        }

        public T GetCenterX(int i1, int i2)
        {
            return CenterX + ((T)(i1 + i2 - PixelsHorizontal) * Width / (T)PixelsHorizontal) * m_half;
        }

        public T GetCenterY(int j1, int j2)
        {
            return CenterY + ((T)(PixelsVertical - (j1 + j2)) * Height / (T)PixelsVertical) * m_half;
        }

        public T GetWidth(int i1, int i2)
        {
            return T.Abs((T)(i2 - i1) * Width / (T)PixelsHorizontal);
        }

        public T GetHeight(int j1, int j2)
        {
            return T.Abs((T)(j2 - j1) * Height / (T)PixelsVertical);
        }

        public T GetX(int i)
        {
            return CenterX + ((T)i - (T)PixelsHorizontal * m_half) * Width / (T)PixelsHorizontal;
        }

        public T GetY(int j)
        {
            return CenterY + ((T)PixelsVertical * m_half - (T)j) * Height / (T)PixelsVertical;
        }

        public int GetI(T x)
        {
            return T.Floor((T)PixelsHorizontal * (m_one + (x - CenterX) / Width) * m_half);
        }

        public int GetJ(T y)
        {
            return T.Floor((T)PixelsVertical * (m_one + (CenterY - y) / Height) * m_half);
        }
    }
}
