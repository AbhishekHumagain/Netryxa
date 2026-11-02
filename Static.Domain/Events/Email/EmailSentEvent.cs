using Static.Domain.Common;

namespace Static.Domain.Events.Email;

public class EmailSentEvent(string userEmail) : BaseEvent
{
    private readonly string _userEmail = userEmail;
}