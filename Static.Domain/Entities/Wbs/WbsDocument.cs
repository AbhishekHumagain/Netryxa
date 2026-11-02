using Static.Domain.Common;
using Static.Domain.Enums;

namespace Static.Domain.Entities.Wbs;

/// <summary>
/// A user-owned WBS document that holds a tree of <see cref="WbsNode"/> entries.
/// Can be created from scratch or seeded from a <see cref="WbsTemplate"/>.
/// </summary>
public class WbsDocument : BaseAuditableEntity
{
    /// <summary>Human-readable project title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional free-text description of the project / document scope.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The software development methodology this WBS follows.</summary>
    public WbsMethodology Methodology { get; set; }

    /// <summary>Lifecycle status of this document.</summary>
    public WbsDocumentStatus Status { get; set; } = WbsDocumentStatus.Draft;

    /// <summary>Version label (e.g. "1.0", "2.3-draft").</summary>
    public string Version { get; set; } = "1.0";

    /// <summary>Keycloak user ID of the document creator (owner-scoped access).</summary>
    public Guid OwnerId { get; set; }

    /// <summary>Optional reference to the template this document was seeded from.</summary>
    public Guid? TemplateId { get; set; }

    /// <summary>All nodes in this document's WBS tree.</summary>
    public List<WbsNode> Nodes { get; set; } = [];

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
