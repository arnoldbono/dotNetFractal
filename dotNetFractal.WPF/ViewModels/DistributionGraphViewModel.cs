using System;
using System.Collections.Generic;
using System.Linq;

namespace dotNetFractal.WPF.ViewModels
{
    public class DistributionGraphViewModel : BaseViewModel
    {
        private List<DistributionGraphPoint> m_graphPoints;
        private int m_maxIteration;
        private int m_totalPixels;

        public List<DistributionGraphPoint> GraphPoints
        {
            get => m_graphPoints;
            set
            {
                if (m_graphPoints == value)
                {
                    return;
                }

                m_graphPoints = value;
                OnPropertyChanged();
            }
        }

        public int MaxIteration
        {
            get => m_maxIteration;
            set
            {
                if (m_maxIteration == value)
                {
                    return;
                }

                m_maxIteration = value;
                OnPropertyChanged();
            }
        }

        public int TotalPixels
        {
            get => m_totalPixels;
            set
            {
                if (m_totalPixels == value)
                {
                    return;
                }

                m_totalPixels = value;
                OnPropertyChanged();
            }
        }

        public DistributionGraphViewModel(int[] distributionGraph)
        {
            UpdateDistributionGraph(distributionGraph);
        }

        public void UpdateDistributionGraph(int[] distributionGraph)
        {
            if (distributionGraph == null || distributionGraph.Length == 0)
            {
                GraphPoints = [];
                MaxIteration = 0;
                TotalPixels = 0;
                return;
            }

            // Find the maximum count for scaling
            int maxValue = distributionGraph.Max();
            if (maxValue == 0)
                maxValue = 1; // Avoid division by zero

            // Create graph points with scaled values
            var points = new List<DistributionGraphPoint>();
            int maxIteration = -1;
            for (int i = 0; i < distributionGraph.Length; i++)
            {
                if (distributionGraph[i] == 0) // Only include iterations with non-zero counts
                    continue;

                points.Add(new DistributionGraphPoint
                {
                    Iteration = i,
                    Count = distributionGraph[i],
                    ScaledValue = (distributionGraph[i] / (double)maxValue) * 100.0
                });

                maxIteration = i;
            }

            GraphPoints = points;
            MaxIteration = maxIteration + 1; // Correcting for zero-based index
            TotalPixels = distributionGraph.Sum();
        }
    }

    public class DistributionGraphPoint
    {
        public int Iteration { get; set; }
        public int Count { get; set; }
        public double ScaledValue { get; set; }
    }
}
