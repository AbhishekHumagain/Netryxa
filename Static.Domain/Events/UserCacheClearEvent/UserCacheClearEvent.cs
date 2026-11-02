namespace Static.Domain.Events.UserCacheClearEvent;

public class UserCacheClearEvent(Guid userId) : Common.BaseEvent
{
    public readonly Guid UserId = userId;
}