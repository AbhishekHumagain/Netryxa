using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.StudentEvents
{
    public class StudentCreatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class StudentDeletedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class StudentUpdatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }
}
