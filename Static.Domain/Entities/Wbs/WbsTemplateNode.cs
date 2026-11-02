using Static.Domain.Common;
using Static.Domain.Enums;

namespace Static.Domain.Entities.Wbs;

/// <summary>
/// A single node in a WBS template's predefined hierarchy.
/// Nodes form a tree via <see cref="ParentNodeId"/> (null = root).
/// </summary>
public class WbsTemplateNode : BaseAuditableEntity
{
    /// <summary>Foreign key to the owning template.</summary>
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Parent node within this template's tree.
    /// Null means this is a root-level node.
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    /// <summary>Node label displayed on the canvas / outline.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional description or guidance for this node.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Semantic category that drives shape/icon in the FE renderer.</summary>
    public WbsNodeType NodeType { get; set; } = WbsNodeType.WorkPackage;

    /// <summary>
    /// Standard WBS code notation (e.g. "1.0", "1.2", "1.3.1").
    /// Copied verbatim when a document is created from this template.
    /// </summary>
    public string WbsCode { get; set; } = string.Empty;

    /// <summary>Sort order among siblings (children of the same parent).</summary>
    public int OrderIndex { get; set; }

    /// <summary>Depth in the tree (root = 0, its children = 1, etc.).</summary>
    public int Level { get; set; }

    /// <summary>Optional default hex colour (e.g. "#4F91FF") applied when cloning into a document.</summary>
    public string? Color { get; set; }

    /// <summary>Navigation property to the owning template.</summary>
    public WbsTemplate Template { get; set; } = null!;
}
