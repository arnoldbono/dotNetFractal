using System;
using dotNetFractal.UI.Services;
using dotNetFractal.WPF.Presentation;

namespace dotNetFractal.WPF.Services;

/// <summary>
/// WPF implementation of IDistributionGraphService for showing distribution graphs.
/// </summary>
public class WpfDistributionGraphService : IDistributionGraphService
{
    private DistributionGraphWindow? m_graphWindow;

    public bool IsGraphOpen => m_graphWindow?.IsLoaded == true;

    public void ShowGraph(object graphData)
    {
        if (graphData == null)
            return;

        // Ensure we have a valid data object
        if (graphData is not int[] distributionGraph)
            throw new ArgumentException("graphData must be of type int[]", nameof(graphData));

        if (m_graphWindow?.IsLoaded == true)
        {
            // Update existing window
            m_graphWindow.UpdateGraph(distributionGraph);
            return;
        }

        // Create and show new window
        m_graphWindow = new DistributionGraphWindow(distributionGraph);
        m_graphWindow.Show();
    }

    public void CloseGraph()
    {
        if (m_graphWindow?.IsLoaded == true)
        {
            m_graphWindow.Close();
        }

        m_graphWindow = null;
    }
}
