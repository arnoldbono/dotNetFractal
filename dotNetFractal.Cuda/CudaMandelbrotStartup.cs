namespace dotNetFractal.Cuda;

public static class CudaMandelbrotStartup
{
    public static bool TrySelectCudaDevice()
    {
        return CudaMandelbrotNative.TryInitializeCudaDevice() == 0;
    }
}
