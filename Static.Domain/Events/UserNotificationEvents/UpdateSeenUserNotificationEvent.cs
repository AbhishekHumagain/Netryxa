using Static.Domain.Common;

namespace Static.Domain.Events.UserNotificationEvents;

public class UpdateSeenUserNotificationEvent(Guid userNotificationId) : BaseEvent
{
    private readonly Guid _userNotificationId = userNotificationId;
}