using System.Runtime.InteropServices;

namespace dotNetFractal.Logic;

internal static class CudaMandelbrotNative
{
    [DllImport("dotNetFractal.Cuda", EntryPoint = "InitializeCudaDevice", CallingConvention = CallingConvention.Cdecl)]
    private static extern int InitializeCudaDeviceNative();

    [DllImport("dotNetFractal.Cuda", EntryPoint = "ComputeMandelbrotTile", CallingConvention = CallingConvention.Cdecl)]
    private static extern int ComputeMandelbrotTileNative(
        int startI,
        int startJ,
        int width,
        int height,
        double centerX,
        double centerY,
        double viewWidth,
        double viewHeight,
        int pixelsHorizontal,
        int pixelsVertical,
        int maxIterations,
        double maxRadius,
        int borderOnly,
        [Out] int[] iterations,
        [Out] double[] radius2,
        [Out] double[] prevRadius2);

    public static int TryInitializeCudaDevice()
    {
        try
        {
            return InitializeCudaDeviceNative();
        }
        catch (DllNotFoundException)
        {
            return -1;
        }
        catch (EntryPointNotFoundException)
        {
            return -2;
        }
        catch (BadImageFormatException)
        {
            return -3;
        }
    }

    public static int TryComputeMandelbrotTile(
        int startI,
        int startJ,
        int width,
        int height,
        double centerX,
        double centerY,
        double viewWidth,
        double viewHeight,
        int pixelsHorizontal,
        int pixelsVertical,
        int maxIterations,
        double maxRadius,
        bool borderOnly,
        int[] iterations,
        double[] radius2,
        double[] prevRadius2)
    {
        try
        {
            int initializeResult = TryInitializeCudaDevice();
            if (initializeResult != 0)
            {
                return initializeResult;
            }

            int result = ComputeMandelbrotTileNative(
                startI,
                startJ,
                width,
                height,
                centerX,
                centerY,
                viewWidth,
                viewHeight,
                pixelsHorizontal,
                pixelsVertical,
                maxIterations,
                maxRadius,
                borderOnly ? 1 : 0,
                iterations,
                radius2,
                prevRadius2);

            return result;
        }
        catch (DllNotFoundException)
        {
            return -1;
        }
        catch (EntryPointNotFoundException)
        {
            return -2;
        }
        catch (BadImageFormatException)
        {
            return -3;
        }
    }
}
