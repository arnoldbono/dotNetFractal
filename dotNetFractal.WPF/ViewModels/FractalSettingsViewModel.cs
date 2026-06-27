namespace dotNetFractal.WPF.ViewModels
{
    public class FractalSettingsViewModel : BaseViewModel
    {
        private int m_maxIterations = 1024;
        private int m_maxColorSteps = 128;
        private bool m_smoothColoring = true;
        private bool m_highPrecision = false;

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

        public FractalSettingsViewModel()
        {
        }
    }
}
