using dotNetFractal.UI.Services;

namespace dotNetFractal.Uno.Services;

/// <summary>
/// Uno implementation of IDistributionGraphService.
/// Currently a no-op since Uno doesn't implement the distribution graph feature yet.
/// </summary>
public class UnoDistributionGraphService : IDistributionGraphService
{
    public bool IsGraphOpen => false;

    public void ShowGraph(object graphData)
    {
        // Not implemented for Uno
    }

    public void CloseGraph()
    {
        // Not implemented for Uno
    }
}
