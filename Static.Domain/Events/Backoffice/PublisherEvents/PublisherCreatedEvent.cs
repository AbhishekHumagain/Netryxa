using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.PublisherEvents;

public class PublisherCreatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class PublisherUpdatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class PublisherDeletedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}