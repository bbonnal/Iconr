using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Iconr.Markup;

/// <summary>
/// XAML markup extension that provides a <see cref="DrawingImage"/> for a given
/// <see cref="Icon"/> and brush, suitable for use as an image source.
/// </summary>
/// <example>
/// <code>
/// &lt;Image Source="{iconr:IconSource Icon=github_logo, Brush={DynamicResource ForegroundBrush}}" /&gt;
/// </code>
/// </example>
public class IconSourceExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the brush used to paint the icon. Defaults to <see cref="Brushes.Black"/>.
    /// </summary>
    public IBrush Brush { get; set; } = Brushes.Black;

    /// <summary>
    /// Gets or sets the icon to render.
    /// </summary>
    public Icon Icon { get; set; }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
        => IconService.CreateDrawingImage(Icon, Brush);
}
