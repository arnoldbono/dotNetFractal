using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using dotNetFractal.UI.Services;

namespace dotNetFractal.Uno.Services;

/// <summary>
/// Uno implementation of IClipboardService using Windows.ApplicationModel.DataTransfer.
/// </summary>
public class UnoClipboardService : IClipboardService
{
    public void SetImage(byte[] imageData, string format)
    {
        if (imageData == null)
            throw new ArgumentNullException(nameof(imageData));

        if (imageData.Length == 0)
            return;

        try
        {
            var dataPackage = new DataPackage();

            // For Uno/WinAppSDK, we need to convert byte array to RandomAccessStreamReference
            var stream = new InMemoryRandomAccessStream();
            stream.WriteAsync(System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.AsBuffer(imageData)).Wait();
            stream.Seek(0);

            var streamRef = RandomAccessStreamReference.CreateFromStream(stream);
            dataPackage.SetBitmap(streamRef);

            Clipboard.SetContent(dataPackage);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting clipboard image: {ex.Message}");
        }
    }
}
