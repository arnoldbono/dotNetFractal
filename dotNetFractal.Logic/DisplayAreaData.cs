using System.Text.Json.Serialization;

namespace dotNetFractal.Logic;

/// <summary>
/// Data transfer object for serializing/deserializing DisplayArea data to/from JSON.
/// </summary>
public class DisplayAreaData
{
    [JsonPropertyName("centerX")]
    public string CenterX { get; set; } = null!;

    [JsonPropertyName("centerY")]
    public string CenterY { get; set; } = null!;

    [JsonPropertyName("width")] 
    public string Width { get; set; } = null!;

    [JsonPropertyName("height")]
    public string Height { get; set; } = null!;

    [JsonPropertyName("cx")]
    public string Cx { get; set; } = null!;

    [JsonPropertyName("cy")]
    public string Cy { get; set; } = null!;

    [JsonPropertyName("pixelsHorizontal")]
    public int PixelsHorizontal { get; set; }

    [JsonPropertyName("pixelsVertical")]
    public int PixelsVertical { get; set; }

    [JsonPropertyName("maxIterations")]
    public int MaxIterations { get; set; }

    [JsonPropertyName("maxColorSteps")]
    public int MaxColorSteps { get; set; }

    [JsonPropertyName("firstColorStep")]
    public int FirstColorStep { get; set; }

    [JsonPropertyName("smoothColoring")]
    public bool SmoothColoring { get; set; }

    [JsonPropertyName("highPrecision")]
    public bool HighPrecision { get; set; }

    [JsonPropertyName("fractalType")]
    public string FractalType { get; set; } = null!;

    [JsonPropertyName("fileVersion")]
    public string FileVersion { get; set; } = "1.0";
}
