namespace dotNetFractal.WPF.ViewModels
{
    public class FractalSettingsViewModel : BaseViewModel
    {
        private int m_maxIterations = 256;
        private int m_maxColorSteps = 16;
        private bool m_smoothColoring = true;
        private bool m_highPrecision = false;
        private bool m_juliaSet = false;

        public int MaxIterations
        {
            get => m_maxIterations;
            set
            {
                if (m_maxIterations == value)
                {
                    return;
                }

                m_maxIterations = value;
                OnPropertyChanged();
            }
        }

        public int MaxColorSteps
        {
            get => m_maxColorSteps;
            set
            {
                if (m_maxColorSteps == value)
                {
                    return;
                }

                m_maxColorSteps = value;
                OnPropertyChanged();
            }
        }

        public bool SmoothColoring
        {
            get => m_smoothColoring;
            set
            {
                if (m_smoothColoring == value)
                {
                    return;
                }

                m_smoothColoring = value;
                OnPropertyChanged();
            }
        }

        public bool HighPrecision
        {
            get => m_highPrecision;
            set
            {
                if (m_highPrecision == value)
                {
                    return;
                }

                m_highPrecision = value;
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

        public FractalSettingsViewModel()
        {
        }
    }
}
