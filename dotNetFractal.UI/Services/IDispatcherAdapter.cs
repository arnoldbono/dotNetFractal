namespace dotNetFractal.UI.Services;

/// <summary>
/// Abstracts platform-specific dispatcher implementations (WPF, Uno, etc).
/// Allows business logic to work independently of UI framework dispatcher details.
/// </summary>
public interface IDispatcherAdapter
{
    /// <summary>
    /// Gets a value indicating whether the current thread is the UI thread.
    /// </summary>
    bool IsOnUIThread { get; }

    /// <summary>
    /// Executes an action on the UI thread. If already on the UI thread, executes immediately.
    /// </summary>
    void RunOnUIThread(Action action);

    /// <summary>
    /// Executes an action on the UI thread asynchronously.
    /// </summary>
    void RunOnUIThreadAsync(Action action);
}
