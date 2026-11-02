using Static.Domain.Common;

namespace Static.Domain.Entities;

/// <summary>
/// A saved PlantUML diagram belonging to a user.
/// </summary>
public class UmlDiagram : BaseAuditableEntity
{
    /// <summary>Display name for the diagram.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional description / notes.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Raw PlantUML source text (e.g. "@startuml ... @enduml").</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Diagram type tag chosen by the user (sequence, class, usecase, …).</summary>
    public string DiagramType { get; set; } = "sequence";

    /// <summary>Internal user ID (maps to Keycloak sub claim) of the owning user.</summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// When true, this diagram is visible to all authenticated users (not just the owner).
    /// Only meaningful for teacher-owned diagrams — students cannot publish.
    /// </summary>
    public bool IsPublished { get; set; } = false;

    /// <summary>
    /// True when the creator held the Teacher role at the time of creation.
    /// Used together with <see cref="IsPublished"/> to determine student visibility.
    /// </summary>
    public bool IsTeacherContent { get; set; } = false;
}
