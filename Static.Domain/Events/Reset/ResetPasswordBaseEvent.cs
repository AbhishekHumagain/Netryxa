using Static.Domain.Entities;

namespace Static.Domain.Events.Reset;

public class ResetPasswordBaseEvent(User item) : Common.BaseEvent
{
    private User Item { get; } = item;
}
