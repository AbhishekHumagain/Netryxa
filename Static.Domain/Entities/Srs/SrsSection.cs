using Static.Domain.Common;

namespace Static.Domain.Entities.Srs;

/// <summary>
/// A single content section (written in Markdown) within an SRS document.
/// </summary>
public class SrsSection : BaseAuditableEntity
{
    /// <summary>Foreign key to the owning document.</summary>
    public Guid DocumentId { get; set; }

    /// <summary>Section heading shown to the reader.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Markdown body of this section.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Optional machine-friendly slug (e.g. "introduction"). May be empty.</summary>
    public string SectionKey { get; set; } = string.Empty;

    /// <summary>1-based display order within the document.</summary>
    public int OrderIndex { get; set; }

    /// <summary>Whether this section is mandatory (carried over from the template definition).</summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>Navigation property back to the parent document.</summary>
    public SrsDocument Document { get; set; } = null!;
}
