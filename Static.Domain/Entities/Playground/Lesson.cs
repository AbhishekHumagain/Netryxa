using Static.Domain.Common;
using Static.Domain.Enums;

namespace Static.Domain.Entities.Playground;

public class Lesson : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LessonCategory Category { get; set; }
    public int OrderIndex { get; set; }
    /// <summary>Must be explicitly set to true by a teacher before students can see this lesson.</summary>
    public bool IsPublished { get; set; } = false;
    /// <summary>Beginner / Intermediate / Advanced.</summary>
    public string Difficulty { get; set; } = "Beginner";
    public int EstimatedMinutes { get; set; } = 10;

    public ICollection<CodeExample> CodeExamples { get; set; } = [];
    public ICollection<LessonContentBlock> ContentBlocks { get; set; } = [];
    public ICollection<CodingChallenge> Challenges { get; set; } = [];
    public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
}
