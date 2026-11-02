using Static.Domain.Common;
using Static.Domain.Enums;

namespace Static.Domain.Entities.Wbs;

/// <summary>
/// A single node in a WBS document's tree.
/// Nodes reference each other via <see cref="ParentNodeId"/> (null = root).
///
/// Position fields (<see cref="PositionX"/>, <see cref="PositionY"/>) support
/// a draw.io-style canvas where the user can drag nodes to custom coordinates.
/// When null the FE applies an automatic tree-layout algorithm.
/// </summary>
public class WbsNode : BaseAuditableEntity
{
    /// <summary>Foreign key to the owning document.</summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// Parent node within this document's tree.
    /// Null means this is a root-level node (there should typically be exactly one).
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    /// <summary>Node label displayed on the canvas and in the outline panel.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional rich-text description / notes attached to this node.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Semantic category that drives the FE shape/icon.</summary>
    public WbsNodeType NodeType { get; set; } = WbsNodeType.WorkPackage;

    /// <summary>Execution progress of this node.</summary>
    public WbsNodeStatus Status { get; set; } = WbsNodeStatus.NotStarted;

    /// <summary>Sort order among siblings (children of the same parent).</summary>
    public int OrderIndex { get; set; }

    /// <summary>Depth in the tree (root = 0). Updated when the node is moved.</summary>
    public int Level { get; set; }

    /// <summary>
    /// WBS numbering code (e.g. "1.0", "1.2", "1.3.1").
    /// Set by the user; not auto-computed by the API.
    /// </summary>
    public string WbsCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional hex colour override (e.g. "#4F91FF") for the canvas node.
    /// Null means the FE uses its default colour for the <see cref="NodeType"/>.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>Estimated duration in calendar days (for Gantt / planning view).</summary>
    public int? DurationDays { get; set; }

    /// <summary>Free-text person or team responsible for this node.</summary>
    public string? AssignedTo { get; set; }

    /// <summary>
    /// Horizontal canvas position in pixels.
    /// Null = FE computes automatic layout.
    /// </summary>
    public double? PositionX { get; set; }

    /// <summary>
    /// Vertical canvas position in pixels.
    /// Null = FE computes automatic layout.
    /// </summary>
    public double? PositionY { get; set; }

    /// <summary>Navigation property to the owning document.</summary>
    public WbsDocument Document { get; set; } = null!;
}
