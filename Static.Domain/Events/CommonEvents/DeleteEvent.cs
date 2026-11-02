using Static.Domain.Common;

namespace Static.Domain.Events.CommonEvents
{
    
    public class DeleteEvent(string id) : BaseEvent
    {
        public string id = id;
    }
}
