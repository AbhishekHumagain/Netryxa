using Newtonsoft.Json;

namespace Static.Application.Common.Dto.CloudFlare.ResponseDto;

public record CloudFlareResponseDto
{
    public string? Identifier { get; init; }
    public string SubDomain { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime ModificationTime { get; init; }
    public int ResultCount { get; init; }
}

public record Result
{
    public string Id { get; init; }
    public string Name { get; init; }
    [JsonProperty("created_on")] public DateTime CreatedOn { get; init; }
    [JsonProperty("modified_on")] public DateTime ModifiedOn { get; init; }
}

public record ResultInfo
{
    [JsonProperty("total_count")] public int TotalCount { get; init; }
}

public record Root
{
    public List<Result> Result { get; init; }
    public bool Success { get; init; }
    public List<object> Errors { get; init; }
    public List<object> Messages { get; init; }
    [JsonProperty("result_info")] public ResultInfo ResultInfo { get; init; }
}