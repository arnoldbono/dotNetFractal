using System;
using System.Diagnostics;
using System.Windows.Input;
using dotNetFractal.Logic;
using dotNetFractal.UI.Commands;
using dotNetFractal.UI.Models;
using dotNetFractal.UI.Services;
using ReactiveUI;
using SkiaSharp;

namespace dotNetFractal.UI.ViewModels;

/// <summary>
/// Shared MainViewModel implementation for all platforms (WPF, Uno, etc.).
/// Platform-specific concerns are injected via constructor dependencies.
/// </summary>
public class SharedMainViewModel : BaseViewModel, IDisposable
{
    private RelayCommand<EventArgs>? m_newFractalCommand;
    private RelayCommand<EventArgs>? m_openDnfCommand;
    private RelayCommand<EventArgs>? m_saveDnfCommand;
    private RelayCommand<EventArgs>? m_saveAsCommand;
    private RelayCommand<EventArgs>? m_copyCommand;
    private RelayCommand<EventArgs>? m_goBackCommand;
    private RelayCommand<EventArgs>? m_goForwardCommand;
    private RelayCommand<EventArgs>? m_toggleStretchImageCommand;
    private RelayCommand<EventArgs>? m_toggleFullScreenCommand;
    private RelayCommand<EventArgs>? m_togglePropertiesPanelCommand;
    private RelayCommand<EventArgs>? m_collapsePropertiesCommand;
    private RelayCommand<EventArgs>? m_hidePropertiesCommand;
    private RelayCommand<EventArgs>? m_stopSelectionCommand;

    private ImageResolutionViewModel m_imageResolution = new();
    private FractalAreaViewModel m_fractalArea = new();
    private FractalSettingsViewModel m_fractalSettings = new();
    private readonly ColorMapViewModel m_colorMap;
    private readonly DisplaySettingsViewModel m_displaySettings = new();
    private readonly PropertiesPanelViewModel m_propertiesPanel;

    private FractalStitcher? m_stitcher;
    private readonly FractalReplay m_fractalReplay = new();
    private int m_currentHistoryIndex = -1;
    private bool m_isNavigating = false;

    // Platform-specific injected services
    private readonly IDispatcherAdapter m_dispatcherAdapter;
    private readonly IBitmapConverter m_bitmapConverter;
    private readonly IFileDialogService m_fileDialogService;
    private readonly IClipboardService m_clipboardService;
    private readonly IWindowManager m_windowManager;
    private readonly IDistributionGraphService m_graphService;

    private Thread? m_updateWorkerThread;
    private volatile bool m_stopWorkerThread;
    private SKBitmap? m_bitmap;
    private object? m_mainImageSource; // Platform-agnostic object representation
    private int m_width;
    private int m_height;
    private bool m_isFullScreen;
    private bool m_isPropertiesPanelVisible = true;
    private bool m_arePropertiesExpanded = true;
    private (double x, double y)? m_selectionStart;
    private bool m_isSelecting;
    private double m_computationProgress;
    private bool m_isComputing;
    private bool m_stretchImage;

