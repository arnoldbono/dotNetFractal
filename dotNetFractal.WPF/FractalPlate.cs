using System;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// A plate from the bool "Fractal Programming in C".
    /// </summary>
    public struct FractalPlate
    {
        public string Name { get; }
        public double MinX { get; }
        public double MinY { get; }
        public double MaxX { get; }
        public double MaxY { get; }

        public FractalPlate(string name, double minX, double minY, double maxX, double maxY)
        {
            Name = name;
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public override string ToString()
        {
            return ($@"({Name}, {MinX}, {MinY}, {MaxX}, {MaxY}");
        }
    };
}
