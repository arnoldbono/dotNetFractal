using dotNetFractal.WPF.ViewModels;

namespace dotNetFractal.WPF
{
    public class DisplaySettingsViewModel : BaseViewModel
    {
        private bool m_stretchImage;

        public bool StretchImage
        {
            get => m_stretchImage;
            set
            {
                if (m_stretchImage == value)
                {
                    return;
                }

                m_stretchImage = value;
                OnPropertyChanged();
            }
        }

        public DisplaySettingsViewModel()
        {
            StretchImage = false;
        }
    }
}
