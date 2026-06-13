using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using dotNetFractal.Logic;

namespace dotNetFractal.WPF
{
    /// <summary>
    /// ViewModel for the Color Map Window that generates a bitmap showing the FractalColorMap
    /// </summary>
    public class ColorMapViewModel : BaseViewModel
    {
        private ImageSource m_colorMapImage;
        private readonly FractalColorMap m_colorMap;
        private EditableFractalColor m_selectedColor;
        private RelayCommand<object> m_addColorCommand;
        private RelayCommand<object> m_deleteColorCommand;

        public ImageSource ColorMapImage
        {
            get => m_colorMapImage;
            set
            {
                if (ReferenceEquals(m_colorMapImage, value))
                {
                    return;
                }

                m_colorMapImage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EditableFractalColor> Colors { get; private set; }

        public EditableFractalColor SelectedColor
        {
            get => m_selectedColor;
            set
            {
                if (ReferenceEquals(m_selectedColor, value))
                {
                    return;
                }

                m_selectedColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddColorCommand => m_addColorCommand ??= new RelayCommand<object>(_ => AddColor(), _ => CanAddColor());

        public ICommand DeleteColorCommand => m_deleteColorCommand ??= new RelayCommand<object>(_ => DeleteColor(), _ => CanDeleteColor());

        public ColorMapViewModel()
        {
            m_colorMap = FractalColorMap.GetInstance();

            // Create editable wrappers for the colors
            Colors = new ObservableCollection<EditableFractalColor>();
            foreach (var color in m_colorMap.Colors)
            {
                var editableColor = new EditableFractalColor(color);
                editableColor.ColorChanged += OnColorChanged;
                Colors.Add(editableColor);
            }

            ColorMapImage = GenerateColorMapBitmap();
        }

        private void OnColorChanged()
        {
            // Update the underlying FractalColorMap
            for (int i = 0; i < Colors.Count && i < m_colorMap.Colors.Length; i++)
            {
                m_colorMap.Colors[i] = Colors[i].ToFractalColor();
            }

            // Regenerate the bitmap
            ColorMapImage = GenerateColorMapBitmap();
        }

        /// <summary>
        /// Generates a 256x1 bitmap showing the FractalColorMap from fraction 0.0 to 1.0
        /// </summary>
        private ImageSource GenerateColorMapBitmap()
        {
            const int width = 256;
            const int height = 1;

            // Create a 256x1 bitmap
            using (var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                // Fill each pixel with the corresponding color from the color map
                for (int x = 0; x < width; x++)
                {
                    // Calculate fraction from 0.0 to 1.0
                    double fraction = x / (double)(width - 1);

                    // Get color from the FractalColorMap
                    var fractalColor = m_colorMap[fraction];

                    // Set the pixel color
                    var color = System.Drawing.Color.FromArgb(
                        fractalColor.Red,
                        fractalColor.Green,
                        fractalColor.Blue);

                    bitmap.SetPixel(x, 0, color);
                }

                // Convert System.Drawing.Bitmap to WPF ImageSource
                return ConvertBitmapToBitmapImage.Convert(bitmap);
            }
        }

        private bool CanAddColor()
        {
            return Colors.Count >= 2;
        }

        private void AddColor()
        {
            if (!CanAddColor())
            {
                return;
            }

            int selectedIndex = SelectedColor != null ? Colors.IndexOf(SelectedColor) : Colors.Count - 1;

            // Default to adding before the last item if nothing is selected or selection is invalid
            if (selectedIndex < 0)
            {
                selectedIndex = Colors.Count - 1;
            }

            // Get the neighboring colors for interpolation
            EditableFractalColor colorAbove = selectedIndex > 0 ? Colors[selectedIndex - 1] : null;
            EditableFractalColor colorBelow = Colors[selectedIndex];

            // Calculate interpolated values
            int red, green, blue;
            double fraction;

            if (colorAbove != null)
            {
                // Interpolate between the color above and the current color
                red = (colorAbove.Red + colorBelow.Red) / 2;
                green = (colorAbove.Green + colorBelow.Green) / 2;
                blue = (colorAbove.Blue + colorBelow.Blue) / 2;
                fraction = (colorAbove.Fraction + colorBelow.Fraction) / 2.0;
            }
            else
            {
                // If there's no color above, use the current color's values with a slightly smaller fraction
                red = colorBelow.Red;
                green = colorBelow.Green;
                blue = colorBelow.Blue;
                fraction = Math.Max(0.0, colorBelow.Fraction - 0.1);
            }

            // Create the new color
            var newColor = new EditableFractalColor(new FractalColor(red, green, blue, fraction));
            newColor.ColorChanged += OnColorChanged;

            // Insert on the selected row
            Colors.Insert(selectedIndex, newColor);

            // Update the underlying color map and regenerate bitmap
            UpdateColorMap();
        }

        private bool CanDeleteColor()
        {
            // Need at least 2 colors, and one must be selected
            return SelectedColor != null && Colors.Count > 2;
        }

        private void DeleteColor()
        {
            if (!CanDeleteColor())
            {
                return;
            }

            // Remove the selected color
            SelectedColor.ColorChanged -= OnColorChanged;
            Colors.Remove(SelectedColor);
            SelectedColor = null;

            // Update the underlying color map and regenerate bitmap
            UpdateColorMap();
        }

        private void UpdateColorMap()
        {
            // Update the underlying FractalColorMap with all current colors
            m_colorMap.Colors = [.. Colors.Select(c => c.ToFractalColor())];

            // Regenerate the bitmap
            ColorMapImage = GenerateColorMapBitmap();
        }
    }
}
