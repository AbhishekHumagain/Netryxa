using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.ClassGradeEvents;

public class ClassGradeCreatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class ClassGradeUpdatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class ClassGradeDeletedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}