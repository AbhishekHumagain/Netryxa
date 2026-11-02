using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.TeacherEvents
{
    public class TeacherCreatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class TeacherDeletedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class TeacherUpdatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }
}
