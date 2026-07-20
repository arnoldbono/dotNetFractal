using dotNetFractal.UI.Services;

namespace dotNetFractal.Uno.Services;

/// <summary>
/// Uno implementation of IWindowManager.
/// Currently a no-op since Uno/WinAppSDK doesn't support full-screen mode the same way as WPF.
/// </summary>
public class UnoWindowManager : IWindowManager
{
    private bool m_isFullScreen;

    public bool IsFullScreen
    {
        get => m_isFullScreen;
        set => m_isFullScreen = value;
    }
}
