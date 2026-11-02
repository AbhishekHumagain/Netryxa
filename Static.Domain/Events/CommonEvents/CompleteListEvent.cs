using Static.Domain.Common;

namespace Static.Domain.Events.CommonEvents
{

    public class CompleteListEvent<T>(List<T> obj) : BaseEvent
    {
        public readonly List<T> Obj = obj;
    }
}
