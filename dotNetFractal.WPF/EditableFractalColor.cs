using System.ComponentModel;
using System.Runtime.CompilerServices;
using dotNetFractal.Logic;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// Editable wrapper for FractalColor to support data binding and property change notifications
    /// </summary>
    public class EditableFractalColor : INotifyPropertyChanged
    {
        private int m_red;
        private int m_green;
        private int m_blue;
        private double m_fraction;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Red
        {
            get => m_red;
            set
            {
                if (m_red != value)
                {
                    m_red = value & 0xFF; // Clamp to 0-255
                    OnPropertyChanged();
                    OnColorChanged();
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
                    m_green = value & 0xFF; // Clamp to 0-255
                    OnPropertyChanged();
                    OnColorChanged();
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
                    m_blue = value & 0xFF; // Clamp to 0-255
                    OnPropertyChanged();
                    OnColorChanged();
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
                    m_fraction = value;
                    OnPropertyChanged();
                    OnColorChanged();
                }
            }
        }

        public event System.Action ColorChanged;

        public EditableFractalColor(FractalColor color)
        {
            m_red = color.Red;
            m_green = color.Green;
            m_blue = color.Blue;
            m_fraction = color.Fraction;
        }

        public FractalColor ToFractalColor()
        {
            return new FractalColor(Red, Green, Blue, Fraction);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnColorChanged()
        {
            ColorChanged?.Invoke();
        }
    }
}
