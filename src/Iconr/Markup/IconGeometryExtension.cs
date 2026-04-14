using Avalonia.Markup.Xaml;

namespace Iconr.Markup;

/// <summary>
/// XAML markup extension that provides an Avalonia <see cref="Avalonia.Media.Geometry"/>
/// for a given <see cref="Icon"/> value.
/// </summary>
/// <example>
/// <code>
/// &lt;Path Data="{iconr:IconGeometry Icon=magnifying_glass}" Fill="Black" /&gt;
/// </code>
/// </example>
public class IconGeometryExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the icon to render.
    /// </summary>
    public Icon Icon { get; set; }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
        => IconService.CreateGeometry(Icon);
}
