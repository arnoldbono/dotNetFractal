namespace dotNetFractal.UI.Services;

/// <summary>
/// Platform-agnostic service for file open/save dialogs.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Shows an open file dialog and returns the selected file path.
    /// </summary>
    /// <param name="filter">File filter (e.g., "Text Files|*.txt|All Files|*.*")</param>
    /// <param name="title">Dialog title</param>
    /// <returns>Selected file path, or null if cancelled</returns>
    string? ShowOpenFileDialog(string filter, string title);

    /// <summary>
    /// Shows a save file dialog and returns the selected file path.
    /// </summary>
    /// <param name="filter">File filter (e.g., "Text Files|*.txt|All Files|*.*")</param>
    /// <param name="title">Dialog title</param>
    /// <returns>Selected file path, or null if cancelled</returns>
    string? ShowSaveFileDialog(string filter, string title);

    /// <summary>
    /// Shows a message box with the specified message and title.
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="title">Dialog title</param>
    /// <param name="type">Message box type (Information, Warning, Error)</param>
    void ShowMessage(string message, string title, MessageBoxType type);
}

/// <summary>
/// Type of message to display.
/// </summary>
public enum MessageBoxType
{
    Information,
    Warning,
    Error
}
