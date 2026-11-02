using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.CsrEvents;

public class CsrCreatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class CsrUpdatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class CsrDeletedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}