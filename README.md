# Iconr

Lightweight icon library for [Avalonia UI](https://avaloniaui.net). Ships 1,500+ [Phosphor](https://phosphoricons.com) icons as embedded SVGs with strongly-typed enum access, XAML markup extensions, and zero runtime dependencies beyond Avalonia.

## Install

```
dotnet add package Iconr
```

## Usage

### C#

```csharp
using Iconr;

// Get a vector geometry (for PathIcon, Path, etc.)
var geo = IconService.CreateGeometry(Icon.magnifying_glass);

// Get a DrawingImage with a specific color
var img = IconService.CreateDrawingImage(Icon.github_logo, Brushes.White);
```

### XAML

Add the namespace:

```xml
xmlns:iconr="using:Iconr.Markup"
```

Then use the markup extensions:

```xml
<!-- Geometry for PathIcon / Path -->
<PathIcon Data="{iconr:IconGeometry Icon=folder_open}" Width="20" Height="20" />

<!-- DrawingImage for Image controls -->
<Image Source="{iconr:IconSource Icon=floppy_disk, Brush={DynamicResource ForegroundBrush}}" />
```

## Adding custom icons

A helper script is included for adding new icons:

```bash
src/Iconr/scripts/create-icon.sh my-new-icon        # creates SVG + enum entry
src/Iconr/scripts/create-icon.sh my-new-icon --open  # also opens in Inkscape
```

Icons must be 256x256 SVGs with a single `<path>` element using `fill="currentColor"`.

## Icon sources

- The majority of icons come from [Phosphor Icons](https://phosphoricons.com) (MIT license).
- Some icons are original additions by the project contributors.

See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for full license details.

## License

[MIT](LICENSE)
