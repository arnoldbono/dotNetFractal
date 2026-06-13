using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point? _selectionStart;
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
                _selectionStart = position;
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
            if (_isSelecting && _selectionStart.HasValue)
            {
                if (FractalImage.ActualWidth == 0 || FractalImage.ActualHeight == 0)
                    return;

                var currentPosition = e.GetPosition(SelectionCanvas);

                // Clamp to image bounds
                currentPosition.X = Math.Max(0, Math.Min(currentPosition.X, FractalImage.ActualWidth));
                currentPosition.Y = Math.Max(0, Math.Min(currentPosition.Y, FractalImage.ActualHeight));

                // Calculate the desired width and height
                var deltaX = currentPosition.X - _selectionStart.Value.X;
                var deltaY = currentPosition.Y - _selectionStart.Value.Y;

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
                var endX = _selectionStart.Value.X + width;
                var endY = _selectionStart.Value.Y + height;

                // Clamp end point to image bounds and adjust if necessary
                if (endX < 0)
                {
                    width = -_selectionStart.Value.X;
                    height = width / aspectRatio * Math.Sign(height);
                    endX = 0;
                    endY = _selectionStart.Value.Y + height;
                }
                else if (endX > FractalImage.ActualWidth)
                {
                    width = FractalImage.ActualWidth - _selectionStart.Value.X;
                    height = width / aspectRatio * Math.Sign(height);
                    endX = FractalImage.ActualWidth;
                    endY = _selectionStart.Value.Y + height;
                }

                if (endY < 0)
                {
                    height = -_selectionStart.Value.Y;
                    width = height * aspectRatio * Math.Sign(width);
                    endY = 0;
                    endX = _selectionStart.Value.X + width;
                }
                else if (endY > FractalImage.ActualHeight)
                {
                    height = FractalImage.ActualHeight - _selectionStart.Value.Y;
                    width = height * aspectRatio * Math.Sign(width);
                    endY = FractalImage.ActualHeight;
                    endX = _selectionStart.Value.X + width;
                }

                // Calculate final rectangle position and size
                var x = Math.Min(_selectionStart.Value.X, endX);
                var y = Math.Min(_selectionStart.Value.Y, endY);
                var rectWidth = Math.Abs(endX - _selectionStart.Value.X);
                var rectHeight = Math.Abs(endY - _selectionStart.Value.Y);

                Canvas.SetLeft(SelectionRectangle, x);
                Canvas.SetTop(SelectionRectangle, y);
                SelectionRectangle.Width = rectWidth;
                SelectionRectangle.Height = rectHeight;
            }
        }

        private void SelectionCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSelecting && _selectionStart.HasValue)
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
                    viewModel?.ZoomToRectangle(x1, y1, x2, y2, FractalImage.ActualWidth, FractalImage.ActualHeight);
                }

                // Hide the selection rectangle
                SelectionRectangle.Visibility = Visibility.Collapsed;
                _selectionStart = null;
            }
        }
    }
}
