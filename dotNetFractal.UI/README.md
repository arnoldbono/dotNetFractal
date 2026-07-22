# dotNetFractal.UI - Shared UI Layer

This project contains UI code shared between dotNetFractal.WPF and dotNetFractal.Uno.

## Purpose
Eliminate code duplication (DRY principle) by consolidating:
- ViewModels
- Models (enums, data classes)
- Commands
- Services/Interfaces

## Migrated Files

### Models ✅
- ✅ ResolutionEnum.cs
- ✅ FractalPlate.cs + JuliaFractalPlate
- ✅ EditableFractalColor.cs

### Commands ✅
- ✅ RelayCommand.cs (platform-agnostic version with RaiseCanExecuteChanged)

### ViewModels ✅
- ✅ BaseViewModel.cs
- ✅ DisplaySettingsViewModel.cs  
- ✅ ImageResolutionViewModel.cs
- ✅ FractalAreaViewModel.cs + FractalAreaViewModelBase<T>
- ✅ FractalSettingsViewModel.cs
- ✅ ColorMapViewModel.cs (uses IBitmapConverter for platform abstraction)
- ✅ PropertiesPanelViewModel.cs
- ✅ DistributionGraphViewModel.cs + DistributionGraphPoint

### Services/Interfaces ✅
- ✅ IBitmapConverter - Interface for platform-specific image conversion

## Platform-Specific Implementations Required

### 1. IBitmapConverter Implementation
Each platform must implement `dotNetFractal.UI.Services.IBitmapConverter`:

**WPF (dotNetFractal.WPF/Services/WpfBitmapConverter.cs):**
```csharp
public class WpfBitmapConverter : IBitmapConverter
{
    public object ConvertToImageSource(SKBitmap bitmap)
    {
        return ConvertBitmapToImageSource.Clone(bitmap);
    }
}
```

**Uno (dotNetFractal.Uno/Services/UnoBitmapConverter.cs):**
```csharp
public class UnoBitmapConverter : IBitmapConverter
{
    public object ConvertToImageSource(SKBitmap bitmap)
    {
        return ConvertBitmapToImageSource.Clone(bitmap);
    }
}
```

### 2. Converters
Converters remain in their respective projects due to different base interfaces:
- WPF: `System.Windows.Data.IValueConverter`
- Uno: `Microsoft.UI.Xaml.Data.IValueConverter`

**Converters to keep in each project:**
- EnumBooleanConverter (uses DependencyProperty.UnsetValue)
- BoolToVisibilityConverter
- BoolToAlignmentConverter  
- BoolToStretchConverter
- BoolToScrollBarVisibilityConverter
- BoolToColumnSpanConverter
- BoolToCollapseSymbolConverter
- FractalUnitConverter
- InverseBooleanConverter
- InverseBoolToVisibilityConverter

**Note:** Ensure Uno converters use the fixed EnumBooleanConverter implementation with type caching.

### 3. Platform-Specific Code Remaining in Projects

**ConvertBitmapToImageSource**
- `dotNetFractal.WPF/ConvertBitmapToImageSource.cs` - WPF WriteableBitmap
- `dotNetFractal.Uno/ConvertBitmapToImageSource.cs` - Uno WriteableBitmap

**MainViewModel**
- Remains in both projects due to platform-specific:
  - File pickers (OpenFileDialog vs StorageFilePicker)
  - Dialogs (MessageBox vs ContentDialog)
  - Clipboard APIs
  - Window management

**App.xaml.cs**
- Platform entry points remain separate

## Migration Steps Completed

1. ✅ Created dotNetFractal.UI project (.NET 10 class library)
2. ✅ Added project references from WPF and Uno to UI project
3. ✅ Migrated all ViewModels to shared project
4. ✅ Migrated Models (ResolutionEnum, FractalPlate, EditableFractalColor)
5. ✅ Created platform-agnostic RelayCommand
6. ✅ Created IBitmapConverter interface for platform abstraction
7. ✅ Created WpfBitmapConverter and UnoBitmapConverter implementations
8. ✅ Updated MainViewModel in both projects to inject IBitmapConverter
9. ✅ Updated using statements in both projects
10. ✅ Excluded duplicate files from compilation in both projects
11. ✅ Updated XAML namespace references (WPF ImageResolutionEditor.xaml)
12. ✅ Updated EnumBooleanConverter in both projects to reference shared ResolutionEnum
13. ✅ Verified all projects build successfully

## Next Steps (Manual Testing)

1. **Test WPF Project:**
   ✅ Run dotNetFractal.WPF
   ✅ Test fractal computation
   ✅ Test property panel (resolution, fractal area, color map)
   ✅ Test distribution graph
   ✅ Test file operations (open, save, copy)

2. **Test Uno Project:**
   ✅ Run dotNetFractal.Uno
   ✅ Test fractal computation
   - Test property panel (resolution, fractal area, color map)
   ✅ Test file operations
   - Test on different platforms (if multi-targeted)

3. **Optional Cleanup:**
   ✅ Delete duplicate ViewModel files from disk (they're already excluded from compilation)
   ✅ Delete duplicate model files from disk (they're already excluded from compilation)
   - Consider if any remaining converters could be consolidated

## Defects

- WPF: Color Map: Delete button does not work
- WPF&Uno: When zooming in, the selected area is the basis of the blown-up bitmap shown at the start of the new fractal computation. It is incorrect at deeper zoom levels.
- Uno: Slider between panel area and fractal image is missing
- Uno: Color Map: table only show Red and Green values
- Uno: Individual panes in Properties do not grow to the available width
- Uno: Hotkeys F2, F10, F11 do not work
- Uno: Collapse button on Properties is missing
- Uno: Esc button does not work when zooming in or out.
- Uno: Color Map Values cannot be modified.

## Benefits Achieved

- **~2000 lines of code** deduplicated
- **Single source of truth** for business logic and ViewModels
- **Easier maintenance** - fixes apply to both platforms simultaneously
- **Better architecture** - clear separation of shared vs platform-specific code
- **Type safety** - compiler ensures consistency across platforms

