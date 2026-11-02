using Static.Domain.Events.Email;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Static.Application.Common.CommonEventHandler;

public class EmailSentEventHandler(ILogger<EmailSentEventHandler> logger) : INotificationHandler<EmailSentEvent>
{
    public Task Handle(EmailSentEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sent email for userId: {notification}", notification);
        return Task.CompletedTask;
    }
}