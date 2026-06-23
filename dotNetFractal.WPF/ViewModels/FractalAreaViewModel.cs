using dotNetFractal.Logic;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace dotNetFractal.WPF.ViewModels
{
    /// <summary>
    /// The view model for the fractal area, which is used to display the fractal in the UI.
    /// Here, we use decimals to have the best precision for the fractal area.
    /// This view model is used by the XAML view to bind the fractal area properties to the UI elements.
    /// </summary>
    public sealed class FractalAreaViewModel : FractalAreaViewModelBase<FractalDecimal>
    {
        public FractalAreaViewModel() : base()
        {
            ; // Empty
        }
    }

    /// <summary>
    /// INTERNAL USE ONLY - Do not use directly. Use FractalAreaViewModel instead.
    /// This generic base class is not intended for public consumption.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FractalAreaViewModelBase<T> : BaseViewModel where T : IFractalUnit<T>, new()
    {
        private T m_cx = new();
        private T m_cy = new();
        private T m_centerX = new();
        private T m_centerY = new();
        private T m_width = new();
        private T m_height = new();
        private int m_selectedPlate = 0;
        private bool m_juliaSet = false;
        private List<FractalPlate> m_plates = [];

        public readonly List<FractalPlate> m_mandelbrotPlates =
        [
            new("Plate 0",  -2.4,      -1.4,      1.4,      1.4),
            new("Plate 4",  -2.0,      -1.25,      0.5,      1.25),
            new("Plate 5",  -0.702973,  0.374785, -0.642879, 0.395415),
            new("Plate 5a", -0.691594,  0.386608, -0.690089, 0.387494),
            new("Plate 6",  -0.691060,  0.387103, -0.690906, 0.387228),
            new("Plate 7",  -0.793114,  0.037822, -0.723005, 0.140974),
            new("Plate 7a", -0.749337,  0.109349, -0.744948, 0.115851),
            new("Plate 8",  -0.745465,  0.112896, -0.745387, 0.113034),
            new("Plate 9",  -0.745464,  0.112967, -0.745388, 0.113030)
        ];

        public readonly List<FractalPlate> m_juliaSetPlates =
        [
            new JuliaFractalPlate("Golden    0", -0.264508,  0.457591, 4.0, 4.0),
            new JuliaFractalPlate("Daunting  1",  0.315479,  0.027924, 4.0, 4.0),
            new JuliaFractalPlate("Plate 12, A",  0.238498,  0.519198, 4.0, 4.0),
            new JuliaFractalPlate("Plate 13, B", -0.743036,  0.113467, 4.0, 4.0),
            new JuliaFractalPlate("Plate --, C", -0.192175,  0.656734, 4.0, 4.0),
            new JuliaFractalPlate("Plate 14, D",  0.108294, -0.670487, 4.0, 4.0),
            new JuliaFractalPlate("Plate --, E", -0.392488, -0.587966, 4.0, 4.0),
            new JuliaFractalPlate("Plate --, F", -0.392488, -0.587966, 4.0, 4.0),
            new JuliaFractalPlate("Plate 15, G",  0.138341,  0.649857, 4.0, 4.0),
            new JuliaFractalPlate("Plate 16, H",  0.278560, -0.003483, 4.0, 4.0),
            new JuliaFractalPlate("Plate --, I", -1.258842,  0.065330, 4.0, 4.0),
            new JuliaFractalPlate("Plate --, J", -1.028482, -0.264756, 4.0, 4.0),
            new JuliaFractalPlate("Plate --, K",  0.268545, -0.003483, 4.0, 4.0),
            new JuliaFractalPlate("Plate 17, L",  0.268545, -0.003483, 4.0, 4.0),
            new JuliaFractalPlate("Plate --, M",  0.268545, -0.003483, 4.0, 4.0),
            new JuliaFractalPlate("Plate 18, N",  0.318623,  0.044699, 4.0, 4.0),
            new JuliaFractalPlate("Plate 19, O",  0.318623,  0.429799, 4.0, 4.0)
        ];

        public List<FractalPlate> Plates
        {
            get => m_plates;
            set
            {
                if (m_plates == value)
                    return;

                m_plates = value;
                OnPropertyChanged();
            }
        }

        public IDisplayArea GetDisplayArea(int width, int height)
        {
            return new DisplayArea<T>(CenterX, CenterY, Width, Height, Cx, Cy, width, height);
        }

        public T Cx
        {
            get => m_cx;
            set
            {
                if (m_cx == value)
                {
                    return;
                }

                m_cx = value;
                OnPropertyChanged();
            }
        }

        public T Cy
        {
            get => m_cy;
            set
            {
                if (m_cy == value)
                {
                    return;
                }

                m_cy = value;
                OnPropertyChanged();
            }
        }

        public T CenterX
        {
            get => m_centerX;
            set
            {
                if (m_centerX == value)
                {
                    return;
                }

                m_centerX = value;
                OnPropertyChanged();
            }
        }

        public T CenterY
        {
            get => m_centerY;
            set
            {
                if (m_centerY == value)
                {
                    return;
                }

                m_centerY = value;
                OnPropertyChanged();
            }
        }

        public T Width
        {
            get => m_width;
            set
            {
                if (m_width == value)
                {
                    return;
                }

                m_width = value;
                OnPropertyChanged();
            }
        }

        public T Height
        {
            get => m_height;
            set
            {
                if (m_height == value)
                {
                    return;
                }
                m_height = value;
                OnPropertyChanged();
            }
        }

        public int SelectedPlate
        {
            get => m_selectedPlate;
            set
            {
                // Clamp the value to valid range for current plates collection
                var clampedValue = value;
                if (clampedValue < 0)
                    clampedValue = 0;
                else if (m_plates.Count > 0 && clampedValue >= m_plates.Count)
                    clampedValue = m_plates.Count - 1;

                if (m_selectedPlate == clampedValue)
                {
                    return;
                }

                m_selectedPlate = clampedValue;
                OnPropertyChanged();
            }
        }

        public bool JuliaSet
        {
            get => m_juliaSet;
            set
            {
                if (m_juliaSet == value)
                {
                    return;
                }

                m_juliaSet = value;
                OnPropertyChanged();
            }
        }

        public FractalAreaViewModelBase()
        {
            Plates = m_juliaSet ? m_juliaSetPlates : m_mandelbrotPlates;
            this.WhenAnyValue(x => x.SelectedPlate).Subscribe(_ => OnSelectedPlate(SelectedPlate));
            this.WhenAnyValue(x => x.JuliaSet).Subscribe(_ =>
            {
                var newPlates = JuliaSet ? m_juliaSetPlates : m_mandelbrotPlates;
                // Reset to the first plate when switching between Mandelbrot and Julia sets
                // Must be done BEFORE updating Plates to avoid index out of range
                if (SelectedPlate >= newPlates.Count)
                {
                    SelectedPlate = 0;
                }
                Plates = newPlates;
                OnSelectedPlate(SelectedPlate);
            });
        }

        private void OnSelectedPlate(int plateIndex)
        {
            // Validate plate index is within bounds
            var platesList = JuliaSet ? m_juliaSetPlates : m_mandelbrotPlates;
            if (plateIndex < 0 || plateIndex >= platesList.Count)
                return;

            var plate = platesList[plateIndex];
            if (JuliaSet)
            {
                Cx = (T)plate.Cx;
                Cy = (T)plate.Cy;
                CenterX = (T)0.0;
                CenterY = (T)0.0;
            }
            else
            {
                Cx = (T)0.0;
                Cy = (T)0.0;
                CenterX = (T)plate.CenterX;
                CenterY = (T)plate.CenterY;
            }
            Width = (T)plate.Width;
            Height = (T)plate.Height;
        }
    }
}
