using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using dotNetFractal.UI.Services;

namespace dotNetFractal.Uno.Services;

/// <summary>
/// Uno implementation of IFileDialogService using Windows.Storage.Pickers.
/// </summary>
public class UnoFileDialogService : IFileDialogService
{
    private readonly Window _window;

    public UnoFileDialogService(Window window)
    {
        _window = window ?? throw new ArgumentNullException(nameof(window));
    }

    public string? ShowOpenFileDialog(string filter, string title)
    {
        // Uno uses async file pickers, so we need to run this synchronously
        // In a real scenario, this should be called from async context
        var task = ShowOpenFileDialogAsync(filter, title);
        task.Wait(); // Block until complete
        return task.Result;
    }

    public string? ShowSaveFileDialog(string filter, string title)
    {
        // Uno uses async file pickers
        var task = ShowSaveFileDialogAsync(filter, title);
        task.Wait(); // Block until complete
        return task.Result;
    }

    private async Task<string?> ShowOpenFileDialogAsync(string filter, string title)
    {
        try
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Parse filter string and add file types
            var fileTypes = ParseFilter(filter);
            foreach (var fileType in fileTypes)
            {
                picker.FileTypeFilter.Add(fileType);
            }

            var file = await picker.PickSingleFileAsync();
            return file?.Path;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening file dialog: {ex.Message}");
            return null;
        }
    }

    private async Task<string?> ShowSaveFileDialogAsync(string filter, string title)
    {
        try
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Parse filter string and add file types
            var fileTypeMap = ParseFilterToMap(filter);
            foreach (var kvp in fileTypeMap)
            {
                picker.FileTypeChoices.Add(kvp.Key, kvp.Value);
            }

            var file = await picker.PickSaveFileAsync();
            return file?.Path;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening save dialog: {ex.Message}");
            return null;
        }
    }

    private string[] ParseFilter(string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return new[] { "*" };

        var fileTypes = new System.Collections.Generic.List<string>();
        var parts = filter.Split('|');

        // Filter format: "Description|*.ext|Description2|*.ext2"
        for (int i = 1; i < parts.Length; i += 2)
        {
            if (i < parts.Length)
            {
                var extensions = parts[i].Split(';');
                foreach (var ext in extensions)
                {
                    fileTypes.Add(ext.Trim());
                }
            }
        }

        return fileTypes.Count > 0 ? fileTypes.ToArray() : new[] { "*" };
    }

    private System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<string>> ParseFilterToMap(string filter)
    {
        var map = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<string>>();

        if (string.IsNullOrEmpty(filter))
        {
            map.Add("All Files", new[] { "*" });
            return map;
        }

        // Filter format: "Description|*.ext|Description2|*.ext2"
        var parts = filter.Split('|');
        for (int i = 0; i < parts.Length; i += 2)
        {
            if (i + 1 < parts.Length)
            {
                var description = parts[i];
                var extensions = parts[i + 1].Split(';');
                var extList = new System.Collections.Generic.List<string>();

                foreach (var ext in extensions)
                {
                    extList.Add(ext.Trim());
                }

                map.Add(description, extList);
            }
        }

        return map.Count > 0 ? map : new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<string>> { { "All Files", new[] { "*" } } };
    }

    public void ShowMessage(string message, string title, MessageBoxType type)
    {
        // In Uno, we show messages using ContentDialog
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK"
        };

        // Show dialog (fire and forget since this is sync API)
        _ = dialog.ShowAsync();
    }
}
