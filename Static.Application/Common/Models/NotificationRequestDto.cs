using Newtonsoft.Json;

namespace Static.Application.Common.Models;

public class NotificationRequestDto
{
    [JsonProperty("socketIds")]
    public List<string> SocketIds { get; set; }
    
    [JsonProperty("namespace")]
    public string Namespace { get; set; }
    
    [JsonProperty("payload")]
    public object Payload { get; set; }
}


public class NotificationInputDto
{
    public Guid UserId { get; set; }
    public string Payload { get; set; }
}

public class BoardCastNotificationInputDto
{
    public string Payload { get; set; }
}

public class NotificationInputUserListDto
{
    public Guid? ChildUserId { get; set; } // Null if the ticket creator is a child itself.
    public Guid TicketCreatorUserId { get; set; } // Could be of either a child or parent,
    public IEnumerable<Guid> ContentExpertUserIds { get; set; }
    public string Payload { get; set; }
}