using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace dotNetFractal.WPF
{
    public class ConvertBitmapToBitmapImage
    {
        /// <summary>
        /// Converts a bitmap to an image that can be used as an ImageSource
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        public static BitmapImage Convert(Bitmap src)
        {
            var ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
