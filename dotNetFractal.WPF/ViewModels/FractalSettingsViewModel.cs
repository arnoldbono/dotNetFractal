namespace dotNetFractal.WPF.ViewModels
{
    public class FractalSettingsViewModel : BaseViewModel
    {
        private int m_maxIterations;
        private int m_maxColorSteps;
        private bool m_smoothColoring;

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

        public FractalSettingsViewModel()
        {
            MaxIterations = 4096;
            MaxColorSteps = 256;
            SmoothColoring = true;
        }
    }
}
