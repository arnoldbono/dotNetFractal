using System;

namespace dotNetFractal.Logic
{
    public class FractalDouble(double value) : IFractalUnit<FractalDouble>
    {
        private double m_value = value;

        public FractalDouble() : this(0.0) { }

        public static FractalDouble operator +(FractalDouble a, FractalDouble b)
        {
            return new FractalDouble(a.m_value + b.m_value);
        }

        public static FractalDouble operator -(FractalDouble a, FractalDouble b)
        {
            return new FractalDouble(a.m_value - b.m_value);
        }

        public static FractalDouble operator *(FractalDouble a, FractalDouble b)
        {
            return new FractalDouble(a.m_value * b.m_value);
        }

        public static FractalDouble operator /(FractalDouble a, FractalDouble b)
        {
            return new FractalDouble(a.m_value / b.m_value);
        }

        public static bool operator >(FractalDouble a, FractalDouble b)
        {
            return a.m_value > b.m_value;
        }

        public static bool operator <(FractalDouble a, FractalDouble b)
        {
            return a.m_value < b.m_value;
        }

        public static bool operator >=(FractalDouble a, FractalDouble b)
        {
            return a.m_value >= b.m_value;
        }

        public static bool operator <=(FractalDouble a, FractalDouble b)
        {
            return a.m_value <= b.m_value;
        }

        public static bool operator ==(FractalDouble a, FractalDouble b)
        {
            return a.m_value == b.m_value;
        }

        public static bool operator !=(FractalDouble a, FractalDouble b)
        {
            return a.m_value != b.m_value;
        }

        public static FractalDouble Max(FractalDouble a, FractalDouble b)
        {
            return a.m_value > b.m_value ? a : b;
        }

        public static FractalDouble Min(FractalDouble a, FractalDouble b)
        {
            return a.m_value < b.m_value ? a : b;
        }

        public static FractalDouble Abs(FractalDouble v)
        {
            return v.m_value >= 0 ? v : new FractalDouble(-v.m_value);
        }

        public static int Floor(FractalDouble v)
        {
            return (int)Math.Floor(v.m_value);
        }

        public static explicit operator FractalDouble(int i)
        {
            return new FractalDouble(i);
        }

        public static explicit operator FractalDouble(double d)
        {
            return new FractalDouble(d);
        }

        public static explicit operator FractalDouble(decimal d)
        {
            return new FractalDouble((double)d);
        }

        public static explicit operator double(FractalDouble d)
        {
            return d.m_value;
        }

        public static explicit operator decimal(FractalDouble d)
        {
            return (decimal)d.m_value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is null || obj is not FractalDouble doubleObj)
                return false;

            return this.m_value == doubleObj.m_value;
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }
    }

    public class FractalDecimal(decimal value) : IFractalUnit<FractalDecimal>
    {
        private decimal m_value = value;

        public FractalDecimal() : this(0.0m) { }

        public static FractalDecimal operator +(FractalDecimal a, FractalDecimal b)
        {
            return new FractalDecimal(a.m_value + b.m_value);
        }

        public static FractalDecimal operator -(FractalDecimal a, FractalDecimal b)
        {
            return new FractalDecimal(a.m_value - b.m_value);
        }

        public static FractalDecimal operator *(FractalDecimal a, FractalDecimal b)
        {
            return new FractalDecimal(a.m_value * b.m_value);
        }

        public static FractalDecimal operator /(FractalDecimal a, FractalDecimal b)
        {
            return new FractalDecimal(a.m_value / b.m_value);
        }

        public static bool operator >(FractalDecimal a, FractalDecimal b)
        {
            return a.m_value > b.m_value;
        }

        public static bool operator <(FractalDecimal a, FractalDecimal b)
        {
            return a.m_value < b.m_value;
        }

        public static bool operator >=(FractalDecimal a, FractalDecimal b)
        {
            return a.m_value >= b.m_value;
        }

        public static bool operator <=(FractalDecimal a, FractalDecimal b)
        {
            return a.m_value <= b.m_value;
        }

        public static bool operator ==(FractalDecimal a, FractalDecimal b)
        {
            return a.m_value == b.m_value;
        }

        public static bool operator !=(FractalDecimal a, FractalDecimal b)
        {
            return a.m_value != b.m_value;
        }

        public static FractalDecimal Max(FractalDecimal a, FractalDecimal b)
        {
            return a.m_value > b.m_value ? a : b;
        }

        public static FractalDecimal Min(FractalDecimal a, FractalDecimal b)
        {
            return a.m_value < b.m_value ? a : b;
        }

        public static FractalDecimal Abs(FractalDecimal v)
        {
            return v.m_value >= 0 ? v : new FractalDecimal(-v.m_value);
        }

        public static int Floor(FractalDecimal v)
        {
            return (int)Math.Floor(v.m_value);
        }

        public static explicit operator FractalDecimal(decimal d)
        {
            return new FractalDecimal(d);
        }

        public static explicit operator FractalDecimal(double d)
        {
            return new FractalDecimal((decimal)d);
        }

        public static explicit operator FractalDecimal(int i)
        {
            return new FractalDecimal(i);
        }

        public static explicit operator decimal(FractalDecimal d)
        {
            return d.m_value;
        }

        public static explicit operator double(FractalDecimal d)
        {
            return (double)d.m_value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is null || obj is not FractalDecimal decimalObj)
                return false;

            return this.m_value == decimalObj.m_value;
        }

        public override int GetHashCode()
        {
            return m_value.GetHashCode();
        }
    }
}
