using Static.Domain.Common;
using Static.Domain.Enums;

namespace Static.Domain.Entities.Wbs;

/// <summary>
/// A reusable WBS template that pre-populates a new document with a methodology-specific
/// node hierarchy. Templates are read-only from the end-user perspective.
/// </summary>
public class WbsTemplate : BaseAuditableEntity
{
    /// <summary>Display name shown in the template picker.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>One-paragraph description of what this template is for.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The methodology this template is designed for.</summary>
    public WbsMethodology Methodology { get; set; }

    /// <summary>Whether this template is visible and selectable by end users.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>All nodes that make up this template's predefined hierarchy.</summary>
    public List<WbsTemplateNode> Nodes { get; set; } = [];
}
