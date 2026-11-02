namespace Static.EntityFrameworkCore.Contracts.Auditing;

/// <summary>
/// Marker interface for all auditable types.
/// </summary>
/// <remarks>
/// This interface is used to identify entities that are subject to auditing, 
/// allowing the application to apply audit-specific behavior or tracking.
/// </remarks>
public interface IAuditKind;