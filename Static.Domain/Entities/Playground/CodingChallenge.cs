using Static.Domain.Common;

namespace Static.Domain.Entities.Playground;

/// <summary>
/// A "Try It" exercise: the student edits starter code and it is graded by running test cases.
/// </summary>
public class CodingChallenge : BaseAuditableEntity
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Easy";
    public string Language { get; set; } = "python";
    public string StarterCode { get; set; } = string.Empty;
    public string SolutionCode { get; set; } = string.Empty;
    /// <summary>JSON array of hint strings (jsonb).</summary>
    public string Hints { get; set; } = "[]";
    /// <summary>
    /// JSON array of test cases (jsonb): [{ "name": string, "code": string, "expectedOutput": string }].
    /// Each test runs the student's code followed by <c>code</c> and compares stdout.
    /// </summary>
    public string TestCases { get; set; } = "[]";
    public int OrderIndex { get; set; }

    public Lesson Lesson { get; set; } = null!;
}
