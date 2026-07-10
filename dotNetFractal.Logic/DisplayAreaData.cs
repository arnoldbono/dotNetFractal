using System.Text.Json.Serialization;

namespace dotNetFractal.Logic
{
    /// <summary>
    /// Data transfer object for serializing/deserializing DisplayArea data to/from JSON.
    /// </summary>
    public class DisplayAreaData
    {
        [JsonPropertyName("centerX")]
        public string CenterX { get; set; }

        [JsonPropertyName("centerY")]
        public string CenterY { get; set; }

        [JsonPropertyName("width")]
        public string Width { get; set; }

        [JsonPropertyName("height")]
        public string Height { get; set; }

        [JsonPropertyName("cx")]
        public string Cx { get; set; }

        [JsonPropertyName("cy")]
        public string Cy { get; set; }

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
        public string FractalType { get; set; }

        [JsonPropertyName("fileVersion")]
        public string FileVersion { get; set; } = "1.0";
    }
}
