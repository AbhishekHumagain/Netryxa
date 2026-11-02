using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.CountryEvents
{
    public class CountryCreatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class CountryDeletedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class CountryUpdatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

}
