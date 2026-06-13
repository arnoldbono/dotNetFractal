using dotNetFractal.Logic;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace dotNetFractal.WPF
{
    public class FractalAreaViewModel : BaseViewModel
    {
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
        
        private double m_minX;
        private double m_minY;
        private double m_maxX;
        private double m_maxY;
        private int m_selectedPlate;

        public List<FractalPlate> Plates => m_plates;

        public DisplayArea GetDisplayArea(int width, int height)
        {
            return new DisplayArea((MaxX + MinX) / 2.0, (MaxY + MinY) / 2.0, MaxX - MinX, MaxY - MinY, width, height);
        }

        public double MinX
        {
            get => m_minX;
            set
            {
                if (m_minX == value)
                {
                    return;
                }

                m_minX = value;
                OnPropertyChanged();
            }
        }

        public double MinY
        {
            get => m_minY;
            set
            {
                if (m_minY == value)
                {
                    return;
                }

                m_minY = value;
                OnPropertyChanged();
            }
        }

        public double MaxX
        {
            get => m_maxX;
            set
            {
                if (m_maxX == value)
                {
                    return;
                }

                m_maxX = value;
                OnPropertyChanged();
            }
        }

        public double MaxY
        {
            get => m_maxY;
            set
            {
                if (m_maxY == value)
                {
                    return;
                }

                m_maxY = value;
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
            MinX = plate.MinX;
            MaxX = plate.MaxX;
            MinY = plate.MinY;
            MaxY = plate.MaxY;
        }
    }
}
