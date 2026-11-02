using Newtonsoft.Json;

namespace Static.Application.Common.Dto.CloudFlare.RequestDto;

public record CloudFlareRequestDto
{
    public string Identifier { get; init; }
    public string SubDomain { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime ModificationTime { get; init; }
}

public record Result
{
    public string Id { get; init; }
    public string Name { get; init; }
    [JsonProperty("created_on")] public DateTime CreatedOn { get; init; }
    [JsonProperty("modified_on")] public DateTime ModifiedOn { get; init; }
}

public record Root
{
    public Result Result { get; init; }
}