using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.SubjectEvents;

public class SubjectLevelTokenCreatedEvent : BaseEvent
{
    private Guid _id;

    public SubjectLevelTokenCreatedEvent(Guid id)
    {
        _id = id;
    }
}