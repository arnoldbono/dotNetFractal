using dotNetFractal.WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace dotNetFractal.WPF.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point? m_selectionStart;
        private bool _isSelecting;

        public MainWindow()
        {
            InitializeComponent();

            // Update canvas size when image changes
            FractalImage.SizeChanged += (s, e) =>
            {
                SelectionCanvas.Width = FractalImage.ActualWidth;
                SelectionCanvas.Height = FractalImage.ActualHeight;
            };
        }

        private void SelectionCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(SelectionCanvas);

            // Check if click is within the actual image bounds
            if (position.X >= 0 && position.X < FractalImage.ActualWidth &&
                position.Y >= 0 && position.Y < FractalImage.ActualHeight)
            {
                m_selectionStart = position;
                _isSelecting = true;
                Canvas.SetLeft(SelectionRectangle, position.X);
                Canvas.SetTop(SelectionRectangle, position.Y);
                SelectionRectangle.Width = 1;
                SelectionRectangle.Height = 1;
                SelectionRectangle.Visibility = Visibility.Visible;
                SelectionCanvas.CaptureMouse();
            }
        }

        private void SelectionCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isSelecting || !m_selectionStart.HasValue)
                return;
            if (FractalImage.ActualWidth == 0 || FractalImage.ActualHeight == 0)
                return;

            var currentPosition = e.GetPosition(SelectionCanvas);

            // Clamp to image bounds
            currentPosition.X = Math.Max(0, Math.Min(currentPosition.X, FractalImage.ActualWidth));
            currentPosition.Y = Math.Max(0, Math.Min(currentPosition.Y, FractalImage.ActualHeight));

            // Calculate the desired width and height
            var deltaX = currentPosition.X - m_selectionStart.Value.X;
            var deltaY = currentPosition.Y - m_selectionStart.Value.Y;

            // Calculate the aspect ratio of the image
            double aspectRatio = FractalImage.ActualWidth / FractalImage.ActualHeight;

            // Determine which dimension to use as the basis
            // Use the dimension with the larger absolute delta
            double width, height;
            if (Math.Abs(deltaX) * FractalImage.ActualHeight > Math.Abs(deltaY) * FractalImage.ActualWidth)
            {
                // Width is the driving dimension
                width = Math.Abs(deltaX);
                height = width / aspectRatio;

                // Adjust height sign to match the drag direction
                if (deltaY < 0)
                    height = -height;
            }
            else
            {
                // Height is the driving dimension
                height = Math.Abs(deltaY);
                width = height * aspectRatio;

                // Adjust width sign to match the drag direction
                if (deltaX < 0)
                    width = -width;
            }

            // Calculate the end point based on constrained dimensions
            var startX = m_selectionStart.Value.X - width;
            var startY = m_selectionStart.Value.Y - height;
            var endX = m_selectionStart.Value.X + width;
            var endY = m_selectionStart.Value.Y + height;

            // Calculate final rectangle position and size
            var x = Math.Min(startX, endX);
            var y = Math.Min(startY, endY);
            var rectWidth = Math.Abs(endX - startX);
            var rectHeight = Math.Abs(endY - startY);

            Canvas.SetLeft(SelectionRectangle, x);
            Canvas.SetTop(SelectionRectangle, y);
            SelectionRectangle.Width = rectWidth;
            SelectionRectangle.Height = rectHeight;

            // Check if Ctrl key is pressed
            var isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isCtrlPressed)
            {
                // Show MainImage in SelectionRectangle
                var viewModel = DataContext as MainViewModel;
                if (viewModel?.MainImage != null)
                {
                    var imageBrush = new System.Windows.Media.ImageBrush(viewModel.MainImage)
                    {
                        Stretch = System.Windows.Media.Stretch.Uniform
                    };
                    SelectionRectangle.Fill = imageBrush;
                }

                // Hide FractalImage
                FractalImage.Visibility = Visibility.Hidden;
            }
            else
            {
                // Reset SelectionRectangle fill to transparent
                SelectionRectangle.Fill = System.Windows.Media.Brushes.Transparent;

                // Show FractalImage
                FractalImage.Visibility = Visibility.Visible;
            }
        }

        private void SelectionCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSelecting && m_selectionStart.HasValue)
            {
                _isSelecting = false;
                SelectionCanvas.ReleaseMouseCapture();

                // Calculate rectangle bounds
                var x1 = Canvas.GetLeft(SelectionRectangle);
                var y1 = Canvas.GetTop(SelectionRectangle);
                var x2 = x1 + SelectionRectangle.Width;
                var y2 = y1 + SelectionRectangle.Height;

                // Only process if we have a meaningful rectangle (at least 5x5 pixels)
                if (Math.Abs(x2 - x1) > 5 && Math.Abs(y2 - y1) > 5)
                {
                    var viewModel = DataContext as MainViewModel;

                    var isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                    if (isCtrlPressed)
                    {
                        // Zoom out with the selected rectangle as the new center
                        viewModel?.ZoomOutFromRectangle(x1, y1, x2, y2, FractalImage.ActualWidth, FractalImage.ActualHeight);
                    }
                    else
                    {
                        // Zoom to the selected rectangle
                        viewModel?.ZoomInToRectangle(x1, y1, x2, y2, FractalImage.ActualWidth, FractalImage.ActualHeight);
                    }
                }

                // Reset SelectionRectangle fill and show FractalImage
                SelectionRectangle.Fill = System.Windows.Media.Brushes.Transparent;
                FractalImage.Visibility = Visibility.Visible;

                // Hide the selection rectangle
                SelectionRectangle.Visibility = Visibility.Collapsed;
                m_selectionStart = null;
            }
        }
    }
}
