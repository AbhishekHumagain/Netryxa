using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

namespace Static.Results;

public record Error(HttpStatusCode Code, string Message)
{
    public string? TraceId { get; init; } = Activity.Current?.TraceId.ToString();

    [JsonIgnore] public bool CommitTransaction { get; init; }

    [JsonExtensionData]
    public IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}