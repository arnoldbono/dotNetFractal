using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using dotNetFractal.Logic;
using System.Windows;
using ReactiveUI;

namespace dotNetFractal.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static readonly FractalDecimal m_half = (FractalDecimal)0.5;

        private RelayCommand<EventArgs> m_newFractalCommand;
        private RelayCommand<EventArgs> m_saveAsCommand;
        private RelayCommand<EventArgs> m_copyCommand;
        private RelayCommand<EventArgs> m_goBackCommand;
        private RelayCommand<EventArgs> m_goForwardCommand;
        private RelayCommand<EventArgs> m_toggleStretchImageCommand;
        private RelayCommand<EventArgs> m_toggleFullScreenCommand;
        private RelayCommand<EventArgs> m_togglePropertiesPanelCommand;
        private RelayCommand<EventArgs> m_collapsePropertiesCommand;
        private RelayCommand<EventArgs> m_hidePropertiesCommand;
        private RelayCommand<EventArgs> m_stopSelectionCommand;

        private ImageResolutionViewModel m_imageResolution = new();
        private FractalAreaViewModel m_fractalArea = new();
        private FractalSettingsViewModel m_fractalSettings = new();
        private ColorMapViewModel m_colorMap = new();
        private DisplaySettingsViewModel m_displaySettings = new();
        private PropertiesPanelViewModel m_propertiesPanel = default;

        private FractalStitcher m_stitcher;
        private readonly FractalReplay m_fractalReplay = new();
        private int m_currentHistoryIndex = -1;
        private bool m_isNavigating = false;

        private Thread m_updateWorkerThread;
        private volatile bool m_stopWorkerThread;
        private readonly Dispatcher m_dispatcher;
        private Bitmap m_bitmap;
        private ImageSource m_mainImageSource;
        private int m_width;
        private int m_height;
        private bool m_isFullScreen;
        private WindowStyle m_windowStyle = WindowStyle.SingleBorderWindow;
        private WindowState m_windowState = WindowState.Normal;
        private bool m_isPropertiesPanelVisible = true;
        private bool m_arePropertiesExpanded = true;
        private System.Windows.Point? m_selectionStart;
        private bool m_isSelecting;


        public ImageSource MainImage
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

        public bool StretchImage
        {
            get => m_displaySettings.StretchImage;
            set
            {
                if (m_displaySettings.StretchImage == value)
                {
                    return;
                }

                m_displaySettings.StretchImage = value;
                OnPropertyChanged();
            }
        }

        public WindowStyle WindowStyle
        {
            get => m_windowStyle;
            set
            {
                if (m_windowStyle == value)
                {
                    return;
                }

                m_windowStyle = value;
                OnPropertyChanged();
            }
        }

        public WindowState WindowState
        {
            get => m_windowState;
            set
            {
                if (m_windowState == value)
                {
                    return;
                }

                m_windowState = value;
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

        public PropertiesPanelViewModel PropertiesPanelViewModel => m_propertiesPanel;

        public System.Windows.Point? SelectionStart
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

        public MainViewModel()
        {
            m_dispatcher = Dispatcher.CurrentDispatcher;

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
                });

            // Subscribe to property changes from PropertiesPanelViewModel to keep MainViewModel in sync
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
            m_displaySettings.WhenAnyValue(x => x.StretchImage).Subscribe(_ => OnPropertyChanged(nameof(StretchImage)));

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

        public void Dispose()
        {
            StopUpdateWorkerThread();
            m_stitcher?.StopThread();
            m_bitmap?.Dispose();
        }

        public ICommand NewFractalCommand => m_newFractalCommand ??= new RelayCommand<EventArgs>(param => OnNewFractal());

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

        private void UpdateBitmap()
        {
            // Assert that this method is called on the main UI thread
            Debug.Assert(m_dispatcher?.CheckAccess() ?? true,
                "UpdateBitmap must be called on the main UI thread");

            if (m_stitcher == null) return;

            int width = m_stitcher.FractalSettings.FractalArea.DisplayArea.PixelsHorizontal;
            int height = m_stitcher.FractalSettings.FractalArea.DisplayArea.PixelsVertical;
            if ((m_bitmap == null) ||
                (m_bitmap.Width != width) ||
                (m_bitmap.Height != height))
            {
                m_bitmap = FractalStitcher.GetBitmap(width, height);
                MainImage = ConvertBitmapToImageSource.Clone(m_bitmap);
            }

            // POST: m_bitmap != null

            if ((MainImage == null) ||
                (MainImage.Width != m_bitmap.Width) ||
                (MainImage.Height != m_bitmap.Height))
            {
                MainImage = ConvertBitmapToImageSource.Clone(m_bitmap);
            }

            if (m_stitcher.Update(m_bitmap))
            {
                MainImage = ConvertBitmapToImageSource.ConvertFast(m_bitmap);
            }
        }

        private void StartUpdateWorkerThread()
        {
            m_stopWorkerThread = false;
            m_updateWorkerThread = new Thread(UpdateWorkerThreadProc)
            {
                IsBackground = true,
                Name = "BitmapUpdateWorker"
            };
            m_updateWorkerThread.Start();
        }

        private void StopUpdateWorkerThread()
        {
            if (m_updateWorkerThread != null)
            {
                m_stopWorkerThread = true;

                // Signal the event to wake up the worker thread so it can exit
                (m_stitcher?.BitmapUpdateEvent as AutoResetEvent)?.Set();

                if (m_updateWorkerThread.IsAlive)
                {
                    m_updateWorkerThread.Join(1000); // Wait up to 1 second
                }

                m_updateWorkerThread = null;
            }
        }

        private void UpdateWorkerThreadProc()
        {
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
                        m_stitcher.HasFractalsToUpdate)
                    {
                        if (m_stopWorkerThread)
                            break;

                        // Call UpdateBitmap on the UI thread
                        m_dispatcher.Invoke(UpdateBitmap);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in UpdateWorkerThread: {ex.Message}");
                }
            }
        }

        public void OnNewFractal()
        {
            m_imageResolution = new();
            m_fractalArea = new();
            m_fractalSettings = new();
            m_stitcher?.StopThread();
            m_fractalReplay.ClearHistory();
            m_currentHistoryIndex = 0;
            m_stitcher = null;

            var centerX = m_fractalArea.CenterX;
            var centerY = m_fractalArea.CenterY;
            var width = m_fractalArea.Width;
            var height = m_fractalArea.Height;

            StartFractalComputation(false, centerX, centerY, width, height);
        }

        private void OnApplyFractalArea()
        {
            var centerX = m_fractalArea.CenterX;
            var centerY = m_fractalArea.CenterY;
            var width = m_fractalArea.Width;
            var height = m_fractalArea.Height;

            StartFractalComputation(m_fractalArea.JuliaSet, centerX, centerY, width, height);
        }

        public void OnToggleStretchImage()
        {
            StretchImage = !StretchImage;
        }

        public void OnToggleFullScreen()
        {
            IsFullScreen = !IsFullScreen;

            if (IsFullScreen)
            {
                // Enter full screen
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
            else
            {
                // Exit full screen
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
            }
        }

        private void OnTogglePropertiesPanel()
        {
            if (!IsPropertiesPanelVisible)
            {
                // When showing the panel, always show it expanded
                IsPropertiesPanelVisible = true;
                ArePropertiesExpanded = true;
            }
            else
            {
                // When hiding, just toggle visibility
                IsPropertiesPanelVisible = false;
            }
        }

        private void OnCollapseProperties()
        {
            ArePropertiesExpanded = !ArePropertiesExpanded;
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

        public void OnCopy()
        {
            Clipboard.SetImage((System.Windows.Media.Imaging.BitmapSource)MainImage);
        }

        public void OnSaveAs()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp",
                Title = "Save an Image File"
            };

            if (saveFileDialog.ShowDialog() == true && !string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                m_stitcher.LockMutex();
                // Saves the Image via a FileStream created by the OpenFile method.
                using (var fs = (FileStream)saveFileDialog.OpenFile())
                {
                    // Saves the Image in the appropriate ImageFormat based upon the
                    // File type selected in the dialog box.
                    // NOTE that the FilterIndex property is one-based.
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            m_bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;

                        case 2:
                            m_bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;

                        case 3:
                            m_bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                    }

                    fs.Close();
                }
                m_stitcher.UnlockMutex();
            }
        }

        public void ComputeJuliaSet(double pixelX1, double pixelY1, double pixelX2, double pixelY2, double imageWidth, double imageHeight)
        {
            if (m_fractalArea == null || imageWidth == 0 || imageHeight == 0)
                return;

            var displayArea = m_fractalArea.GetDisplayArea((int)imageWidth, (int)imageHeight);

            var displayAreaTyped = displayArea as DisplayArea<FractalDecimal> ?? throw new InvalidOperationException("Unsupported display area type.");

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

            var displayAreaTyped = displayArea as DisplayArea<FractalDecimal> ?? throw new InvalidOperationException("Unsupported display area type.");

            // Update the fractal area
            var centerX = displayAreaTyped.GetCenterX((int)pixelX1, (int)pixelX2);
            var centerY = displayAreaTyped.GetCenterY((int)pixelY1, (int)pixelY2);
            var width = displayAreaTyped.GetWidth((int)pixelX1, (int)pixelX2);
            var height = displayAreaTyped.GetHeight((int)pixelY1, (int)pixelY2);

            // Regenerate the fractal with the new area
            StartFractalComputation(m_fractalArea.JuliaSet, centerX, centerY, width, height);
        }

        public void ZoomOutFromRectangle(double pixelX1, double pixelY1, double pixelX2, double pixelY2, double imageWidth, double imageHeight)
        {
            if (m_fractalArea == null || imageWidth == 0 || imageHeight == 0)
                return;

            var displayArea = m_fractalArea.GetDisplayArea((int)imageWidth, (int)imageHeight);
            var displayAreaTyped = displayArea as DisplayArea<FractalDecimal> ?? throw new InvalidOperationException("Unsupported display area type.");

            // Get the center of the selected rectangle in fractal coordinates
            var centerPixelX = (FractalDecimal)(pixelX1 + pixelX2) * m_half;
            var centerPixelY = (FractalDecimal)(pixelY1 + pixelY2) * m_half;
            var centerFractalX = displayAreaTyped.GetX(FractalDecimal.Floor(centerPixelX));
            var centerFractalY = displayAreaTyped.GetY(FractalDecimal.Floor(centerPixelY));

            // Calculate the zoom-out ratio based on the rectangle size
            var rectWidth = (FractalDecimal)Math.Abs(pixelX2 - pixelX1);
            var rectHeight = (FractalDecimal)Math.Abs(pixelY2 - pixelY1);
            var widthRatio = (FractalDecimal)imageWidth / rectWidth;
            var heightRatio = (FractalDecimal)imageHeight / rectHeight;
            var zoomOutRatio = FractalDecimal.Min(widthRatio, heightRatio);

            // Calculate the new fractal area dimensions (zoomed out)
            var newWidth = m_fractalArea.Width * zoomOutRatio;
            var newHeight = m_fractalArea.Height * zoomOutRatio;

            // Set the new fractal area centered on the selection rectangle center
            StartFractalComputation(m_fractalArea.JuliaSet, centerFractalX, centerFractalY, newWidth, newHeight);
        }

        private bool CanGoBack()
        {
            return m_currentHistoryIndex > 0;
        }

        private bool CanGoForward()
        {
            return m_currentHistoryIndex >= 0 && m_currentHistoryIndex < m_fractalReplay.HistoryCount - 1;
        }

        public void OnGoBack()
        {
            if (!CanGoBack())
                return;

            NavigateToHistoryIndex(--m_currentHistoryIndex);
        }

        public void OnGoForward()
        {
            if (!CanGoForward())
                return;

            NavigateToHistoryIndex(++m_currentHistoryIndex);
        }

        private void NavigateToHistoryIndex(int index)
        {
            if (index < 0 || index >= m_fractalReplay.HistoryCount)
                return;

            var displayArea = m_fractalReplay[index];
            if (displayArea == null)
                return;

            // Set flag to prevent adding to history during navigation
            m_isNavigating = true;

            try
            {
                // Regenerate the fractal with the historical area
                var displayAreaTyped = displayArea as DisplayArea<FractalDecimal> ?? throw new InvalidOperationException("Unsupported display area type.");
                StartFractalComputation(m_fractalArea.JuliaSet, displayAreaTyped.CenterX, displayAreaTyped.CenterY, displayAreaTyped.Width, displayAreaTyped.Height);
            }
            finally
            {
                m_isNavigating = false;
            }
        }

        private void StartFractalComputation(bool juliaSet, FractalDecimal centerX, FractalDecimal centerY, FractalDecimal width, FractalDecimal height)
        {
            var previousFractalArea = m_fractalArea.Clone();

            m_fractalArea.JuliaSet = juliaSet;
            m_fractalArea.CenterX = centerX;
            m_fractalArea.CenterY = centerY;
            m_fractalArea.Width = width;
            m_fractalArea.Height = height;

            StartFractalComputation(m_fractalArea.JuliaSet, previousFractalArea);
        }

        private void StartFractalComputation(bool juliaSet, FractalAreaViewModel oldFractalArea)
        {
            m_stitcher?.StopThread();

            Width = m_imageResolution.Width;
            Height = m_imageResolution.Height;

            IDisplayArea displayArea;
            displayArea = m_fractalArea.GetDisplayArea(Width, Height);

            // Only add to history if not navigating
            if (!m_isNavigating)
            {
                // Remove any forward history if we're creating a new fractal
                if (m_currentHistoryIndex >= 0 && m_currentHistoryIndex < m_fractalReplay.HistoryCount - 1)
                {
                    m_fractalReplay.RemoveAllFromIndex(m_currentHistoryIndex);
                }

                m_currentHistoryIndex = m_fractalReplay.Add(displayArea);
            }

            var fractalSettings = new FractalSettings(displayArea,
                m_fractalSettings.MaxIterations,
                m_fractalSettings.MaxColorSteps,
                m_fractalSettings.SmoothColoring,
                m_fractalSettings.HighPrecision
            );

            m_stitcher = new FractalStitcher(fractalSettings);
            fractalSettings.FractalArea.JuliaSet = juliaSet;

            if (MainImage != null)
            {
                var oldBitmap = m_bitmap;
                m_bitmap = FractalStitcher.GetBitmap(Width, Height);
                if (oldBitmap != null)
                {
                    if (oldFractalArea.JuliaSet == m_fractalArea.JuliaSet &&
                        oldFractalArea.Width >= m_fractalArea.Width &&
                        oldFractalArea.Height >= m_fractalArea.Height)
                    {
                        // Map the section from the m_oldBitmap described with the dimension from oldFractalArea
                        // to m_bitmap that has the dimensions of m_fractalArea. This is a zoom operation.

                        var newLeft = m_fractalArea.CenterX - m_fractalArea.Width * m_half;
                        var newTop = m_fractalArea.CenterY + m_fractalArea.Height * m_half;

                        var oldLeft = oldFractalArea.CenterX - oldFractalArea.Width * m_half;
                        var oldTop = oldFractalArea.CenterY + oldFractalArea.Height * m_half;

                        // Convert new area bounds to pixel coordinates in the old bitmap
                        var sourceX = (double)((newLeft - oldLeft) / oldFractalArea.Width) * m_bitmap.Width;
                        double sourceY = (double)((oldTop - newTop) / oldFractalArea.Height) * m_bitmap.Height;
                        double sourceWidth = (double)(m_fractalArea.Width / oldFractalArea.Width) * m_bitmap.Width;
                        double sourceHeight = (double)(m_fractalArea.Height / oldFractalArea.Height) * m_bitmap.Height;

                        // Create source and destination rectangles
                        var sourceRect = new RectangleF((float)sourceX, (float)sourceY, (float)sourceWidth, (float)sourceHeight);
                        var destRect = new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height);

                        // Perform the zoom operation using Graphics.DrawImage
                        using (Graphics grfx = Graphics.FromImage(m_bitmap))
                        {
                            grfx.DrawImage(oldBitmap, destRect, sourceRect, GraphicsUnit.Pixel);
                        }
                    }
                }
                MainImage = ConvertBitmapToImageSource.ConvertFast(m_bitmap);
            }

            m_stitcher.StartThread();

            // Update command states
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
