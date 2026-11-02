using Newtonsoft.Json;

namespace Static.Application.Common.Dto;

public class PackageManagementKeyDto
{
    public string CurrentKey { get; set; }
    public string PreviousKey { get; set; }
}

public class RedisRoatingKeyDto
{
    [JsonProperty("previousKey")] public string PreviousKey { get; set; }
    [JsonProperty("currentKey")] public string CurrentKey { get; set; }
}