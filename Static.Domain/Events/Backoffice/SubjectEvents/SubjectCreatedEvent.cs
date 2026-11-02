using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.SubjectEvents;

public class SubjectCreatedEvent : BaseEvent
{
    private Guid _id;

    public SubjectCreatedEvent(Guid id)
    {
        _id = id;
    }
}