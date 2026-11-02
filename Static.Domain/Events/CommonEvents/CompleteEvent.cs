using Static.Domain.Common;

namespace Static.Domain.Events.CommonEvents;

public class CompleteEvent<T>(T obj) : BaseEvent
{
    public readonly T Obj = obj;
}