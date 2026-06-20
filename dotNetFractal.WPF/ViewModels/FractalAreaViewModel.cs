using ReactiveUI;
using System;
using System.Collections.Generic;

using dotNetFractal.Logic;

namespace dotNetFractal.WPF.ViewModels
{
    public class FractalAreaViewModel : FractalAreaViewModel<FractalDouble>
    {
        public FractalAreaViewModel() : base()
        {
            ; // Empty
        }
    }

    public class FractalAreaViewModel<T> : BaseViewModel where T : IFractalUnit<T>, new()
    {
        private readonly T m_half = (T)0.5;

        public readonly List<FractalPlate> m_plates =
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

        private T m_centerX = new();
        private T m_centerY = new();
        private T m_width = new();
        private T m_height = new();
        private int m_selectedPlate = 0;

        public List<FractalPlate> Plates => m_plates;

        public IDisplayArea GetDisplayArea(int width, int height)
        {
            return new DisplayArea<T>(CenterX, CenterY, Width, Height, width, height);
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
                if (m_selectedPlate == value)
                {
                    return;
                }

                m_selectedPlate = value;
                OnPropertyChanged();
            }
        }

        public FractalAreaViewModel()
        {
            this.WhenAnyValue(x => x.SelectedPlate).Subscribe(OnSelectedPlate);
        }

        private void OnSelectedPlate(int plateIndex)
        {
            var plate = Plates[plateIndex];
            CenterX = (T)(plate.MinX + plate.MaxX) * m_half;
            CenterY = (T)(plate.MinY + plate.MaxY) * m_half;
            Width = (T)(plate.MaxX - plate.MinX);
            Height = (T)(plate.MaxY - plate.MinY);
        }
    }
}
