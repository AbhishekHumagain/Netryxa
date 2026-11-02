using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Static.Domain.Entities.Playground;
using Static.Domain.Enums;

namespace Static.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the learning-step content (content blocks, walkthrough steps, challenges, quiz)
/// for the built-in playground lessons from the embedded
/// <c>Data/playground-lesson-content.json</c>.
/// <para>
/// A lesson is only seeded once — when it has never had any content block, challenge or
/// quiz question (soft-deleted rows count). Teacher edits and deletions are never overwritten.
/// </para>
/// </summary>
internal static class PlaygroundLessonContentSeed
{
    private const string ResourceName = "Static.Infrastructure.PlaygroundLessonContent.json";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task ApplyAsync(StaticDbContext db)
    {
        var catalog = Load();
        if (catalog.Count == 0) return;

        var lessonsWithContent = await db.LessonContentBlock.IgnoreQueryFilters().Select(b => b.LessonId)
            .Union(db.CodingChallenge.IgnoreQueryFilters().Select(c => c.LessonId))
            .Union(db.QuizQuestion.IgnoreQueryFilters().Select(q => q.LessonId))
            .ToListAsync();

        var lessons = await db.Lesson
            .Include(l => l.CodeExamples)
            .Where(l => !lessonsWithContent.Contains(l.Id))
            .ToListAsync();

        foreach (var lesson in lessons)
        {
            var seed = catalog.FirstOrDefault(s => s.Category == lesson.Category && s.Title == lesson.Title);
            if (seed is null) continue;

            lesson.Difficulty       = seed.Difficulty;
            lesson.EstimatedMinutes = seed.EstimatedMinutes;

            var order = new Dictionary<LessonStep, int>();
            foreach (var block in seed.Blocks)
            {
                order[block.Step] = order.GetValueOrDefault(block.Step) + 1;
                db.LessonContentBlock.Add(new LessonContentBlock
                {
                    Id         = Guid.NewGuid(),
                    LessonId   = lesson.Id,
                    Step       = block.Step,
                    BlockType  = block.Type,
                    Title      = block.Title,
                    Data       = block.Data.GetRawText(),
                    OrderIndex = order[block.Step]
                });
            }

            foreach (var walkthrough in seed.Walkthroughs)
            {
                var example = lesson.CodeExamples.FirstOrDefault(e => !e.IsDeleted && e.Title == walkthrough.ExampleTitle);
                var steps   = example is null ? null : ResolveSteps(example.Code, walkthrough.Steps);
                if (steps is not null && example!.WalkthroughSteps is "[]" or "")
                    example.WalkthroughSteps = steps;
            }

            var challengeOrder = 0;
            foreach (var challenge in seed.Challenges)
            {
                db.CodingChallenge.Add(new CodingChallenge
                {
                    Id           = Guid.NewGuid(),
                    LessonId     = lesson.Id,
                    Title        = challenge.Title,
                    Instructions = challenge.Instructions,
                    Difficulty   = challenge.Difficulty,
                    StarterCode  = challenge.StarterCode,
                    SolutionCode = challenge.SolutionCode,
                    Hints        = JsonSerializer.Serialize(challenge.Hints, JsonOptions),
                    TestCases    = JsonSerializer.Serialize(challenge.TestCases, JsonOptions),
                    OrderIndex   = ++challengeOrder
                });
            }

            var quizOrder = 0;
            foreach (var question in seed.Quiz)
            {
                db.QuizQuestion.Add(new QuizQuestion
                {
                    Id                 = Guid.NewGuid(),
                    LessonId           = lesson.Id,
                    Question           = question.Question,
                    CodeSnippet        = question.CodeSnippet,
                    Options            = JsonSerializer.Serialize(question.Options, JsonOptions),
                    CorrectOptionIndex = question.CorrectOptionIndex,
                    Explanation        = question.Explanation,
                    OrderIndex         = ++quizOrder
                });
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Turns each step's <c>match</c> text into the 1-based line number where it first appears
    /// in <paramref name="code"/>. Returns null if any step can't be located (e.g. a teacher has
    /// since rewritten the example), so a walkthrough never points at the wrong line.
    /// </summary>
    private static string? ResolveSteps(string code, List<JsonObject> steps)
    {
        var lines    = code.Replace("\r\n", "\n").Split('\n');
        var resolved = new JsonArray();

        foreach (var step in steps)
        {
            var match = step["match"]?.GetValue<string>();
            var index = match is null ? -1 : Array.FindIndex(lines, l => l.Contains(match, StringComparison.Ordinal));
            if (index < 0) return null;

            var copy = (JsonObject)step.DeepClone();
            copy.Remove("match");
            copy["line"] = index + 1;
            resolved.Add(copy);
        }

        return resolved.ToJsonString(JsonOptions);
    }

    private static List<LessonSeed> Load()
    {
        using var stream = typeof(PlaygroundLessonContentSeed).Assembly.GetManifestResourceStream(ResourceName);
        if (stream is null) return [];
        return JsonSerializer.Deserialize<CatalogSeed>(stream, JsonOptions)?.Lessons ?? [];
    }

    // ── JSON shapes ───────────────────────────────────────────────────────────

    private sealed record CatalogSeed(List<LessonSeed> Lessons);

    private sealed record LessonSeed(
        string Title,
        LessonCategory Category,
        string Difficulty,
        int EstimatedMinutes,
        List<BlockSeed> Blocks,
        List<WalkthroughSeed> Walkthroughs,
        List<ChallengeSeed> Challenges,
        List<QuizSeed> Quiz);

    private sealed record BlockSeed(LessonStep Step, ContentBlockType Type, string? Title, JsonElement Data);

    private sealed record WalkthroughSeed(string ExampleTitle, List<JsonObject> Steps);

    private sealed record ChallengeSeed(
        string Title,
        string Difficulty,
        string Instructions,
        string StarterCode,
        string SolutionCode,
        List<string> Hints,
        List<TestCaseSeed> TestCases);

    private sealed record TestCaseSeed(string Name, string Code, string ExpectedOutput);

    private sealed record QuizSeed(
        string Question,
        string? CodeSnippet,
        List<string> Options,
        int CorrectOptionIndex,
        string Explanation);
}
