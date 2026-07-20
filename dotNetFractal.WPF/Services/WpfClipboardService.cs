using System;
using System.Windows;
using dotNetFractal.UI.Services;

namespace dotNetFractal.WPF.Services;

/// <summary>
/// WPF implementation of IClipboardService using System.Windows.Clipboard.
/// </summary>
public class WpfClipboardService : IClipboardService
{
    public void SetImage(byte[] imageData, string format)
    {
        if (imageData == null)
            throw new ArgumentNullException(nameof(imageData));

        if (imageData.Length == 0)
            return;

        try
        {
            // For WPF, we work with BitmapSource objects
            // This method converts byte data to BitmapSource and sets it on clipboard
            using (var ms = new System.IO.MemoryStream(imageData))
            {
                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                Clipboard.SetImage(bitmap);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting clipboard image: {ex.Message}");
        }
    }
}
