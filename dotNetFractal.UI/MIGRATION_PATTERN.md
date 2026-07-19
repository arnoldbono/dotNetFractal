# Migration Pattern Guide

This document describes the pattern used to migrate code from platform-specific projects (WPF/Uno) to the shared `dotNetFractal.UI` project.

## When to Share Code

### ✅ Move to dotNetFractal.UI
- ViewModels that contain only business logic
- Models (DTOs, enums, data classes)
- Commands (ICommand implementations)
- Services that can be abstracted via interfaces
- Pure computation logic (no platform APIs)

### ❌ Keep in Platform Projects
- ViewModels that use platform-specific APIs:
  - File pickers (OpenFileDialog vs StorageFilePicker)
  - Message boxes (MessageBox vs ContentDialog)
  - Clipboard APIs
  - Window management
- Converters (different base interfaces: WPF's IValueConverter vs Uno's IValueConverter)
- Image conversion utilities (WriteableBitmap implementations)
- Platform entry points (App.xaml.cs, Program.cs)

## Migration Pattern

### Step 1: Identify Shared Code
Look for files that exist in both `dotNetFractal.WPF` and `dotNetFractal.Uno` with identical or near-identical logic.

### Step 2: Abstract Platform Dependencies
If code uses platform-specific types (like `ImageSource`), create an interface:

```csharp
// dotNetFractal.UI/Services/IMyService.cs
public interface IMyService
{
    object DoSomethingPlatformSpecific();
}
```

Then implement in each platform:
```csharp
// WPF
public class WpfMyService : IMyService
{
    public object DoSomethingPlatformSpecific()
    {
        // WPF-specific implementation
    }
}

// Uno
public class UnoMyService : IMyService
{
    public object DoSomethingPlatformSpecific()
    {
        // Uno-specific implementation
    }
}
```

### Step 3: Move Shared Code
1. Create the file in `dotNetFractal.UI` with appropriate folder structure:
   - ViewModels → `dotNetFractal.UI/ViewModels/`
   - Models → `dotNetFractal.UI/Models/`
   - Commands → `dotNetFractal.UI/Commands/`
   - Services → `dotNetFractal.UI/Services/`

2. Update namespace:
   ```csharp
   namespace dotNetFractal.UI.ViewModels; // or Models, Commands, etc.
   ```

3. Remove platform-specific usings:
   - Remove `System.Windows.*` (WPF)
   - Remove `Microsoft.UI.*` (Uno)
   - Remove `Windows.*` (Uno)

4. Replace platform types with abstractions:
   - `ImageSource` → `object` (with IBitmapConverter)
   - `ICommand` → Use shared `RelayCommand`
   - Platform enums → Shared enums

### Step 4: Update Platform Projects

#### In Project Files (.csproj)
```xml
<ItemGroup>
  <Compile Remove="ViewModels\SharedViewModel.cs" />
  <Compile Remove="Models\SharedModel.cs" />
</ItemGroup>
```

#### In Consuming Code
```csharp
// Add using statements
using dotNetFractal.UI.ViewModels;
using dotNetFractal.UI.Models;
using dotNetFractal.UI.Commands;

// Inject platform-specific services
var myViewModel = new SharedViewModel(new WpfMyService());
```

#### In XAML (if needed)
```xml
<!-- Add namespace -->
xmlns:models="clr-namespace:dotNetFractal.UI.Models;assembly=dotNetFractal.UI"

<!-- Use in bindings -->
<RadioButton IsChecked="..." ConverterParameter="{x:Static models:MyEnum.Value}" />
```

### Step 5: Verify
1. Build `dotNetFractal.UI` project
2. Build WPF project
3. Build Uno project
4. Build entire solution
5. Test runtime behavior

## Real Example: ColorMapViewModel

### Original Problem
`ColorMapViewModel` generated a color map bitmap and converted it to platform-specific `ImageSource`:
- WPF: `System.Windows.Media.ImageSource`
- Uno: `Microsoft.UI.Xaml.Media.ImageSource`

### Solution: Interface Abstraction

#### 1. Created Interface
```csharp
// dotNetFractal.UI/Services/IBitmapConverter.cs
public interface IBitmapConverter
{
    object ConvertToImageSource(SKBitmap bitmap);
}
```

#### 2. Updated Shared ViewModel
```csharp
// dotNetFractal.UI/ViewModels/ColorMapViewModel.cs
public class ColorMapViewModel : BaseViewModel
{
    private readonly IBitmapConverter _bitmapConverter;

    public ColorMapViewModel(IBitmapConverter bitmapConverter)
    {
        _bitmapConverter = bitmapConverter;
    }

    public object ColorMapImage => 
        _bitmapConverter.ConvertToImageSource(GenerateColorMapBitmap());
}
```

#### 3. Implemented Platform-Specific Converters
```csharp
// WPF
public class WpfBitmapConverter : IBitmapConverter
{
    public object ConvertToImageSource(SKBitmap bitmap)
    {
        return ConvertBitmapToImageSource.ConvertFast(bitmap);
    }
}

// Uno
public class UnoBitmapConverter : IBitmapConverter
{
    public object ConvertToImageSource(SKBitmap bitmap)
    {
        return ConvertBitmapToImageSource.ConvertFast(bitmap);
    }
}
```

#### 4. Injected in MainViewModel
```csharp
// WPF
m_colorMap = new ColorMapViewModel(new WpfBitmapConverter());

// Uno
m_colorMap = new ColorMapViewModel(new UnoBitmapConverter());
```

## Common Pitfalls

### 1. Forgetting to Update XAML Namespaces
**Problem:** XAML still references old namespace
```xml
<!-- Wrong -->
ConverterParameter="{x:Static local:ResolutionEnum.FullHD}"
```

**Solution:** Add new namespace and update reference
```xml
<!-- Correct -->
xmlns:models="clr-namespace:dotNetFractal.UI.Models;assembly=dotNetFractal.UI"
ConverterParameter="{x:Static models:ResolutionEnum.FullHD}"
```

### 2. Missing Using Statements
**Problem:** Code doesn't compile because shared types aren't imported
```csharp
CS0246: The type or namespace name 'ResolutionEnum' could not be found
```

**Solution:** Add using statement
```csharp
using dotNetFractal.UI.Models;
```

### 3. Converters Can't Be Shared
**Problem:** Trying to share converters between WPF and Uno

**Reality:** Converter interfaces are different:
- WPF: `System.Windows.Data.IValueConverter`
- Uno: `Microsoft.UI.Xaml.Data.IValueConverter`

**Solution:** Keep converters in platform projects, but they CAN reference shared models/enums

### 4. Forgetting to Exclude Files
**Problem:** Compilation errors due to duplicate type definitions

**Solution:** Add to .csproj:
```xml
<ItemGroup>
  <Compile Remove="ViewModels\DuplicateViewModel.cs" />
</ItemGroup>
```

## Benefits of This Pattern

1. **DRY Principle** - Write once, use everywhere
2. **Type Safety** - Compiler catches inconsistencies
3. **Maintainability** - Bug fixes apply to all platforms
4. **Testability** - Shared code can be unit tested once
5. **Clear Boundaries** - Obvious what's shared vs platform-specific

## Future Migrations

When adding new features:
1. Ask: "Is this platform-specific or business logic?"
2. If business logic → Add to `dotNetFractal.UI`
3. If platform-specific → Add to platform project
4. If mixed → Abstract with interface pattern

Keep this pattern consistent to maintain clean architecture.
