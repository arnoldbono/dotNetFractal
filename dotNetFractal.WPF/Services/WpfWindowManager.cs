using System;
using System.Windows;
using dotNetFractal.UI.Services;

namespace dotNetFractal.WPF.Services;

/// <summary>
/// WPF implementation of IWindowManager for full-screen and window state management.
/// </summary>
public class WpfWindowManager : IWindowManager
{
    private readonly Window m_window;
    private WindowStyle m_previousWindowStyle = WindowStyle.SingleBorderWindow;
    private WindowState m_previousWindowState = WindowState.Normal;
    private bool m_isFullScreen;

    public WpfWindowManager(Window window)
    {
        m_window = window ?? throw new ArgumentNullException(nameof(window));
    }

    public bool IsFullScreen
    {
        get => m_isFullScreen;
        set
        {
            if (m_isFullScreen == value)
                return;

            m_isFullScreen = value;

            if (value)
            {
                // Store current state and enter full-screen
                m_previousWindowStyle = m_window.WindowStyle;
                m_previousWindowState = m_window.WindowState;

                m_window.WindowStyle = WindowStyle.None;
                m_window.WindowState = WindowState.Maximized;
            }
            else
            {
                // Restore previous state
                m_window.WindowStyle = m_previousWindowStyle;
                m_window.WindowState = m_previousWindowState;
            }
        }
    }
}
