using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.ClassGroupEvents;

public class ClassGroupCreatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class ClassGroupUpdatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class ClassGroupDeletedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}