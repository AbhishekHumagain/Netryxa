using Static.Domain.Common;
using Static.Helper.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Static.Domain.Entities;

public class UserNotification : BaseAuditableEntity
{
    [ForeignKey("User")] public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public StaticConstants.UserNotificationTypes Type { get; set; }
    public string? Message { get; set; }
    [Required] [StringLength(256)] public string Title { get; set; }
    public string? Payload { get; set; }
    public bool IsSeen { get; set; }
}