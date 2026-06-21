using System;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// A plate from the bool "Fractal Programming in C".
    /// </summary>
    public class FractalPlate(string name, double minX, double minY, double maxX, double maxY)
    {
        public string Name { get; } = name;
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
        }
    }
}
