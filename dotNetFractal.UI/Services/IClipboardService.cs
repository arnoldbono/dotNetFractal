namespace dotNetFractal.UI.Services;

/// <summary>
/// Platform-agnostic service for clipboard operations.
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Sets an image on the clipboard.
    /// </summary>
    /// <param name="imageData">Image data as byte array</param>
    /// <param name="format">Image format (e.g., "png", "jpg", "bmp")</param>
    void SetImage(byte[] imageData, string format);
}
