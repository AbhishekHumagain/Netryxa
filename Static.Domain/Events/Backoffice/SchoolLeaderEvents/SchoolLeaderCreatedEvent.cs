using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.SchoolLeaderEvents
{
    public class SchoolLeaderCreatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class SchoolLeaderDeletedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class SchoolLeaderUpdatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }
}
