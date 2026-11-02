namespace Static.Domain.Enums;

/// <summary>
/// Semantic type of a WBS node.
/// Used by the FE to render different shapes / icons on the canvas.
/// </summary>
public enum WbsNodeType
{
    /// <summary>Top-level project node (root).</summary>
    Project     = 0,

    /// <summary>A major phase of work (e.g. "Planning", "Development").</summary>
    Phase       = 1,

    /// <summary>A tangible, verifiable outcome (e.g. "Technical Design Document").</summary>
    Deliverable = 2,

    /// <summary>The smallest unit of schedulable work assigned to a person or team.</summary>
    WorkPackage = 3,

    /// <summary>A significant point in time with no duration (e.g. "Go-Live").</summary>
    Milestone   = 4,

    /// <summary>A roll-up node that summarises child work (no direct work associated).</summary>
    Summary     = 5
}
