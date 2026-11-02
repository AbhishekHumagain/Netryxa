namespace Static.Domain.Enums;

/// <summary>Execution status of a WBS node (work package or deliverable).</summary>
public enum WbsNodeStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed  = 2,
    Blocked    = 3,
    OnHold     = 4
}
