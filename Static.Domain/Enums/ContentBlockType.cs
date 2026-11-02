namespace Static.Domain.Enums;

/// <summary>
/// Kind of teaching block shown on the Concept / Visualize steps.
/// The shape of <c>LessonContentBlock.Data</c> depends on this type.
/// </summary>
public enum ContentBlockType
{
    /// <summary>{ "term": string, "text": string }</summary>
    Definition = 1,
    /// <summary>{ "markdown": string }</summary>
    RichText = 2,
    /// <summary>{ "headers": string[], "rows": string[][] }</summary>
    Table = 3,
    /// <summary>{ "url": string, "caption": string, "alt": string }</summary>
    Image = 4,
    /// <summary>{ "url": string, "caption": string }</summary>
    Video = 5,
    /// <summary>{ "plantUml": string, "caption": string }</summary>
    Diagram = 6,
    /// <summary>{ "variant": "tip" | "warning" | "info", "text": string }</summary>
    Callout = 7,
    /// <summary>{ "items": string[] }</summary>
    KeyPoints = 8
}
