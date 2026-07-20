namespace dotNetFractal.UI.Services;

/// <summary>
/// Platform-agnostic service for window state management (e.g., full-screen mode).
/// </summary>
public interface IWindowManager
{
    /// <summary>
    /// Gets or sets whether the window is in full-screen mode.
    /// </summary>
    bool IsFullScreen { get; set; }
}
