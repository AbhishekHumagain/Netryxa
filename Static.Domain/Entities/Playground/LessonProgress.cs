using Static.Domain.Common;

namespace Static.Domain.Entities.Playground;

/// <summary>
/// One user's progress through one lesson, stored server-side so it follows the user
/// to any device. Correctness and completion are derived when read, from the current
/// quiz questions, so teacher edits never leave stale results behind.
/// </summary>
public class LessonProgress : BaseAuditableEntity
{
    /// <summary>Keycloak subject (same id as <c>ICurrentUserService.UserId</c>).</summary>
    public Guid UserId { get; set; }
    public Guid LessonId { get; set; }

    /// <summary>JSON array of visited <c>LessonStep</c> names (jsonb), e.g. ["Concept","Visualize"].</summary>
    public string VisitedSteps { get; set; } = "[]";

    /// <summary>JSON object of quiz question id → selected option index (jsonb).</summary>
    public string QuizAnswers { get; set; } = "{}";

    /// <summary>JSON array of challenge ids whose tests all passed (jsonb).</summary>
    public string ChallengesPassed { get; set; } = "[]";

    public Lesson Lesson { get; set; } = null!;
}
