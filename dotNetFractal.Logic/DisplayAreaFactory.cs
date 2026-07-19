
namespace dotNetFractal.Logic;

public class DisplayAreaFactory
{
    public static IDisplayArea? Create(IDisplayArea displayArea, int pixelsHorizontal, int pixelsVertical)
    {
        var displayAreaT = Convert(displayArea, displayArea is DisplayArea<decimal>);
        displayAreaT?.Resize(pixelsHorizontal, pixelsVertical);
        return displayAreaT;
    }

    public static IDisplayArea? Convert(IDisplayArea displayArea, bool useDecimal)
    {
        if (useDecimal)
        {
            if (displayArea is DisplayArea<decimal> displayAreaT1)
                return new DisplayArea<decimal>(displayAreaT1);

            if (displayArea is DisplayArea<double> displayAreaT)
                return new DisplayArea<decimal>(
                    (decimal)displayAreaT.CenterX,
                    (decimal)displayAreaT.CenterY,
                    (decimal)displayAreaT.Width,
                    (decimal)displayAreaT.Height,
                    (decimal)displayAreaT.Cx,
                    (decimal)displayAreaT.Cy,
                    displayAreaT.PixelsHorizontal,
                    displayAreaT.PixelsVertical
                );
        }
        else
        {
            if (displayArea is DisplayArea<double> displayAreaT1)
                return new DisplayArea<double>(displayAreaT1);

            if (displayArea is DisplayArea<decimal> displayAreaT)
                return new DisplayArea<double>(
                    (double)displayAreaT.CenterX,
                    (double)displayAreaT.CenterY,
                    (double)displayAreaT.Width,
                    (double)displayAreaT.Height,
                    (double)displayAreaT.Cx,
                    (double)displayAreaT.Cy,
                    displayAreaT.PixelsHorizontal,
                    displayAreaT.PixelsVertical
                );
        }

        return default;
    }

    public static IFractalArea? CreateFractalArea(IDisplayArea displayArea)
    {
        if (displayArea is DisplayArea<decimal> displayAreaT1)
        {
            return new FractalArea<decimal>(displayAreaT1);
        }
        else if (displayArea is DisplayArea<double> displayAreaT2)
        {
            return new FractalArea<double>(displayAreaT2);
        }

        return default;
    }

    public static IDisplayArea? ZoomIn(IDisplayArea displayArea, int i1, int j1, int i2, int j2, int horizontal, int vertical)
    {
        if (displayArea is DisplayArea<decimal> displayAreaT1)
        {
            return new DisplayArea<decimal>(displayAreaT1.GetCenterX(i1, i2), displayAreaT1.GetCenterY(j1, j2),
                displayAreaT1.GetWidth(i1, i2), displayAreaT1.GetHeight(j1, j2),
                displayAreaT1.Cx, displayAreaT1.Cy, horizontal, vertical);
        }
        else if (displayArea is DisplayArea<double> displayAreaT2)
        {
            return new DisplayArea<double>(displayAreaT2.GetCenterX(i1, i2), displayAreaT2.GetCenterY(j1, j2),
                displayAreaT2.GetWidth(i1, i2), displayAreaT2.GetHeight(j1, j2),
                displayAreaT2.Cx, displayAreaT2.Cy,
                horizontal, vertical);
        }

        return default;
    }

}
