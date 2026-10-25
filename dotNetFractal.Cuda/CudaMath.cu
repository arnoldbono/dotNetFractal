#include <cuda_runtime.h>

extern "C" __declspec(dllexport) int InitializeCudaDevice()
{
    int deviceCount = 0;
    if (cudaGetDeviceCount(&deviceCount) != cudaSuccess || deviceCount <= 0)
    {
        return 10;
    }

    for (int deviceIndex = 0; deviceIndex < deviceCount; ++deviceIndex)
    {
        cudaDeviceProp deviceProps;
        if (cudaGetDeviceProperties(&deviceProps, deviceIndex) != cudaSuccess)
        {
            continue;
        }

        if (!deviceProps.integrated)
        {
            if (cudaSetDevice(deviceIndex) == cudaSuccess)
            {
                return 0;
            }

            break;
        }
    }

    return cudaSetDevice(0) == cudaSuccess ? 0 : 11;
}

__device__ void ComputeMandelbrotPixel(
    int i,
    int j,
    double centerX,
    double centerY,
    double viewWidth,
    double viewHeight,
    int pixelsHorizontal,
    int pixelsVertical,
    int maxIterations,
    double maxRadius,
    int& iteration,
    double& radius2,
    double& prevRadius2)
{
    double halfPixelsHorizontal = static_cast<double>(pixelsHorizontal) * 0.5;
    double halfPixelsVertical = static_cast<double>(pixelsVertical) * 0.5;

    double Cx = centerX + ((static_cast<double>(i) - halfPixelsHorizontal) * viewWidth / static_cast<double>(pixelsHorizontal));
    double Cy = centerY + ((halfPixelsVertical - static_cast<double>(j)) * viewHeight / static_cast<double>(pixelsVertical));

    double x = Cx;
    double y = Cy;

    iteration = 0;
    radius2 = 0.0;
    prevRadius2 = 0.0;

    do
    {
        ++iteration;
        prevRadius2 = radius2;

        double xx = x * x;
        double yy = y * y;

        radius2 = xx + yy;
        if (radius2 > maxRadius)
        {
            break;
        }

        y = (2.0 * x * y) + Cy;
        x = xx - yy + Cx;
    }
    while (iteration < maxIterations);
}

__global__ void MandelbrotTileKernel(
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
    int* iterations,
    double* radius2,
    double* prevRadius2)
{
    int x = blockIdx.x * blockDim.x + threadIdx.x;
    int y = blockIdx.y * blockDim.y + threadIdx.y;

    if (x >= width || y >= height)
    {
        return;
    }

    int idx = y * width + x;

    int i = startI + x;
    int j = startJ + y;

    int iteration;
    double currentRadius2;
    double currentPrevRadius2;

    ComputeMandelbrotPixel(
        i,
        j,
        centerX,
        centerY,
        viewWidth,
        viewHeight,
        pixelsHorizontal,
        pixelsVertical,
        maxIterations,
        maxRadius,
        iteration,
        currentRadius2,
        currentPrevRadius2);

    iterations[idx] = iteration;
    radius2[idx] = currentRadius2;
    prevRadius2[idx] = currentPrevRadius2;
}

extern "C" __declspec(dllexport) int ComputeMandelbrotTile(
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
    int* iterations,
    double* radius2,
    double* prevRadius2)
{
    if (width <= 0 || height <= 0 || iterations == nullptr || radius2 == nullptr || prevRadius2 == nullptr)
    {
        return 1;
    }

    int count = width * height;

    int* dIterations = nullptr;
    double* dRadius2 = nullptr;
    double* dPrevRadius2 = nullptr;

    if (cudaMalloc(&dIterations, count * sizeof(int)) != cudaSuccess)
    {
        return 2;
    }

    if (cudaMalloc(&dRadius2, count * sizeof(double)) != cudaSuccess)
    {
        cudaFree(dIterations);
        return 3;
    }

    if (cudaMalloc(&dPrevRadius2, count * sizeof(double)) != cudaSuccess)
    {
        cudaFree(dIterations);
        cudaFree(dRadius2);
        return 4;
    }

    dim3 block(16, 16);
    dim3 grid((width + block.x - 1) / block.x, (height + block.y - 1) / block.y);

    MandelbrotTileKernel<<<grid, block>>>(
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
        dIterations,
        dRadius2,
        dPrevRadius2);

    cudaError_t launchError = cudaGetLastError();
    if (launchError != cudaSuccess)
    {
        cudaFree(dIterations);
        cudaFree(dRadius2);
        cudaFree(dPrevRadius2);
        return 5000 + static_cast<int>(launchError);
    }

    cudaError_t syncError = cudaDeviceSynchronize();
    if (syncError != cudaSuccess)
    {
        cudaFree(dIterations);
        cudaFree(dRadius2);
        cudaFree(dPrevRadius2);
        return 6000 + static_cast<int>(syncError);
    }

    if (cudaMemcpy(iterations, dIterations, count * sizeof(int), cudaMemcpyDeviceToHost) != cudaSuccess ||
        cudaMemcpy(radius2, dRadius2, count * sizeof(double), cudaMemcpyDeviceToHost) != cudaSuccess ||
        cudaMemcpy(prevRadius2, dPrevRadius2, count * sizeof(double), cudaMemcpyDeviceToHost) != cudaSuccess)
    {
        cudaFree(dIterations);
        cudaFree(dRadius2);
        cudaFree(dPrevRadius2);
        return 6;
    }

    cudaFree(dIterations);
    cudaFree(dRadius2);
    cudaFree(dPrevRadius2);

    return 0;
}
