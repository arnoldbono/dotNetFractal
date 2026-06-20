
namespace dotNetFractal.Logic
{
    /// <summary>
    /// Represents a unit type used in fractal calculations that supports basic arithmetic operations.
    /// </summary>
    /// <typeparam name="TSelf">The implementing type</typeparam>
    public interface IFractalUnit<TSelf> where TSelf : IFractalUnit<TSelf>, new()
    {
        // Arithmetic
        static abstract TSelf operator +(TSelf left, TSelf right);
        static abstract TSelf operator -(TSelf left, TSelf right);
        static abstract TSelf operator *(TSelf left, TSelf right);
        static abstract TSelf operator /(TSelf left, TSelf right);

        // Comparison
        static abstract bool operator <(TSelf left, TSelf right);
        static abstract bool operator >(TSelf left, TSelf right);
        static abstract bool operator <=(TSelf left, TSelf right);
        static abstract bool operator >=(TSelf left, TSelf right);
        static abstract bool operator ==(TSelf left, TSelf right);
        static abstract bool operator !=(TSelf left, TSelf right);

        // Explicit conversion
        static abstract explicit operator TSelf(decimal value);
        static abstract explicit operator TSelf(double value);
        static abstract explicit operator TSelf(int value);
        static abstract explicit operator double(TSelf value);
        static abstract explicit operator decimal(TSelf value);

        static abstract TSelf Max(TSelf a, TSelf b);
        static abstract TSelf Min(TSelf a, TSelf b);
        static abstract TSelf Abs(TSelf v);
        static abstract int Floor(TSelf v);
    }
}
