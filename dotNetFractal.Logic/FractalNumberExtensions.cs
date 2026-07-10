using System;
using System.Numerics;

namespace dotNetFractal.Logic
{
    public static class FractalNumberExtensions
    {
        public static int Floor<T>(T v) where T : INumber<T>
        {
            // Convert to int truncates towards zero
            int truncated = int.CreateTruncating(v);

            // If the value is negative and not an integer, we need to subtract 1
            // because truncation rounds towards zero, but floor rounds down
            T truncatedAsT = T.CreateTruncating(truncated);

            if (v < T.Zero && v != truncatedAsT)
            {
                return truncated - 1;
            }

            return truncated;
        }
    }
}
