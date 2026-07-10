using System;
using System.Numerics;

namespace dotNetFractal.Logic
{
    public class DisplayArea<T> : IDisplayArea where T : INumber<T>
    {
        private readonly T m_half = T.CreateChecked(0.5);
        private readonly T m_one = T.CreateChecked(1.0);

        public T Cx { get; private set; }

        public T Cy { get; private set; }

        public T CenterX { get; private set; }

        public T CenterY { get; private set; }

        public T Width { get; private set; }

        public T Height { get; private set; }

        public T Right => CenterX + Width * m_half;

        public T Left => CenterX - Width * m_half;

        public T Top => CenterY + Height * m_half;

        public T Bottom => CenterY - Height * m_half;

        public int PixelsHorizontal { get; private set; }

        public int PixelsVertical { get; private set; }

        public DisplayArea(IDisplayArea displayArea)
        {
            var da = displayArea as DisplayArea<T> ?? throw new ArgumentException("Invalid display area type", nameof(displayArea));
            Cx = da.Cx;
            Cy = da.Cy;
            CenterX = da.CenterX;
            CenterY = da.CenterY;
            Width = da.Width;
            Height = da.Height;
            PixelsHorizontal = da.PixelsHorizontal;
            PixelsVertical = da.PixelsVertical;
        }

        public DisplayArea(T centerX, T centerY, T width, T height, T cx, T cy, int horizontal, int vertical)
        {
            Cx = cx;
            Cy = cy;
            CenterX = centerX;
            CenterY = centerY;
            PixelsHorizontal = horizontal;
            PixelsVertical = vertical;
            var ratio = T.CreateChecked(PixelsVertical) / T.CreateChecked(PixelsHorizontal);
            var length = T.Max(width, height);
            Width = length;
            Height = ratio * length;
        }

        public void Resize(int pixelsHorizontal, int pixelsVertical)
        {
            PixelsHorizontal = pixelsHorizontal;
            PixelsVertical = pixelsVertical;
            var ratio = T.CreateChecked(PixelsVertical) / T.CreateChecked(PixelsHorizontal);
            var length = T.Max(Width, Height);
            Width = length;
            Height = ratio * length;
        }

        public IDisplayArea ZoomIn(int i1, int j1, int i2, int j2, int horizontal, int vertical)
        {
            return DisplayAreaFactory.ZoomIn(this, i1, j1, i2, j2, horizontal, vertical);
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
            return CenterX + ((T.CreateChecked(i1 + i2) - T.CreateChecked(PixelsHorizontal)) * Width / T.CreateChecked(PixelsHorizontal)) * m_half;
        }

        public T GetCenterY(int j1, int j2)
        {
            return CenterY + ((T.CreateChecked(PixelsVertical) - (T.CreateChecked(j1 + j2))) * Height / T.CreateChecked(PixelsVertical)) * m_half;
        }

        public T GetWidth(int i1, int i2)
        {
            return T.Abs((T.CreateChecked(i2 - i1) * Width / T.CreateChecked(PixelsHorizontal)));
        }

        public T GetHeight(int j1, int j2)
        {
            return T.Abs((T.CreateChecked(j2 - j1) * Height / T.CreateChecked(PixelsVertical)));
        }

        public T GetX(int i)
        {
            return CenterX + ((T.CreateChecked(i) - T.CreateChecked(PixelsHorizontal) * m_half) * Width / T.CreateChecked(PixelsHorizontal));
        }

        public T GetY(int j)
        {
            return CenterY + ((T.CreateChecked(PixelsVertical) * m_half - T.CreateChecked(j)) * Height / T.CreateChecked(PixelsVertical));
        }

        public int GetI(T x)
        {
            return FractalNumberExtensions.Floor(T.CreateChecked(PixelsHorizontal) * (m_one + (x - CenterX) / Width) * m_half);
        }

        public int GetJ(T y)
        {
            return FractalNumberExtensions.Floor(T.CreateChecked(PixelsVertical) * (m_one + (CenterY - y) / Height) * m_half);
        }

    }
}
