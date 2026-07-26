using System.Collections.ObjectModel;
using System.Windows.Input;
using dotNetFractal.Logic;
using dotNetFractal.UI.Commands;
using dotNetFractal.UI.Models;
using dotNetFractal.UI.Services;
using ReactiveUI;
using SkiaSharp;

namespace dotNetFractal.UI.ViewModels;

/// <summary>
/// ViewModel for the Color Map Window that generates a bitmap showing the FractalColorMap.
/// </summary>
public class ColorMapViewModel : BaseViewModel
{
    private readonly FractalColorMap m_colorMap;
    private readonly IBitmapConverter m_bitmapConverter;
    private readonly IFileDialogService m_fileDialogService;

    private object m_colorMapImage = null!;
    private EditableFractalColor? m_selectedColor = null!;
    private RelayCommand<object>? m_addColorCommand;
    private RelayCommand<object>? m_deleteColorCommand;
    private RelayCommand<object>? m_resetColorMapCommand;
    private RelayCommand<object>? m_saveColorMapCommand;
    private RelayCommand<object>? m_loadColorMapCommand;

    public object ColorMapImage
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

    public EditableFractalColor? SelectedColor
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

    public ICommand ResetColorMapCommand => m_resetColorMapCommand ??= new RelayCommand<object>(_ => ResetColorMap());

    public ICommand SaveColorMapCommand => m_saveColorMapCommand ??= new RelayCommand<object>(_ => SaveColorMap());

    public ICommand LoadColorMapCommand => m_loadColorMapCommand ??= new RelayCommand<object>(_ => LoadColorMap());

    public ColorMapViewModel(IBitmapConverter bitmapConverter, IFileDialogService fileDialogService)
    {
        m_bitmapConverter = bitmapConverter ?? throw new ArgumentNullException(nameof(bitmapConverter));
        m_fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
        m_colorMap = FractalColorMap.GetInstance();

        // Create editable wrappers for the colors
        Colors = [];
        foreach (var color in m_colorMap.Colors)
        {
            var editableColor = new EditableFractalColor(color);
            editableColor.PropertyChanged += OnColorChanged;
            Colors.Add(editableColor);
        }

        ColorMapImage = GenerateColorMapBitmap();

        this.WhenAnyValue(x => x.SelectedColor).Subscribe(_ =>
        {
            m_addColorCommand?.RaiseCanExecuteChanged();
            m_deleteColorCommand?.RaiseCanExecuteChanged();
        });
    }

    private void OnColorChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Update the underlying FractalColorMap
        var count = Math.Min(Colors.Count, m_colorMap.Colors.Length);
        var colors = new FractalColor[count];
        for (int i = 0; i < count; i++)
        {
            colors[i] = Colors[i].ToFractalColor();
        }
        m_colorMap.Colors = colors;

        // Regenerate the bitmap
        ColorMapImage = GenerateColorMapBitmap();
    }

    /// <summary>
    /// Generates a 256x1 bitmap showing the FractalColorMap from fraction 0.0 to 1.0
    /// </summary>
    private object GenerateColorMapBitmap()
    {
        const int Width = 256;
        const int Height = 1;
        const double FractionStep = 1.0 / (Width - 1);

        // Create a 256x1 bitmap
        using (var bitmap = new SKBitmap(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul))
        {
            // Fill each pixel with the corresponding color from the color map
            for (int x = 0; x < Width; x++)
            {
                double fraction = x * FractionStep;
                var fractalColor = m_colorMap.GetColor(fraction);

                // Set the pixel color
                var color = new SKColor(
                    (byte)fractalColor.Red,
                    (byte)fractalColor.Green,
                    (byte)fractalColor.Blue);
                bitmap.SetPixel(x, 0, color);
            }

            return m_bitmapConverter.ConvertToImageSource(bitmap);
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
        EditableFractalColor? colorAbove = selectedIndex > 0 ? Colors[selectedIndex - 1] : null;
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
        newColor.PropertyChanged += OnColorChanged;

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
        if (SelectedColor == null || !CanDeleteColor())
        {
            return;
        }

        var first = Colors[0] == SelectedColor;
        var last = Colors[^1] == SelectedColor;

        // Remove the selected color
        SelectedColor.PropertyChanged -= OnColorChanged;
        Colors.Remove(SelectedColor);
        SelectedColor = null;

        if (first)
            Colors[0].Fraction = 0.0;
        else if (last)
            Colors[^1].Fraction = 1.0;

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

    private void ResetColorMap()
    {
        // Reset the underlying color map to default colors
        m_colorMap.Colors = (FractalColor[])FractalColorMap.GetInstance().DefaultColors.Clone();
        OnColorMapChanged();
    }

    private void SaveColorMap()
    {
        const string filter = "Color Map File|*.json";
        const string title = "Save a color map file";
        var filePath = m_fileDialogService.ShowSaveFileDialog(filter, title);
        if (!string.IsNullOrEmpty(filePath))
        {
            m_colorMap.SaveToFile(filePath);
            OnColorMapChanged();
        }
    }

    private void LoadColorMap()
    {
        const string filter = "Color Map File|*.json";
        const string title = "Load a color map file";
        var filePath = m_fileDialogService.ShowOpenFileDialog(filter, title);
        if (!string.IsNullOrEmpty(filePath))
        {
            m_colorMap.LoadFromFile(filePath);
            OnColorMapChanged();
        }
    }

    private void OnColorMapChanged()
    {
        // Clear existing colors
        foreach (var color in Colors)
        {
            color.PropertyChanged -= OnColorChanged;
        }
        Colors.Clear();
        // Create editable wrappers for the new colors
        foreach (var color in m_colorMap.Colors)
        {
            var editableColor = new EditableFractalColor(color);
            editableColor.PropertyChanged += OnColorChanged;
            Colors.Add(editableColor);
        }
        // Clear selection
        SelectedColor = null;
        // Regenerate the bitmap
        ColorMapImage = GenerateColorMapBitmap();
    }
}
