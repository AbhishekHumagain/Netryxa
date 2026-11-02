using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.TestPaperSettingEvents;

public class TestPaperSettingCreatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class TestPaperSettingUpdatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}