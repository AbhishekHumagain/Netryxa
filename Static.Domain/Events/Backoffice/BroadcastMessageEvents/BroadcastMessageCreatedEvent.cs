using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.BroadcastMessageEvents;

public class BroadcastMessageCreatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class BroadcastMessageUpdatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class BroadcastMessageDeletedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}