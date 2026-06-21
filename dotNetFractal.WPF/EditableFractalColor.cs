using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using dotNetFractal.Logic;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// Editable wrapper for FractalColor to support data binding and property change notifications
    /// </summary>
    public class EditableFractalColor(FractalColor color) : INotifyPropertyChanged
    {
        private int m_red = color.Red;
        private int m_green = color.Green;
        private int m_blue = color.Blue;
        private double m_fraction = color.Fraction;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Red
        {
            get => m_red;
            set
            {
                if (m_red != value)
                {
                    m_red = ClampToByte(value);
                    OnPropertyChanged();
                }
            }
        }

        public int Green
        {
            get => m_green;
            set
            {
                if (m_green != value)
                {
                    m_green = ClampToByte(value);
                    OnPropertyChanged();
                }
            }
        }

        public int Blue
        {
            get => m_blue;
            set
            {
                if (m_blue != value)
                {
                    m_blue = ClampToByte(value);
                    OnPropertyChanged();
                }
            }
        }

        public double Fraction
        {
            get => m_fraction;
            set
            {
                if (m_fraction != value)
                {
                    m_fraction = ClampToFraction(value);
                    OnPropertyChanged();
                }
            }
        }

        private static int ClampToByte(int value)
        {
            return Math.Max(0, Math.Min(255, value));
        }

        private static double ClampToFraction(double value)
        {
            return Math.Max(0.0, Math.Min(1.0, value));
        }

        public FractalColor ToFractalColor()
        {
            return new FractalColor(Red, Green, Blue, Fraction);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
