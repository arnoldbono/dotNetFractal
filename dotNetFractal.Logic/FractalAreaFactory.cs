
namespace dotNetFractal.Logic
{
    public static class FractalAreaFactory
    {
        public static void SetJuliaSet(IFractalArea fractalArea, IFractalArea oldFractalArea)
        {
            if (fractalArea is FractalArea<FractalDecimal> fractalAreaT1)
            {
                var oldFractalAreaT1 = oldFractalArea as FractalArea<FractalDecimal>;
                if (oldFractalAreaT1 != default && oldFractalAreaT1.JuliaSet)
                {
                    fractalAreaT1.SetJulieSet(oldFractalAreaT1.JuliaSetX, oldFractalAreaT1.JuliaSetY);
                }
                else
                {
                    var displayArea = fractalAreaT1.DisplayArea as DisplayArea<FractalDecimal>;
                    fractalAreaT1.SetJulieSet(displayArea.CenterX, displayArea.CenterY);
                }
            }
            else if (fractalArea is FractalArea<FractalDouble> fractalAreaT2)
            {
                var oldFractalAreaT2 = oldFractalArea as FractalArea<FractalDouble>;
                if (oldFractalAreaT2 != default && oldFractalAreaT2.JuliaSet)
                {
                    fractalAreaT2.SetJulieSet(oldFractalAreaT2.JuliaSetX, oldFractalAreaT2.JuliaSetY);
                }
                else
                {
                    var displayArea = fractalAreaT2.DisplayArea as DisplayArea<FractalDouble>;
                    fractalAreaT2.SetJulieSet(displayArea.CenterX, displayArea.CenterY);
                }
            }
        }
    }
}
