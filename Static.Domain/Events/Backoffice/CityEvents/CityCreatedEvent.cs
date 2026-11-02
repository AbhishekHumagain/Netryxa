using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.CityEvents
{
    public class CityCreatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class CityDeletedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class CityUpdatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }
}
