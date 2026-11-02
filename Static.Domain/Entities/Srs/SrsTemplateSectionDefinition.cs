using Static.Domain.Common;

namespace Static.Domain.Entities.Srs;

/// <summary>
/// Defines a single section slot within an SRS template (its title, key, sample content, and guidance).
/// </summary>
public class SrsTemplateSectionDefinition : BaseAuditableEntity
{
    /// <summary>Foreign key to the owning template.</summary>
    public Guid TemplateId { get; set; }

    /// <summary>Human-readable section title (e.g. "Introduction").</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Machine-friendly slug identifying this section (e.g. "introduction").</summary>
    public string SectionKey { get; set; } = string.Empty;

    /// <summary>Markdown text pre-filled as a starting point for the author.</summary>
    public string SampleContent { get; set; } = string.Empty;

    /// <summary>Short description of what the author should write in this section.</summary>
    public string GuidanceNotes { get; set; } = string.Empty;

    /// <summary>1-based display order within the template.</summary>
    public int OrderIndex { get; set; }

    /// <summary>Whether this section must be present in any document created from this template.</summary>
    public bool IsRequired { get; set; }

    /// <summary>Navigation property back to the parent template.</summary>
    public SrsTemplate Template { get; set; } = null!;
}
