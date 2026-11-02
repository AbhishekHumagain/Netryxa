using Static.Domain.Common;

namespace Static.Domain.Entities.Playground;

/// <summary>A multiple-choice question shown on the Quiz step of a lesson.</summary>
public class QuizQuestion : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Question { get; set; } = string.Empty;
    /// <summary>Optional code snippet shown with the question (e.g. "what does this print?").</summary>
    public string? CodeSnippet { get; set; }
    /// <summary>JSON array of answer option strings (jsonb).</summary>
    public string Options { get; set; } = "[]";
    public int CorrectOptionIndex { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public Lesson Lesson { get; set; } = null!;
}
