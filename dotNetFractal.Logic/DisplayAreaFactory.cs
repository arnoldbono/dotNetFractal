
namespace dotNetFractal.Logic
{
    public class DisplayAreaFactory
    {
        public static IDisplayArea Convert(IDisplayArea displayArea, bool useDecimal)
        {
            if (useDecimal)
            {
                if (displayArea is DisplayArea<FractalDecimal>)
                {
                    return displayArea;
                }

                if (displayArea is DisplayArea<FractalDouble> displayAreaT)

                return new DisplayArea<FractalDecimal>(
                    (FractalDecimal)(double)displayAreaT.CenterX,
                    (FractalDecimal)(double)displayAreaT.CenterY,
                    (FractalDecimal)(double)displayAreaT.Width,
                    (FractalDecimal)(double)displayAreaT.Height,
                    displayAreaT.PixelsHorizontal,
                    displayAreaT.PixelsVertical
                );
            }
            else
            {
                if (displayArea is DisplayArea<FractalDouble>)
                {
                    return displayArea;
                }

                if (displayArea is DisplayArea<FractalDecimal> displayAreaT)
                {
                    return new DisplayArea<FractalDouble>(
                        (FractalDouble)(double)displayAreaT.CenterX,
                        (FractalDouble)(double)displayAreaT.CenterY,
                        (FractalDouble)(double)displayAreaT.Width,
                        (FractalDouble)(double)displayAreaT.Height,
                        displayAreaT.PixelsHorizontal,
                        displayAreaT.PixelsVertical
                    );
                }
            }

            return default;
        }

        public static IFractalArea CreateFractalArea(IDisplayArea displayArea)
        {
            if (displayArea is DisplayArea<FractalDecimal> displayAreaT1)
            {
                return new FractalArea<FractalDecimal>(displayAreaT1);
            }
            else if (displayArea is DisplayArea<FractalDouble> displayAreaT2)
            {
                return new FractalArea<FractalDouble>(displayAreaT2);
            }

            return default;
        }

        public static IDisplayArea ZoomIn(IDisplayArea displayArea, int i1, int j1, int i2, int j2, int horizontal, int vertical)
        {
            if (displayArea is DisplayArea<FractalDecimal> displayAreaT1)
            {
                return new DisplayArea<FractalDecimal>(displayAreaT1.GetCenterX(i1, i2), displayAreaT1.GetCenterY(j1, j2), displayAreaT1.GetWidth(i1, i2), displayAreaT1.GetHeight(j1, j2), horizontal, vertical);
            }
            else if (displayArea is DisplayArea<FractalDouble> displayAreaT2)
            {
                return new DisplayArea<FractalDouble>(displayAreaT2.GetCenterX(i1, i2), displayAreaT2.GetCenterY(j1, j2), displayAreaT2.GetWidth(i1, i2), displayAreaT2.GetHeight(j1, j2), horizontal, vertical);
            }

            return default;
        }

    }
}
