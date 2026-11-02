using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.DefaultSettingEvents
{
    public class DefaultSettingCreatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class DefaultSettingDeletedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }

    public class DefaultSettingUpdatedEvent(Guid id) : BaseEvent
    {
        private Guid _id = id;
    }
}