    /// <summary>
    /// Initializes a new instance of SharedMainViewModel with all platform-specific dependencies.
    /// </summary>
    public SharedMainViewModel(
        IDispatcherAdapter dispatcherAdapter,
        IBitmapConverter bitmapConverter,
        IFileDialogService fileDialogService,
        IClipboardService clipboardService,
        IWindowManager windowManager,
        IDistributionGraphService graphService)
    {
        m_dispatcherAdapter = dispatcherAdapter ?? throw new ArgumentNullException(nameof(dispatcherAdapter));
        m_bitmapConverter = bitmapConverter ?? throw new ArgumentNullException(nameof(bitmapConverter));
        m_fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
        m_clipboardService = clipboardService ?? throw new ArgumentNullException(nameof(clipboardService));
        m_windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        m_graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));

        // Create ColorMapViewModel with injected bitmap converter
        m_colorMap = new ColorMapViewModel(m_bitmapConverter);

        // Initialize PropertiesPanelViewModel with child view models and callback
        m_propertiesPanel = new PropertiesPanelViewModel(
            m_fractalArea,
            m_imageResolution,
            m_colorMap,
            m_displaySettings,
            m_fractalSettings,
            juliaSet =>
            {
                var centerX = m_fractalArea.CenterX;
                var centerY = m_fractalArea.CenterY;
                var width = m_fractalArea.Width;
                var height = m_fractalArea.Height;
                StartFractalComputation(juliaSet, centerX, centerY, width, height);
            },
            () => OnShowDistributionGraph());

        // Subscribe to property changes from PropertiesPanelViewModel to keep SharedMainViewModel in sync
        m_propertiesPanel.WhenAnyValue(x => x.IsPropertiesPanelVisible)
            .Subscribe(value =>
            {
                if (m_isPropertiesPanelVisible != value)
                {
                    m_isPropertiesPanelVisible = value;
                    OnPropertyChanged(nameof(IsPropertiesPanelVisible));
                }
            });

        m_propertiesPanel.WhenAnyValue(x => x.ArePropertiesExpanded)
            .Subscribe(value =>
            {
                if (m_arePropertiesExpanded != value)
                {
                    m_arePropertiesExpanded = value;
                    OnPropertyChanged(nameof(ArePropertiesExpanded));
                }
            });

        // Subscribe to StretchImage changes from the DisplaySettingsViewModel
        m_displaySettings.WhenAnyValue(x => x.StretchImage).Subscribe(value =>
        {
            if (m_stretchImage != value)
            {
                m_stretchImage = value;
                OnPropertyChanged(nameof(StretchImage));
            }
        });

        // Sync with PropertiesPanelViewModel
        this.WhenAnyValue(x => x.IsFullScreen).Subscribe(value =>
        {
            if (m_propertiesPanel != null && m_propertiesPanel.IsFullScreen != value)
            {
                m_propertiesPanel.IsFullScreen = value;
            }
        });
        this.WhenAnyValue(x => x.IsPropertiesPanelVisible).Subscribe(value =>
        {
            if (m_propertiesPanel != null && m_propertiesPanel.IsPropertiesPanelVisible != value)
            {
                m_propertiesPanel.IsPropertiesPanelVisible = value;
            }
        });
        this.WhenAnyValue(x => x.ArePropertiesExpanded).Subscribe(value =>
        {
            if (m_propertiesPanel != null && m_propertiesPanel.ArePropertiesExpanded != value)
            {
                m_propertiesPanel.ArePropertiesExpanded = value;
            }
        });

        StartUpdateWorkerThread();

        var centerX = m_fractalArea.CenterX;
        var centerY = m_fractalArea.CenterY;
        var width = m_fractalArea.Width;
        var height = m_fractalArea.Height;
        StartFractalComputation(false, centerX, centerY, width, height);
    }

    /// <summary>
    /// Gets the main image source for binding to the UI.
    /// </summary>
    public object? MainImage
    {
        get => m_mainImageSource;
        set
        {
            if (ReferenceEquals(m_mainImageSource, value))
            {
                return;
            }

            m_mainImageSource = value;
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
            m_windowManager.IsFullScreen = value;
            OnPropertyChanged();
        }
    }

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

    /// <summary>
    /// Gets or sets the selection start point as a platform-agnostic tuple.
    /// Platform-specific views can convert to/from their Point types as needed.
    /// </summary>
    public (double x, double y)? SelectionStart
    {
        get => m_selectionStart;
        set
        {
            if (m_selectionStart == value)
            {
                return;
            }

            m_selectionStart = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelecting
    {
        get => m_isSelecting;
        set
        {
            if (m_isSelecting == value)
            {
                return;
            }

            m_isSelecting = value;
            OnPropertyChanged();
        }
    }

    public double ComputationProgress
    {
        get => m_computationProgress;
        set
        {
            if (Math.Abs(m_computationProgress - value) < 0.01)
            {
                return;
            }

            m_computationProgress = value;
            OnPropertyChanged();
        }
    }

    public bool IsComputing
    {
        get => m_isComputing;
        set
        {
            if (m_isComputing == value)
            {
                return;
            }

            m_isComputing = value;
            OnPropertyChanged();
        }
    }

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
            m_displaySettings.StretchImage = value;
            OnPropertyChanged();
        }
    }

    public ImageResolutionViewModel ImageResolution => m_imageResolution;
    public FractalAreaViewModel FractalArea => m_fractalArea;
    public FractalSettingsViewModel FractalSettings => m_fractalSettings;
    public ColorMapViewModel ColorMap => m_colorMap;
    public DisplaySettingsViewModel DisplaySettings => m_displaySettings;
    public PropertiesPanelViewModel PropertiesPanel => m_propertiesPanel;

    public void Dispose()
    {
        StopUpdateWorkerThread();
        m_stitcher?.StopThread();
        m_bitmap?.Dispose();
        m_graphService?.CloseGraph();
    }

    #region Commands

    public ICommand NewFractalCommand => m_newFractalCommand ??= new RelayCommand<EventArgs>(param => OnNewFractal());
    public ICommand OpenDnfCommand => m_openDnfCommand ??= new RelayCommand<EventArgs>(param => OnOpenDnf());
    public ICommand SaveDnfCommand => m_saveDnfCommand ??= new RelayCommand<EventArgs>(param => OnSaveDnf());
    public ICommand SaveAsCommand => m_saveAsCommand ??= new RelayCommand<EventArgs>(param => OnSaveAs());
    public ICommand CopyCommand => m_copyCommand ??= new RelayCommand<EventArgs>(param => OnCopy());
    public ICommand GoBackCommand => m_goBackCommand ??= new RelayCommand<EventArgs>(param => OnGoBack(), param => CanGoBack());
    public ICommand GoForwardCommand => m_goForwardCommand ??= new RelayCommand<EventArgs>(param => OnGoForward(), param => CanGoForward());
    public ICommand ToggleStretchImageCommand => m_toggleStretchImageCommand ??= new RelayCommand<EventArgs>(param => OnToggleStretchImage());
    public ICommand ToggleFullScreenCommand => m_toggleFullScreenCommand ??= new RelayCommand<EventArgs>(param => OnToggleFullScreen());
    public ICommand TogglePropertiesPanelCommand => m_togglePropertiesPanelCommand ??= new RelayCommand<EventArgs>(param => OnTogglePropertiesPanel());
    public ICommand CollapsePropertiesCommand => m_collapsePropertiesCommand ??= new RelayCommand<EventArgs>(param => OnCollapseProperties());
    public ICommand HidePropertiesCommand => m_hidePropertiesCommand ??= new RelayCommand<EventArgs>(param => OnHideProperties());
    public ICommand StopSelectionCommand => m_stopSelectionCommand ??= new RelayCommand<EventArgs>(param => OnStopSelection());

    #endregion

    #region Core UI Methods

    private void UpdateBitmap()
    {
        // Assert that this method is called on the main UI thread
        Debug.Assert(m_dispatcherAdapter?.IsOnUIThread ?? true,
            "UpdateBitmap must be called on the main UI thread");

        if (m_stitcher == null)
            return;

        int width = m_stitcher.FractalSettings.FractalArea.DisplayArea.PixelsHorizontal;
        int height = m_stitcher.FractalSettings.FractalArea.DisplayArea.PixelsVertical;
        if ((m_bitmap == null) ||
            (m_bitmap.Width != width) ||
            (m_bitmap.Height != height))
        {
            m_bitmap = FractalStitcher.GetBitmap(width, height);
            MainImage = m_bitmapConverter.ConvertToImageSource(m_bitmap);
        }

        // POST: m_bitmap != null

        if ((MainImage == null) ||
            (MainImage != null && (MainImage as dynamic)?.Width != m_bitmap.Width) ||
            (MainImage != null && (MainImage as dynamic)?.Height != m_bitmap.Height))
        {
            MainImage = m_bitmapConverter.ConvertToImageSource(m_bitmap);
        }

        if (m_stitcher.Update(m_bitmap))
        {
            MainImage = m_bitmapConverter.ConvertToImageSource(m_bitmap);
        }

        Width = width;
        Height = height;
    }

    private void OnNewFractal()
    {
        m_imageResolution = new();
        m_fractalArea = new();
        m_fractalSettings = new();
        m_stitcher?.StopThread();
        m_fractalReplay.ClearHistory();
        m_currentHistoryIndex = 0;
        m_stitcher = null;

        // Close the distribution graph window when creating a new fractal
        m_graphService?.CloseGraph();

        var centerX = m_fractalArea.CenterX;
        var centerY = m_fractalArea.CenterY;
        var width = m_fractalArea.Width;
        var height = m_fractalArea.Height;

        StartFractalComputation(false, centerX, centerY, width, height);
    }

    public void OnCopy()
    {
        if (MainImage != null && m_bitmap != null)
        {
            using (var image = SKImage.FromBitmap(m_bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                m_clipboardService.SetImage(data.ToArray(), "png");
            }
        }
    }

    public void OnSaveAs()
    {
        const string filter = "PNG Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp";
        const string title = "Save an Image File";

        if (m_stitcher == null || m_bitmap == null)
        {
            m_fileDialogService.ShowMessage("No fractal image available to save.", "Error", MessageBoxType.Error);
            return;
        }

        var filePath = m_fileDialogService.ShowSaveFileDialog(filter, title);
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        m_stitcher.LockMutex();
        try
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                // Determine format based on file extension
                var ext = Path.GetExtension(filePath).ToLower();
                var format = ext switch
                {
                    ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
                    ".bmp" => SKEncodedImageFormat.Bmp,
                    ".png" or _ => SKEncodedImageFormat.Png
                };

                using (var image = SKImage.FromBitmap(m_bitmap))
                using (var data = image.Encode(format, 100))
                {
                    data.SaveTo(fs);
                }
            }
        }
        catch (Exception ex)
        {
            m_fileDialogService.ShowMessage($"Failed to save image: {ex.Message}", "Error", MessageBoxType.Error);
        }
        finally
        {
            m_stitcher.UnlockMutex();
        }
    }

    public void OnOpenDnf()
    {
        const string filter = "DotNet Fractal Files|*.dnf|All Files|*.*";
        const string title = "Open a DotNet Fractal File";

        var filePath = m_fileDialogService.ShowOpenFileDialog(filter, title);
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        try
        {
            var jsonString = File.ReadAllText(filePath);
            var data = System.Text.Json.JsonSerializer.Deserialize<DisplayAreaData>(jsonString);

            if (data == null)
            {
                m_fileDialogService.ShowMessage("Failed to load fractal data from file.", "Error", MessageBoxType.Error);
                return;
            }

            // Parse string values to decimal
            if (!decimal.TryParse(data.CenterX, out var centerX) ||
                !decimal.TryParse(data.CenterY, out var centerY) ||
                !decimal.TryParse(data.Width, out var width) ||
                !decimal.TryParse(data.Height, out var height) ||
                !decimal.TryParse(data.Cx, out var cx) ||
                !decimal.TryParse(data.Cy, out var cy))
            {
                if (!double.TryParse(data.CenterX, out var dblCenterX) ||
                    !double.TryParse(data.CenterY, out var dblCenterY) ||
                    !double.TryParse(data.Width, out var dblWidth) ||
                    !double.TryParse(data.Height, out var dblHeight) ||
                    !double.TryParse(data.Cx, out var dblCx) ||
                    !double.TryParse(data.Cy, out var dblCy))
                {
                    m_fileDialogService.ShowMessage("Invalid numeric values in fractal file.", "Error", MessageBoxType.Error);
                    return;
                }

                centerX = decimal.CreateChecked(dblCenterX);
                centerY = decimal.CreateChecked(dblCenterY);
                width = decimal.CreateChecked(dblWidth);
                height = decimal.CreateChecked(dblHeight);
                cx = decimal.CreateChecked(dblCx);
                cy = decimal.CreateChecked(dblCy);
            }

            // Update resolution
            m_imageResolution.SelectedResolution = ResolutionEnum.Custom;
            m_imageResolution.Width = data.PixelsHorizontal;
            m_imageResolution.Height = data.PixelsVertical;

            // Update fractal settings
            m_fractalSettings.MaxIterations = data.MaxIterations;
            m_fractalSettings.MaxColorSteps = data.MaxColorSteps;
            m_fractalSettings.FirstColorStep = data.FirstColorStep;
            m_fractalSettings.SmoothColoring = data.SmoothColoring;
            m_fractalSettings.HighPrecision = data.HighPrecision;

            // Determine if it's a Julia set
            bool isJuliaSet = data.FractalType?.Equals("JuliaSet", StringComparison.OrdinalIgnoreCase) == true;

            // Update fractal area
            m_fractalArea.JuliaSet = isJuliaSet;
            m_fractalArea.CenterX = centerX;
            m_fractalArea.CenterY = centerY;
            m_fractalArea.Width = width;
            m_fractalArea.Height = height;
            m_fractalArea.Cx = cx;
            m_fractalArea.Cy = cy;

            // Start computation with the loaded data
            StartFractalComputation(isJuliaSet, m_fractalArea);
        }
        catch (Exception ex)
        {
            m_fileDialogService.ShowMessage($"Failed to open fractal file: {ex.Message}", "Error", MessageBoxType.Error);
        }
    }

    public void OnSaveDnf()
    {
        if (m_stitcher?.FractalSettings?.FractalArea == null)
        {
            m_fileDialogService.ShowMessage("No fractal data to save.", "Warning", MessageBoxType.Warning);
            return;
        }

        const string filter = "DotNet Fractal Files|*.dnf";
        const string title = "Save DotNet Fractal File";

        var filePath = m_fileDialogService.ShowSaveFileDialog(filter, title);
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        try
        {
            var displayArea = m_stitcher.FractalSettings.FractalArea.DisplayArea;
            var data = new DisplayAreaData();

            // Extract data from DisplayArea based on precision type
            if (m_fractalSettings.HighPrecision && displayArea is DisplayArea<decimal> displayAreaDecimal)
            {
                data.CenterX = displayAreaDecimal.CenterX.ToString();
                data.CenterY = displayAreaDecimal.CenterY.ToString();
                data.Width = displayAreaDecimal.Width.ToString();
                data.Height = displayAreaDecimal.Height.ToString();
                data.Cx = displayAreaDecimal.Cx.ToString();
                data.Cy = displayAreaDecimal.Cy.ToString();
            }
            else if (displayArea is DisplayArea<double> displayAreaDouble)
            {
                data.CenterX = displayAreaDouble.CenterX.ToString();
                data.CenterY = displayAreaDouble.CenterY.ToString();
                data.Width = displayAreaDouble.Width.ToString();
                data.Height = displayAreaDouble.Height.ToString();
                data.Cx = displayAreaDouble.Cx.ToString();
                data.Cy = displayAreaDouble.Cy.ToString();
            }
            else
            {
                m_fileDialogService.ShowMessage("Unsupported display area type.", "Error", MessageBoxType.Error);
                return;
            }

            data.PixelsHorizontal = displayArea.PixelsHorizontal;
            data.PixelsVertical = displayArea.PixelsVertical;
            data.MaxIterations = m_fractalSettings.MaxIterations;
            data.MaxColorSteps = m_fractalSettings.MaxColorSteps;
            data.FirstColorStep = m_fractalSettings.FirstColorStep;
            data.SmoothColoring = m_fractalSettings.SmoothColoring;
            data.HighPrecision = m_fractalSettings.HighPrecision;
            data.FractalType = m_fractalArea.JuliaSet ? "JuliaSet" : "Mandelbrot";

            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jsonString = System.Text.Json.JsonSerializer.Serialize(data, options);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, jsonString);

            m_fileDialogService.ShowMessage("Fractal data saved successfully!", "Success", MessageBoxType.Information);
        }
        catch (Exception ex)
        {
            m_fileDialogService.ShowMessage($"Failed to save fractal file: {ex.Message}", "Error", MessageBoxType.Error);
        }
    }

    public void ComputeJuliaSet(double pixelX1, double pixelY1, double pixelX2, double pixelY2, double imageWidth, double imageHeight)
    {
        if (m_fractalArea == null || imageWidth == 0 || imageHeight == 0)
            return;

        var displayArea = m_fractalArea.GetDisplayArea((int)imageWidth, (int)imageHeight);

        var displayAreaTyped = displayArea as DisplayArea<decimal> ?? throw new InvalidOperationException("Unsupported display area type.");

        // Update the fractal area
        var centerX = displayAreaTyped.GetCenterX((int)pixelX1, (int)pixelX2);
        var centerY = displayAreaTyped.GetCenterY((int)pixelY1, (int)pixelY2);
        var width = displayAreaTyped.GetWidth((int)pixelX1, (int)pixelX2);
        var height = displayAreaTyped.GetHeight((int)pixelY1, (int)pixelY2);

        // Regenerate the fractal with the new area
        StartFractalComputation(true, centerX, centerY, width, height);
    }

    public void ZoomInToRectangle(double pixelX1, double pixelY1, double pixelX2, double pixelY2, double imageWidth, double imageHeight)
    {
        if (m_fractalArea == null || imageWidth == 0 || imageHeight == 0)
            return;

        var displayArea = m_fractalArea.GetDisplayArea((int)imageWidth, (int)imageHeight);

        var displayAreaTyped = displayArea as DisplayArea<decimal> ?? throw new InvalidOperationException("Unsupported display area type.");

        // Update the fractal area
        var centerX = displayAreaTyped.GetCenterX((int)pixelX1, (int)pixelX2);
        var centerY = displayAreaTyped.GetCenterY((int)pixelY1, (int)pixelY2);
        var width = displayAreaTyped.GetWidth((int)pixelX1, (int)pixelX2);
        var height = displayAreaTyped.GetHeight((int)pixelY1, (int)pixelY2);

        // Regenerate the fractal with the new area
        StartFractalComputation(false, centerX, centerY, width, height);
    }

    public void ZoomOutFromRectangle(double pixelX1, double pixelY1, double pixelX2, double pixelY2, double imageWidth, double imageHeight)
    {
        if (m_fractalArea == null || imageWidth == 0 || imageHeight == 0)
            return;

        var displayArea = m_fractalArea.GetDisplayArea((int)imageWidth, (int)imageHeight);

        var displayAreaTyped = displayArea as DisplayArea<decimal> ?? throw new InvalidOperationException("Unsupported display area type.");

        // Instead of zooming to a specific region, zoom out from the center by 2x
        var newWidth = displayAreaTyped.Width * 2;
        var newHeight = displayAreaTyped.Height * 2;

        // Regenerate the fractal with the new area
        StartFractalComputation(m_fractalArea.JuliaSet, displayAreaTyped.CenterX, displayAreaTyped.CenterY, newWidth, newHeight);
    }

    private bool CanGoBack() => m_currentHistoryIndex > 0;

    private bool CanGoForward() => m_currentHistoryIndex < m_fractalReplay.HistoryCount - 1;

    public void OnGoBack()
    {
        if (!CanGoBack())
            return;

        m_isNavigating = true;
        m_currentHistoryIndex--;
        var displayArea = m_fractalReplay[m_currentHistoryIndex];

        if (displayArea == null)
            return;

        LoadFractalState(displayArea);
        m_isNavigating = false;
    }

    public void OnGoForward()
    {
        if (!CanGoForward())
            return;

        m_isNavigating = true;
        m_currentHistoryIndex++;
        var displayArea = m_fractalReplay[m_currentHistoryIndex];

        if (displayArea == null)
            return;

        LoadFractalState(displayArea);
        m_isNavigating = false;
    }

    private void LoadFractalState(IDisplayArea displayArea)
    {
        // Create a fractal area from the display area
        if (displayArea is DisplayArea<decimal> displayAreaDecimal)
        {
            m_fractalArea.CenterX = displayAreaDecimal.CenterX;
            m_fractalArea.CenterY = displayAreaDecimal.CenterY;
            m_fractalArea.Width = displayAreaDecimal.Width;
            m_fractalArea.Height = displayAreaDecimal.Height;
            m_fractalArea.Cx = displayAreaDecimal.Cx;
            m_fractalArea.Cy = displayAreaDecimal.Cy;
        }
        else if (displayArea is DisplayArea<double> displayAreaDouble)
        {
            m_fractalArea.CenterX = decimal.CreateChecked(displayAreaDouble.CenterX);
            m_fractalArea.CenterY = decimal.CreateChecked(displayAreaDouble.CenterY);
            m_fractalArea.Width = decimal.CreateChecked(displayAreaDouble.Width);
            m_fractalArea.Height = decimal.CreateChecked(displayAreaDouble.Height);
            m_fractalArea.Cx = decimal.CreateChecked(displayAreaDouble.Cx);
            m_fractalArea.Cy = decimal.CreateChecked(displayAreaDouble.Cy);
        }

        StartFractalComputation(m_fractalArea.JuliaSet, m_fractalArea);
    }

    private void StartFractalComputation(bool juliaSet, decimal centerX, decimal centerY, decimal width, decimal height)
    {
        var fractalArea = new FractalAreaViewModel
        {
            JuliaSet = juliaSet,
            CenterX = centerX,
            CenterY = centerY,
            Width = width,
            Height = height,
            Cx = m_fractalArea.Cx,
            Cy = m_fractalArea.Cy
        };

        StartFractalComputation(juliaSet, fractalArea);
    }

    private void StartFractalComputation(bool juliaSet, FractalAreaViewModel oldFractalArea)
    {
        if (!m_isNavigating)
        {
            // Add to history when not navigating
            var displayArea = oldFractalArea.GetDisplayArea((int)m_imageResolution.Width, (int)m_imageResolution.Height);
            m_currentHistoryIndex = m_fractalReplay.Add(displayArea);
        }

        IsComputing = true;
        ComputationProgress = 0.0;

        // Create new fractal computation
        var fractalArea = oldFractalArea.GetDisplayArea((int)m_imageResolution.Width, (int)m_imageResolution.Height);

        var fractalSettings = new FractalSettings(
            fractalArea,
            m_fractalSettings.MaxIterations,
            m_fractalSettings.MaxColorSteps,
            m_fractalSettings.FirstColorStep,
            m_fractalSettings.SmoothColoring,
            m_fractalSettings.HighPrecision,
            m_fractalSettings.DistributionGraph);

        var stitcher = new FractalStitcher(fractalSettings);
        stitcher.ComputationCompleted += OnComputationCompleted;
        fractalSettings.FractalArea.JuliaSet = juliaSet;

        if (MainImage != null)
        {
            var oldBitmap = m_bitmap;
            m_bitmap = null;
            oldBitmap?.Dispose();
        }

        m_stitcher?.StopThread();
        m_stitcher = stitcher;
        m_stitcher.StartThread();
    }

    private void OnComputationCompleted(object? sender, EventArgs e)
    {
        m_dispatcherAdapter.RunOnUIThread(() =>
        {
            IsComputing = false;
        });
    }

    private void OnShowDistributionGraph()
    {
        // Show the distribution graph on the UI thread
        m_dispatcherAdapter.RunOnUIThread(() =>
        {
            if (m_graphService.IsGraphOpen && m_fractalSettings?.DistributionGraph != null)
            {
                m_graphService.ShowGraph(m_fractalSettings.DistributionGraph);
                return; // Already open and now updated
            }

            // Check if distribution graph is enabled and has data
            if (m_fractalSettings?.DistributionGraph == null)
            {
                return;
            }

            m_graphService.ShowGraph(m_fractalSettings.DistributionGraph);
        });
    }

    private void OnToggleStretchImage()
    {
        StretchImage = !StretchImage;
    }

    private void OnToggleFullScreen()
    {
        IsFullScreen = !IsFullScreen;
    }

    private void OnTogglePropertiesPanel()
    {
        IsPropertiesPanelVisible = !IsPropertiesPanelVisible;
    }

    private void OnCollapseProperties()
    {
        ArePropertiesExpanded = false;
    }

    private void OnHideProperties()
    {
        IsPropertiesPanelVisible = false;
    }

    private void OnStopSelection()
    {
        IsSelecting = false;
        SelectionStart = null;
    }

    #endregion

    #region Worker Thread

    private void StartUpdateWorkerThread()
    {
        if (m_updateWorkerThread != null)
            return;

        m_stopWorkerThread = false;
        m_updateWorkerThread = new Thread(UpdateWorkerThreadProc)
        {
            Name = "UpdateWorkerThread",
            IsBackground = true
        };
        m_updateWorkerThread.Start();
    }

    private void StopUpdateWorkerThread()
    {
        m_stopWorkerThread = true;
        if (m_updateWorkerThread != null)
        {
            m_updateWorkerThread.Join(1000);
            m_updateWorkerThread = null;
        }
    }

    private void UpdateWorkerThreadProc()
    {
        bool updating = false;
        bool updatePending = true;

        while (!m_stopWorkerThread)
        {
            try
            {
                // Wait for the FractalStitcher to signal that a fractal needs updating
                if (m_stitcher?.BitmapUpdateEvent == null)
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (m_stopWorkerThread)
                    break;

                if (m_stitcher.BitmapUpdateEvent.WaitOne(100) ||
                    m_stitcher.HasFractalsToUpdate ||
                    updatePending)
                {
                    if (m_stopWorkerThread)
                        break;

                    if (updating)
                    {
                        updatePending = true;
                        continue;
                    }

                    updatePending = false;
                    updating = true;

                    // Call UpdateBitmap on the UI thread
                    m_dispatcherAdapter.RunOnUIThreadAsync(() =>
                    {
                        UpdateBitmap();

                        // Update progress on the UI thread
                        var progress = m_stitcher?.Progress ?? 0.0;
                        ComputationProgress = progress;
                        IsComputing = progress < 100.0;

                        updating = false;
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateWorkerThread: {ex.Message}");
            }
        }
    }

    #endregion
}
