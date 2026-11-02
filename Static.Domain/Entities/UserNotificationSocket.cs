using Static.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Static.Domain.Entities;

public class UserNotificationSocket : BaseAuditableEntity
{
    public Guid Id { get; set; }
    [ForeignKey("User")] public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public string SocketId { get; set; }
}