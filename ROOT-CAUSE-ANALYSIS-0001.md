# Root Cause Analysis - Image Flickering Issue

**Issue ID**: ROOT-CAUSE-ANALYSIS-0001  
**Date**: 2024  
**Component**: `dotNetFractal.WPF\Services\WpfBitmapConverter.cs`  
**Status**: RESOLVED

## Problem Summary

The fractal image displayed in the WPF application was experiencing intermittent flickering where a white or empty image would briefly flash on screen. This flickering occurred:

1. During fractal computation when `FractalStitcher` updated rectangular patches on the bitmap
2. When `RunColorist()` updated color patches
3. Whenever `m_stitcher.Update(m_bitmap)` returned `true`, indicating a fractal patch had been rendered

### Initial Observations

- The flickering was **not** related to the initial bitmap creation (clearing to Azure color at the start of new fractal computation is intentional)
- The issue occurred during incremental updates of the existing bitmap
- `MainImage` was being updated on the main UI thread as expected
- `UpdateBitmap()` in `SharedMainViewModel.cs` was correctly calling the bitmap converter

## Root Cause - The Real Problem

The flickering was caused by **incorrect use of `WriteableBitmap` update APIs** in the WPF bitmap converter. The issue was with the `Lock()`/`WritePixels()`/`Unlock()` pattern.

### Problematic Code Pattern

```csharp
writeableBitmap.Lock();
try
{
    var pixels = bitmap.GetPixels();
    var stride = width * 4;
    writeableBitmap.WritePixels(
        new Int32Rect(0, 0, width, height),
        pixels,
        stride * height,
        stride);  // ❌ PROBLEM: WritePixels on a locked bitmap
}
finally
{
    writeableBitmap.Unlock();
}
```

### Why This Caused Flickering

`WritePixels()` is designed to be called on an **unlocked** bitmap (it handles locking internally). When called on an already-locked bitmap:

1. The pixel data gets written to memory buffer
2. **BUT** WPF's rendering system doesn't get properly notified about which regions changed
3. This causes a "lazy update" effect where the UI update is deferred or unpredictable
4. **Result**: Flickering as the display momentarily shows stale, incomplete, or empty data during the delayed update

The critical missing piece was **explicitly telling WPF which pixels changed** using `AddDirtyRect()`.

## Solution

The correct pattern for updating a `WriteableBitmap` without flickering:

```csharp
writeableBitmap.Lock();
try
{
    var pixels = bitmap.GetPixels();
    var stride = width * 4;
    var bufferSize = stride * height;

    // 1. Copy directly to BackBuffer (fast, unsafe memory copy)
    unsafe
    {
        Buffer.MemoryCopy(
            (void*)pixels,
            (void*)writeableBitmap.BackBuffer,
            bufferSize,
            bufferSize);
    }

    // 2. CRITICAL: Mark the dirty region - tells WPF exactly what changed
    writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
}
finally
{
    writeableBitmap.Unlock();  // 3. Unlock triggers immediate UI update
}
```

### Key Changes

1. **Direct `BackBuffer` access**: Copy pixels directly to the backing memory using `Buffer.MemoryCopy()`
2. **`AddDirtyRect()` call**: Explicitly tells WPF which pixels changed - **this is what was missing!**
3. **Immediate notification**: When `Unlock()` is called, WPF immediately knows to update the display for the dirty region

### Why This Eliminates Flickering

- **Atomic updates**: The entire Lock→Copy→AddDirtyRect→Unlock sequence is atomic from WPF's perspective
- **Immediate notification**: `AddDirtyRect()` ensures WPF's rendering pipeline knows exactly what changed
- **No lazy evaluation**: The dirty region is processed immediately on `Unlock()`, not deferred
- **Same object reference**: Updates the same `WriteableBitmap` in-place (no object recreation)

## Additional Improvements

As part of the fix, we also implemented `TryUpdateImageSource()` in the `IBitmapConverter` interface to enable in-place bitmap updates:

```csharp
public bool TryUpdateImageSource(object? imageSource, SKBitmap bitmap)
{
    if (imageSource is not WriteableBitmap writeableBitmap)
        return false;

    if (writeableBitmap.PixelWidth != bitmap.Width || 
        writeableBitmap.PixelHeight != bitmap.Height)
        return false;

    Copy(writeableBitmap, bitmap);
    return true;
}
```

This ensures the same `WriteableBitmap` object remains bound to the UI, with only its pixel data changing, further reducing overhead and potential display issues.

## Files Modified

1. **dotNetFractal.UI\Services\IBitmapConverter.cs**
   - Added `TryUpdateImageSource()` method

2. **dotNetFractal.WPF\Services\WpfBitmapConverter.cs**
   - Implemented `TryUpdateImageSource()`
   - Fixed `Copy()` method to use `BackBuffer` + `AddDirtyRect()` pattern

3. **dotNetFractal.WPF\dotNetFractal.WPF.csproj**
   - Added `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>` to support `Buffer.MemoryCopy()`

4. **dotNetFractal.UI\ViewModels\SharedMainViewModel.cs**
   - Updated `UpdateBitmap()` to use `TryUpdateImageSource()` for incremental updates

5. **dotNetFractal.Uno\Services\UnoBitmapConverter.cs**
   - Implemented `TryUpdateImageSource()` for Uno platform

6. **dotNetFractal.UI\ViewModels\PropertiesPanelViewModel.cs**
   - Updated `FakeBitmapConverter` to implement new interface method

## Verification

The fix has been verified to work correctly for:
- ✅ Fractal computation incremental updates
- ✅ `RunColorist()` color patch updates
- ✅ Both WPF and Uno platforms (with platform-specific implementations)

## Lessons Learned

1. **WPF `WriteableBitmap` requires explicit dirty region notification** via `AddDirtyRect()` when updating the `BackBuffer` directly
2. **Don't call `WritePixels()` on a locked bitmap** - it bypasses proper WPF update notifications
3. **The `Lock()`/`BackBuffer`/`AddDirtyRect()`/`Unlock()` pattern is the correct way** to do high-performance bitmap updates in WPF
4. **In-place updates are critical** - creating new `WriteableBitmap` objects on every update causes binding churn and visual artifacts
