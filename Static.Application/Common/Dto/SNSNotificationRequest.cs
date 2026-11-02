using Newtonsoft.Json;

namespace Static.Application.Common.Dto;

public class SnsNotificationRequest
{
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("userId")] public Guid? UserId { get; set; }
    [JsonProperty("payload")] public string Payload { get; set; }
}