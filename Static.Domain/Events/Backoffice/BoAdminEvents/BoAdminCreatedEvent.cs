using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.BoAdminEvents;

public class BoAdminCreatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class BoAdminUpdatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class BoAdminDeletedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}