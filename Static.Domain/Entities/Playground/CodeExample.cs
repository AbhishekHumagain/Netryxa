using Static.Domain.Common;

namespace Static.Domain.Entities.Playground;

public class CodeExample : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = "python";
    public string? ExpectedOutput { get; set; }
    public int OrderIndex { get; set; }
    public bool IsEditable { get; set; } = true;

    /// <summary>
    /// JSON array of guided walkthrough steps (jsonb):
    /// [{ "line": int, "title": string, "explanation": string, "predict": string?,
    ///    "answer": string?, "output": string?, "variables": [{ "name": string, "value": string }] }]
    /// </summary>
    public string WalkthroughSteps { get; set; } = "[]";

    public Lesson Lesson { get; set; } = null!;
}
