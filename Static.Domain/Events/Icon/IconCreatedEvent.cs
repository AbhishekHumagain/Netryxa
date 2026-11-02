using Static.Domain.Common;

namespace Static.Domain.Events.Icon;

public class IconCreatedEvent(Guid iconId) : BaseEvent
{
    private readonly Guid _iconId = iconId;
}

public class IconDeletedEvent(Guid iconId) : BaseEvent
{
    private readonly Guid _iconId = iconId;
}