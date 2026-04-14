using System.Collections.Concurrent;
using System.Reflection;
using System.Xml;
using Avalonia.Media;

namespace Iconr;

/// <summary>
/// Provides static methods for loading and rendering icons from embedded SVG resources.
/// </summary>
/// <remarks>
/// Reads SVG icon files embedded in the assembly, extracts vector path data,
/// and converts them into Avalonia <see cref="Geometry"/> and <see cref="DrawingImage"/> objects.
/// Parsed path data is cached in-process for the lifetime of the application.
/// </remarks>
public static class IconService
{
    private static readonly Assembly Assembly = typeof(IconService).Assembly;
    private static readonly ConcurrentDictionary<Icon, string> PathDataCache = new();

    /// <summary>
    /// Converts an icon enum value to its kebab-case file name.
    /// </summary>
    private static string GetIconName(Icon icon)
        => $"{icon}".Replace('_', '-');

    /// <summary>
    /// Constructs the fully-qualified manifest resource name for the icon.
    /// </summary>
    private static string GetResourceName(Icon icon)
        => $"Iconr.Icons.{GetIconName(icon)}.svg";

    /// <summary>
    /// Retrieves the embedded resource stream for the specified icon.
    /// </summary>
    /// <param name="icon">The icon to load.</param>
    /// <returns>A stream containing the SVG data, or <c>null</c> if the icon is not found.</returns>
    public static Stream? GetIconStream(Icon icon)
        => Assembly.GetManifestResourceStream(GetResourceName(icon));

    /// <summary>
    /// Extracts the SVG path data (<c>d</c> attribute) from an icon's embedded SVG file.
    /// </summary>
    /// <param name="icon">The icon to read.</param>
    /// <returns>The raw SVG path data string.</returns>
    /// <exception cref="InvalidOperationException">
    /// The icon resource was not found or does not contain a valid <c>&lt;path&gt;</c> element.
    /// </exception>
    public static string GetIconData(Icon icon)
    {
        using var stream = GetIconStream(icon)
                           ?? throw new InvalidOperationException($"Icon '{icon}' not found");
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        var xml = new XmlDocument();
        xml.LoadXml(content);

        // Handle both namespaced and plain SVG documents
        var nsm = new XmlNamespaceManager(xml.NameTable);
        nsm.AddNamespace("svg", "http://www.w3.org/2000/svg");

        var node = xml.SelectSingleNode("//svg:path", nsm)
                   ?? xml.SelectSingleNode("//path");

        if (node?.Attributes?["d"]?.Value is { } data)
            return data;

        throw new InvalidOperationException($"Cannot extract path data from icon '{GetIconName(icon)}'");
    }

    /// <summary>
    /// Creates an Avalonia <see cref="Geometry"/> for the specified icon.
    /// The underlying path data is cached after the first parse.
    /// </summary>
    /// <param name="icon">The icon to render.</param>
    /// <returns>A <see cref="Geometry"/> containing the icon's vector path.</returns>
    public static Geometry CreateGeometry(Icon icon)
    {
        var data = PathDataCache.GetOrAdd(icon, static i => GetIconData(i));
        return Geometry.Parse(data);
    }

    /// <summary>
    /// Creates a <see cref="DrawingImage"/> with the specified icon and brush.
    /// </summary>
    /// <param name="icon">The icon to render.</param>
    /// <param name="brush">The brush used to fill the icon.</param>
    /// <returns>A <see cref="DrawingImage"/> suitable for use as an image source.</returns>
    public static DrawingImage CreateDrawingImage(Icon icon, IBrush brush)
    {
        return new DrawingImage(new GeometryDrawing
        {
            Geometry = CreateGeometry(icon),
            Brush = brush
        });
    }
}
