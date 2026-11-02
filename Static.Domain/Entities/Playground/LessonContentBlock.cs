using Static.Domain.Common;
using Static.Domain.Enums;

namespace Static.Domain.Entities.Playground;

/// <summary>
/// A teaching block (definition, table, image, video, diagram, …) shown on a lesson step.
/// </summary>
public class LessonContentBlock : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public LessonStep Step { get; set; }
    public ContentBlockType BlockType { get; set; }
    public string? Title { get; set; }
    /// <summary>Type-specific payload stored as jsonb — see <see cref="ContentBlockType"/>.</summary>
    public string Data { get; set; } = "{}";
    public int OrderIndex { get; set; }

    public Lesson Lesson { get; set; } = null!;
}
