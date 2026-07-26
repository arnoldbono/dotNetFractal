using System.Windows.Input;
using dotNetFractal.UI.Commands;
using dotNetFractal.UI.Services;
using SkiaSharp;

namespace dotNetFractal.UI.ViewModels;

public class PropertiesPanelViewModel : BaseViewModel
{
    private readonly Action? m_onRunColorist;
    private readonly Action? m_onApplyChanges;
    private readonly Action m_showDistributionGraph = null!;
    private RelayCommand<EventArgs>? m_runColoristCommand;
    private RelayCommand<EventArgs>? m_applyCommand;
    private RelayCommand<EventArgs>? m_collapsePropertiesCommand;
    private RelayCommand<EventArgs>? m_hidePropertiesCommand;

    private bool m_isPropertiesPanelVisible = true;
    private bool m_arePropertiesExpanded = true;
    private bool m_isFullScreen;

    private FractalAreaViewModel m_fractalArea;
    private ImageResolutionViewModel m_imageResolution;
    private ColorMapViewModel m_colorMap;
    private DisplaySettingsViewModel m_displaySettings;
    private FractalSettingsViewModel m_fractalSettings;

    public bool IsPropertiesPanelVisible
    {
        get => m_isPropertiesPanelVisible;
        set
        {
            if (m_isPropertiesPanelVisible == value)
            {
                return;
            }

            m_isPropertiesPanelVisible = value;
            OnPropertyChanged();
        }
    }

    public bool ArePropertiesExpanded
    {
        get => m_arePropertiesExpanded;
        set
        {
            if (m_arePropertiesExpanded == value)
            {
                return;
            }

            m_arePropertiesExpanded = value;
            OnPropertyChanged();
        }
    }

    public bool IsFullScreen
    {
        get => m_isFullScreen;
        set
        {
            if (m_isFullScreen == value)
            {
                return;
            }

            m_isFullScreen = value;
            OnPropertyChanged();
        }
    }

    public FractalAreaViewModel FractalAreaViewModel
    {
        get => m_fractalArea;
        set
        {
            if (m_fractalArea == value)
            {
                return;
            }

            m_fractalArea = value;
            OnPropertyChanged();
        }
    }

    public ImageResolutionViewModel ImageResolutionViewModel
    {
        get => m_imageResolution;
        set
        {
            if (m_imageResolution == value)
            {
                return;
            }

            m_imageResolution = value;
            OnPropertyChanged();
        }
    }

    public ColorMapViewModel ColorMapViewModel
    {
        get => m_colorMap;
        set
        {
            if (m_colorMap == value)
            {
                return;
            }

            m_colorMap = value;
            OnPropertyChanged();
        }
    }

    public DisplaySettingsViewModel DisplaySettingsViewModel
    {
        get => m_displaySettings;
        set
        {
            if (m_displaySettings == value)
            {
                return;
            }

            m_displaySettings = value;
            OnPropertyChanged();
        }
    }

    public FractalSettingsViewModel FractalSettingsViewModel
    {
        get => m_fractalSettings;
        set
        {
            if (m_fractalSettings == value)
            {
                return;
            }

            m_fractalSettings = value;
            OnPropertyChanged();
        }
    }

    public ICommand ApplyChangesCommand => m_applyCommand ??= new RelayCommand<EventArgs>(param => OnApplyChanges());

    public ICommand RunColoristCommand => m_runColoristCommand ??= new RelayCommand<EventArgs>(param => OnRunColorist());

    public ICommand ShowDistributionGraphCommand => new RelayCommand<EventArgs>(param => OnShowDistributionGraph());

    public ICommand CollapsePropertiesCommand => m_collapsePropertiesCommand ??= new RelayCommand<EventArgs>(param => OnCollapseProperties());

    public ICommand HidePropertiesCommand => m_hidePropertiesCommand ??= new RelayCommand<EventArgs>(param => OnHideProperties());

    class FakeBitmapConverter : IBitmapConverter
    {
        public object ConvertToImageSource(SKBitmap bitmap)
        {
            return null!;
        }

        public bool TryUpdateImageSource(object? imageSource, SKBitmap bitmap)
        {
            return false;
        }
    }

    public PropertiesPanelViewModel(
        FractalAreaViewModel fractalAreaViewModel,
        ImageResolutionViewModel imageResolutionViewModel,
        ColorMapViewModel colorMapViewModel,
        DisplaySettingsViewModel displaySettingsViewModel,
        FractalSettingsViewModel fractalSettingsViewModel,
        Action onApplyChanges,
        Action onRunColorist,
        Action showDistributionGraph)
    {
        m_fractalArea = fractalAreaViewModel ?? throw new ArgumentNullException(nameof(fractalAreaViewModel));
        m_imageResolution = imageResolutionViewModel ?? throw new ArgumentNullException(nameof(imageResolutionViewModel));
        m_colorMap = colorMapViewModel ?? throw new ArgumentNullException(nameof(colorMapViewModel));
        m_displaySettings = displaySettingsViewModel ?? throw new ArgumentNullException(nameof(displaySettingsViewModel));
        m_fractalSettings = fractalSettingsViewModel ?? throw new ArgumentNullException(nameof(fractalSettingsViewModel));
        m_onApplyChanges = onApplyChanges ?? throw new ArgumentNullException(nameof(onApplyChanges));
        m_onRunColorist = onRunColorist ?? throw new ArgumentNullException(nameof(onRunColorist));
        m_showDistributionGraph = showDistributionGraph ?? throw new ArgumentNullException(nameof(showDistributionGraph));
    }

    private void OnRunColorist()
    {
        // Delegate to MainViewModel to start fractal computation
        m_onRunColorist?.Invoke();
    }

    private void OnApplyChanges()
    {
        // Delegate to MainViewModel to start fractal computation
        m_onApplyChanges?.Invoke();
    }

    private void OnShowDistributionGraph()
    {
        // Delegate to MainViewModel to show distribution graph
        m_showDistributionGraph.Invoke();
    }

    private void OnCollapseProperties()
    {
        ArePropertiesExpanded = !ArePropertiesExpanded;
    }

    private void OnHideProperties()
    {
        IsPropertiesPanelVisible = false;
    }
}
