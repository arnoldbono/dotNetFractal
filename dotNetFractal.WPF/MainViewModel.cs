using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace dotNetFractal.WPF
{
    public class MainViewModel : BaseViewModel
    {
        private RelayCommand<EventArgs> m_newFractalCommand;
        private RelayCommand<EventArgs> m_imageResolutionCommand;
        private RelayCommand<EventArgs> m_fractalAreaCommand;
        private RelayCommand<EventArgs> m_saveAsCommand;

        private ImageResolutionViewModel m_imageResolution;
        private FractalAreaViewModel m_fractalArea;

        private FractalStitcher m_stitcher;

        private DispatcherTimer m_dispatcherTimer;
        private Bitmap m_bitmap;
        private ImageSource m_mainImageSource;
        private int m_width;
        private int m_height;

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


        public MainViewModel()
        {
            m_dispatcherTimer = new DispatcherTimer();
            m_dispatcherTimer.Tick += new EventHandler(UpdateBitmap);
            m_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(20);
            m_dispatcherTimer.Start();
            OnNewFractal();
        }

        public ICommand NewFractalCommand => m_newFractalCommand ??= new RelayCommand<EventArgs>(param => OnNewFractal());

        public ICommand ImageResolutionCommand => m_imageResolutionCommand ??= new RelayCommand<EventArgs>(param => OnImageResolution());

        public ICommand FractalAreaCommand => m_fractalAreaCommand ??= new RelayCommand<EventArgs>(param => OnFractalAreaCommand());

        public ICommand SaveAsCommand => m_saveAsCommand ??= new RelayCommand<EventArgs>(param => OnSaveAs());

        private void UpdateBitmap(object sender, EventArgs e)
        {
            if (m_stitcher == null) return;

            int width = m_stitcher.Area.PixelsHorizontal;
            int height = m_stitcher.Area.PixelsVertical;
            if ((m_bitmap == null) ||
                (m_bitmap.Width != width) ||
                (m_bitmap.Height != height))
            {
                m_bitmap = m_stitcher.GetBitmap(width, height);
                m_stitcher.DefaultFill(m_bitmap);
            }

            if (m_bitmap == null)
            {
                return;
            }

            if ((m_mainImageSource == null) ||
                (m_mainImageSource.Width != m_bitmap.Width) ||
                (m_mainImageSource.Height != m_bitmap.Height))
            {
                MainImage = ConvertBitmapToBitmapImage.Convert(m_bitmap);
            }

            var rect = m_stitcher.Update(m_bitmap);
            if (!rect.IsEmpty)
            {
                //Graphics grfx = Graphics.FromImage(m_mainBitmap);
                //grfx.Clip = new Region(rect);
                //grfx.DrawImage(m_bitmap, new Point(0, 0));
                MainImage = ConvertBitmapToBitmapImage.Convert(m_bitmap);
            }
        }

        public void OnNewFractal()
        {
            m_imageResolution = new ImageResolutionViewModel
            {
                SelectedResolution = ResolutionEnum.Custom,
                Width = 128,
                Height = 128
            };
            m_fractalArea = new FractalAreaViewModel();
            NewFractal();
        }

        private void NewFractal(bool force = false)
        {
            if (force || m_stitcher == null)
            {
                Width = m_imageResolution.Width;
                Height = m_imageResolution.Height;

                var displayArea = m_fractalArea.GetDisplayArea(Width, Height);

                m_stitcher =
                    new FractalStitcher(() => new FractalMandelbrot(), displayArea);
                m_stitcher.StartThread();
            }
        }

        public void OnImageResolution()
        {
            var dlg = new ImageResolutionWindow
            {
                DataContext = m_imageResolution
            };

            if (dlg.ShowDialog() == true)
            {
                NewFractal(true);
            }
        }

        public void OnFractalAreaCommand()
        {
            var dlg = new FractalAreaWindow
            {
                DataContext = m_fractalArea
            };

            if (dlg.ShowDialog() == true)
            {
                NewFractal(true);
            }
        }


        public void OnSaveAs()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp",
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
                    }

                    fs.Close();
                }
                m_stitcher.UnlockMutex();
            }
        }
    }
}
