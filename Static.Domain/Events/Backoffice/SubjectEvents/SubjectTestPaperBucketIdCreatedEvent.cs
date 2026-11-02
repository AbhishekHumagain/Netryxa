using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.SubjectEvents;

public class SubjectTestPaperBucketIdCreatedEvent : BaseEvent
{
    private Guid _id;

    public SubjectTestPaperBucketIdCreatedEvent(Guid id)
    {
        _id = id;
    }
}