using System;
using System.Windows;
using Microsoft.Win32;
using dotNetFractal.UI.Services;

namespace dotNetFractal.WPF.Services;

/// <summary>
/// WPF implementation of IFileDialogService using Windows.Forms dialogs.
/// </summary>
public class WpfFileDialogService : IFileDialogService
{
    public string? ShowOpenFileDialog(string filter, string title)
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            Title = title
        };

        if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.FileName))
        {
            return dialog.FileName;
        }

        return null;
    }

    public string? ShowSaveFileDialog(string filter, string title)
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            Title = title
        };

        if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.FileName))
        {
            return dialog.FileName;
        }

        return null;
    }

    public void ShowMessage(string message, string title, MessageBoxType type)
    {
        var icon = type switch
        {
            MessageBoxType.Information => MessageBoxImage.Information,
            MessageBoxType.Warning => MessageBoxImage.Warning,
            MessageBoxType.Error => MessageBoxImage.Error,
            _ => MessageBoxImage.None
        };

        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}
