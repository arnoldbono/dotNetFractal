namespace dotNetFractal.Logic;

public static class CudaMandelbrotStartup
{
    public static bool TrySelectCudaDevice()
    {
        return CudaMandelbrotNative.TryInitializeCudaDevice() == 0;
    }
}
