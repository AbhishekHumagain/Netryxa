using Static.Domain.Common;
using Static.Domain.Enums;

namespace Static.Domain.Entities.Srs;

/// <summary>
/// A user-authored SRS document, optionally created from a template.
/// </summary>
public class SrsDocument : BaseAuditableEntity
{
    /// <summary>Document title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Short description / purpose statement for this document.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Development methodology this document follows.</summary>
    public SrsMethodology Methodology { get; set; }

    /// <summary>Current lifecycle status of the document.</summary>
    public SrsDocumentStatus Status { get; set; } = SrsDocumentStatus.Draft;

    /// <summary>Semantic version string (e.g. "1.0", "2.3").</summary>
    public string Version { get; set; } = "1.0";

    /// <summary>User ID of the person who owns (created) this document.</summary>
    public Guid OwnerId { get; set; }

    /// <summary>Optional reference to the template this document was created from.</summary>
    public Guid? TemplateId { get; set; }

    /// <summary>Ordered sections that make up the body of this document.</summary>
    public List<SrsSection> Sections { get; set; } = [];

    /// <summary>
    /// When true, this document is visible to all authenticated users (not just the owner).
    /// Only meaningful for teacher-owned documents — students cannot publish.
    /// </summary>
    public bool IsPublished { get; set; } = false;

    /// <summary>
    /// True when the creator held the Teacher role at the time of creation.
    /// Used together with <see cref="IsPublished"/> to determine student visibility.
    /// </summary>
    public bool IsTeacherContent { get; set; } = false;
}
