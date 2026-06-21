using System;
using dotNetFractal.WPF.ViewModels;
using ReactiveUI;

namespace dotNetFractal.WPF
{
    public class ImageResolutionViewModel : BaseViewModel
    {
        private ResolutionEnum m_selectedResolution;
        private int m_width;
        private int m_height;
        private bool m_readOnly;

        public ResolutionEnum SelectedResolution
        {
            get => m_selectedResolution;
            set
            {
                if (m_selectedResolution == value)
                {
                    return;
                }

                m_selectedResolution = value;
                OnPropertyChanged();
            }
        }

        public int Width
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

        public int Height
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

        public bool ReadOnly
        {
            get => m_readOnly;
            set
            {
                if (m_readOnly == value)
                {
                    return;
                }

                m_readOnly = value;
                OnPropertyChanged();
            }
        }

        public ImageResolutionViewModel()
        {
            SelectedResolution = ResolutionEnum.Custom;
            Width = 512;
            Height = 512;
            this.WhenAnyValue(x => x.SelectedResolution).Subscribe(OnSelectedResolution);
        }

        private void OnSelectedResolution(ResolutionEnum resolution)
        {
            switch (resolution)
            {
                case ResolutionEnum.Custom:
                    ReadOnly = false;
                    break;
                case ResolutionEnum.FullHD:
                    Width = 1920;
                    Height = 1080;
                    ReadOnly = true;
                    break;
                case ResolutionEnum.UltraHD:
                    Width = 3840;
                    Height = 2160;
                    ReadOnly = true;
                    break;
            }
        }
    }
}
