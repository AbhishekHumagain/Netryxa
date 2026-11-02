namespace Static.Application.Common.Interfaces;

/// <summary>
/// Renders PlantUML source text to SVG by calling the PlantUML server.
/// </summary>
public interface IPlantUmlService
{
    /// <summary>
    /// Encode and render <paramref name="umlContent"/> to an SVG string.
    /// Returns <c>null</c> when the render service is unreachable.
    /// </summary>
    Task<string?> RenderToSvgAsync(string umlContent, CancellationToken cancellationToken = default);
}
