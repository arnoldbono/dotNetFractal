using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using dotNetFractal.WPF.ViewModels;

namespace dotNetFractal.WPF.Presentation
{
    public partial class DistributionGraphWindow : Window
    {
        private readonly DistributionGraphViewModel m_viewModel;

        public DistributionGraphWindow(int[] distributionGraph)
        {
            InitializeComponent();

            m_viewModel = new DistributionGraphViewModel(distributionGraph);
            DataContext = m_viewModel;

            // Draw the graph when the canvas is loaded
            Loaded += DistributionGraphWindow_Loaded;
            GraphCanvas.SizeChanged += GraphCanvas_SizeChanged;
        }

        private void DistributionGraphWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGraph();
        }

        private void GraphCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawGraph();
        }

        private void DrawGraph()
        {
            if (m_viewModel.GraphPoints == null || m_viewModel.GraphPoints.Count == 0)
            {
                return;
            }

            GraphCanvas.Children.Clear();

            double canvasWidth = GraphCanvas.ActualWidth;
            double canvasHeight = GraphCanvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0)
            {
                return;
            }

            // Draw grid lines
            DrawGridLines(canvasWidth, canvasHeight);

            // Calculate bar width to fit exactly within canvas
            int pointCount = m_viewModel.GraphPoints.Count;
            double spacing = 1;
            double totalSpacing = spacing * (pointCount - 1);
            double barWidth = Math.Max(1, (canvasWidth - totalSpacing) / pointCount);

            // Draw bars
            for (int i = 0; i < m_viewModel.GraphPoints.Count; i++)
            {
                var point = m_viewModel.GraphPoints[i];
                double barHeight = (point.ScaledValue / 100.0) * canvasHeight;
                double x = i * (barWidth + spacing);
                double y = canvasHeight - barHeight;

                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = new SolidColorBrush(Color.FromRgb(0, 120, 215)), // #FF0078D7
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 100, 180)),
                    StrokeThickness = 0.5
                };

                Canvas.SetLeft(bar, x);
                Canvas.SetTop(bar, y);
                GraphCanvas.Children.Add(bar);

                // Add tooltip with details
                bar.ToolTip = $"Iteration: {point.Iteration}\nCount: {point.Count}\nScaled: {point.ScaledValue:F2}";
            }
        }

        private void DrawGridLines(double width, double height)
        {
            var gridBrush = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255));

            // Draw horizontal grid lines (for Y-axis: 0, 25, 50, 75, 100)
            for (int i = 0; i <= 4; i++)
            {
                double y = height - (i * height / 4.0);
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = gridBrush,
                    StrokeThickness = 1
                };
                GraphCanvas.Children.Add(line);

                // Add Y-axis labels
                var label = new TextBlock
                {
                    Text = $"{i * 25}",
                    Foreground = Brushes.LightGray,
                    FontSize = 10
                };
                Canvas.SetLeft(label, -35);
                Canvas.SetTop(label, y - 7);
                GraphCanvas.Children.Add(label);
            }

            // Draw vertical grid lines (every ~10% of iterations)
            if (m_viewModel.MaxIteration > 0)
            {
                int step = Math.Max(1, m_viewModel.MaxIteration / 10);
                int pointCount = m_viewModel.GraphPoints.Count;
                double spacing = 1;
                double totalSpacing = spacing * (pointCount - 1);
                double barWidth = Math.Max(1, (width - totalSpacing) / pointCount);

                for (int iteration = 0; iteration <= m_viewModel.MaxIteration; iteration += step)
                {
                    int index = m_viewModel.GraphPoints.FindIndex(p => p.Iteration >= iteration);
                    if (index >= 0)
                    {
                        double x = index * (barWidth + spacing);
                        var line = new Line
                        {
                            X1 = x,
                            Y1 = 0,
                            X2 = x,
                            Y2 = height,
                            Stroke = gridBrush,
                            StrokeThickness = 1
                        };
                        GraphCanvas.Children.Add(line);

                        // Add X-axis labels
                        var label = new TextBlock
                        {
                            Text = iteration.ToString(),
                            Foreground = Brushes.LightGray,
                            FontSize = 10
                        };
                        Canvas.SetLeft(label, x - 10);
                        Canvas.SetTop(label, height + 5);
                        GraphCanvas.Children.Add(label);
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Updates the distribution graph with new data and redraws it
        /// </summary>
        /// <param name="distributionGraph">The new distribution graph data</param>
        public void UpdateGraph(int[] distributionGraph)
        {
            if (distributionGraph == null)
                return;

            m_viewModel.UpdateDistributionGraph(distributionGraph);
            DrawGraph();
        }
    }
}
