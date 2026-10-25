namespace dotNetFractal.UI.ViewModels;

public class FractalSettingsViewModel : BaseViewModel
{
    private int m_maxIterations = 1024;
    private int m_maxColorSteps = 128;
    private int m_firstColorStep = 0;
    private bool m_smoothColoring = true;
    private bool m_highPrecision = false;
    private bool m_useCuda = false;
    private int m_patchSize = 128;
    private int[] m_distributionGraph;

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
            m_distributionGraph = new int[m_maxIterations];
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

    public int FirstColorStep
    {
        get => m_firstColorStep;
        set
        {
            if (m_firstColorStep == value)
            {
                return;
            }

            m_firstColorStep = value;
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

            if (m_highPrecision && m_useCuda)
            {
                m_useCuda = false;
                OnPropertyChanged(nameof(UseCuda));
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(IsUseCudaEnabled));
        }
    }

    public bool UseCuda
    {
        get => m_useCuda;
        set
        {
            if (m_useCuda == value)
            {
                return;
            }

            m_useCuda = value;
            OnPropertyChanged();
        }
    }

    public bool IsUseCudaEnabled => !m_highPrecision;

    public int[] PatchSizeOptions { get; } = [128, 256, 512, 1024];

    public int PatchSize
    {
        get => m_patchSize;
        set
        {
            if (m_patchSize == value)
            {
                return;
            }

            if (value != 128 && value != 256 && value != 512 && value != 1024)
            {
                return;
            }

            m_patchSize = value;
            OnPropertyChanged();
        }
    }

    public int[] DistributionGraph
    {
        get => m_distributionGraph;
        set
        {
            if (m_distributionGraph == value)
            {
                return;
            }

            m_distributionGraph = value;
            OnPropertyChanged();
        }
    }

    public FractalSettingsViewModel()
    {
        m_distributionGraph = new int[m_maxIterations];
    }
}
