using System;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// A plate from the book "Fractal Programming in C" by Roger T. Stevens.
    /// </summary>
    public class FractalPlate(string name, double minX, double minY, double maxX, double maxY)
    {
        public string Name { get; } = name;
        public double Cx { get; protected set; } = 0.0;
        public double Cy { get; protected set; } = 0.0;
        public double CenterX { get; protected set; } = (minX + maxX) / 2;
        public double CenterY { get; protected set; } = (minY + maxY) / 2;
        public double Width { get; protected set; } = maxX - minX;
        public double Height { get; protected set; } = maxY - minY;
        public double MinX { get; } = minX;
        public double MinY { get; } = minY;
        public double MaxX { get; } = maxX;
        public double MaxY { get; } = maxY;

        public override string ToString()
        {
            return $@"{Name}";
        }
    };

    public class JuliaFractalPlate : FractalPlate
    {
        public JuliaFractalPlate(string name, double centerX, double centerY, double width, double height)
            : base(name, centerX - width / 2, centerY - height / 2, centerX + width / 2, centerY + height / 2)
        {
            Cx = centerX;
            Cy = centerY;
            CenterX = 0.0;
            CenterY = 0.0;
            Width = width;
            Height = height;
        }
    }
}
