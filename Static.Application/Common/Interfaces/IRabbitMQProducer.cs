using Static.Application.Common.Models;

namespace Static.Application.Common.Interfaces;

public interface IRabbitMQProducer
{
    public Task SendEmailToQueue(EmailQueueRequestDto message);

    public Task SendNotificationQueue(NotificationInputDto message);

    public Task SendBulkNotificationQueue(List<NotificationInputDto> messages);

    public Task BroadCastNotificationQueue(BoardCastNotificationInputDto message);

}