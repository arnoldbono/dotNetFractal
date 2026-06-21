using System;
using System.ComponentModel;

namespace dotNetFractal.WPF
{
    public enum ResolutionEnum
    {
        [Description("512,512")]
        Custom,
        [Description("1920,1080")]
        FullHD,
        [Description("3840,2160")]
        UltraHD
    }
}
