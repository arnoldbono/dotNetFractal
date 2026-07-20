namespace dotNetFractal.UI.Services;

/// <summary>
/// Platform-agnostic interface for showing a distribution graph window.
/// This is only implemented on platforms that support it (e.g., WPF).
/// </summary>
public interface IDistributionGraphService
{
    /// <summary>
    /// Shows or updates the distribution graph with the given data.
    /// </summary>
    /// <param name="graphData">The distribution graph data to display.</param>
    void ShowGraph(object graphData);

    /// <summary>
    /// Closes the distribution graph window if it's open.
    /// </summary>
    void CloseGraph();

    /// <summary>
    /// Gets whether the graph window is currently open.
    /// </summary>
    bool IsGraphOpen { get; }
}
