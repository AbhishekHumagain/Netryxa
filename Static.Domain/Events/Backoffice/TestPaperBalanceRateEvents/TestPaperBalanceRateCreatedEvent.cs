using Static.Domain.Common;

namespace Static.Domain.Events.Backoffice.TestPaperBalanceRateEvents;

public class TestPaperBalanceRateCreatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}

public class TestPaperBalanceRateUpdatedEvent(Guid id) : BaseEvent
{
    private Guid _id = id;
}