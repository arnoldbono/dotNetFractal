using System;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// A plate from the bool "Fractal Programming in C".
    /// </summary>
    public struct FractalPlate(string name, double minX, double minY, double maxX, double maxY)
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
}
