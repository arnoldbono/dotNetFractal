using System;
using System.IO;

namespace dotNetFractal.Logic
{
    public class DisplayArea<T> : IDisplayArea where T : IFractalUnit<T>, new()
    {
        private readonly T m_half = (T)0.5;
        private readonly T m_one = (T)1.0;

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
            var displayAreaTyped = displayArea as DisplayArea<T> ?? throw new ArgumentException("Invalid display area type", nameof(displayArea));
            CenterX = displayAreaTyped.CenterX;
            CenterY = displayAreaTyped.CenterY;
            PixelsHorizontal = displayAreaTyped.PixelsHorizontal;
            PixelsVertical = displayAreaTyped.PixelsVertical;
            Width = displayAreaTyped.Width;
            Height = displayAreaTyped.Height;
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

        public void JuliaSetResetCenter()
        {
            CenterX = (T)0.0;
            CenterY = (T)0.0;
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

        public static DisplayArea<T> Read(BinaryReader br)
        {
            var centerX = (T)(decimal)br.ReadDecimal();
            var centerY = (T)(decimal)br.ReadDecimal();
            var width = (T)(decimal)br.ReadDecimal();
            var height = (T)(decimal)br.ReadDecimal();
            var pixelsHorizontal = br.ReadInt32();
            var pixelsVertical = br.ReadInt32();
            return new DisplayArea<T>(centerX, centerY, width, height, pixelsHorizontal, pixelsVertical);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((decimal)CenterX);
            bw.Write((decimal)CenterY);
            bw.Write((decimal)Width);
            bw.Write((decimal)Height);
            bw.Write(PixelsHorizontal);
            bw.Write(PixelsVertical);
        }
    }
}
