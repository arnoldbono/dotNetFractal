using System;
using System.Numerics;

namespace dotNetFractal.Logic;

public class FractalPixel<T> : IFractalPixel where T : INumber<T>, new()
{
    public int Iteration { get; }

    public T Radius { get; }

    public T PreviousRadius { get; }

    public double GetEscapeFraction(double maxRadius)
    {
        // PRE: radius > MaxRadius (otherwise the fractal computation loop should not have stopped).
        // PRE: previousRadius < MaxRadius.
        return Math.Sqrt(double.CreateChecked((T.CreateChecked(maxRadius) - PreviousRadius) / (Radius - PreviousRadius)));
    }

    public FractalPixel(int iteration, T radius, T previousRadius)
    {
        Iteration = iteration;
        Radius = radius;
        PreviousRadius = previousRadius;
    }
}
