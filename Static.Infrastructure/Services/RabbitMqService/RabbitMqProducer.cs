using System.Text;
using Static.Application.Common.Interfaces;
using Static.Application.Common.Models;
using Static.Domain.Entities;
using Static.EntityFrameworkCore.Repository.Guid;
using Static.Helper.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Static.Infrastructure.Services.RabbitMqService;

public class RabbitMqProducer(
    IGenericRepository<UserNotificationSocket> userNotificationSocket,
    ILogger<RabbitMqProducer> logger)
    : IRabbitMQProducer
{
    public async Task SendEmailToQueue(EmailQueueRequestDto message)
    {  
        var factory = new ConnectionFactory()
        {
            HostName = RabbitMqConfiguration.HostName,
            Port =  RabbitMqConfiguration.Port,
            UserName = RabbitMqConfiguration.UserName,
            Password = RabbitMqConfiguration.Password,
        };
        var conn = await factory.CreateConnectionAsync();

        //When the control leaves the block of the using statement, an acquired IDisposable instance is disposed.
        //In particular,the using statement ensures that a disposable instance is disposed even
        //if an exception occurs within the block of the using statement.
        //In the preceding example, an opened file is closed after all lines are processed.
        // using var channel = connection.CreateChannelAsync(); 
        var channel = await conn.CreateChannelAsync();
        message.EmailFrom = RabbitMqConfiguration.EmailFrom;
        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);
        await channel.BasicPublishAsync(exchange: "", routingKey: StaticConstants.RabbitMq.EmailQueue, body: body);
        logger.LogInformation($"Static Email Queue: {message.EmailTemplateName} {message.ToAddress}");
    }

    public async Task SendNotificationQueue(NotificationInputDto message)
    {
        var socketIds = await userNotificationSocket.GetAll().Where(x => x.UserId == message.UserId)
            .Select(x => x.SocketId).ToListAsync();
        
        var payload = JsonConvert.DeserializeObject(message.Payload);
        if (string.IsNullOrEmpty(message.Payload))
        {
            payload = new Dictionary<string, bool>
            {
                { "fetchNotification", true },
            };
        }
        if (socketIds.Count != 0)
        {
            var factory = new ConnectionFactory()
            {
                HostName = RabbitMqConfiguration.HostName,
                Port =  RabbitMqConfiguration.Port,
                UserName = RabbitMqConfiguration.UserName,
                Password = RabbitMqConfiguration.Password,
            };
            var conn = await factory.CreateConnectionAsync();
            var channel = await conn.CreateChannelAsync();

            var socketIdList = socketIds.ToList();

            var notifications = new NotificationRequestDto
            {
                SocketIds = socketIdList,
                Payload =payload ,
                Namespace = RabbitMqConfiguration.SocketNamespace
            };
            var json = JsonConvert.SerializeObject(notifications);
            var body = Encoding.UTF8.GetBytes(json);
            await channel.BasicPublishAsync(exchange: "", routingKey: StaticConstants.RabbitMq.NotificationQueue,
                body: body);
        }
    }

    public async Task SendBulkNotificationQueue(List<NotificationInputDto> messages)
    {
        var socketIds = await userNotificationSocket.GetAll().Where(x => messages.Select(x => x.UserId).Contains(x.UserId))
            .Select(x =>new { x.SocketId, x.UserId}).ToListAsync();
        if (socketIds.Count != 0)
        { 
            var factory = new ConnectionFactory()
            {
                HostName = RabbitMqConfiguration.HostName,
                Port =  RabbitMqConfiguration.Port,
                UserName = RabbitMqConfiguration.UserName,
                Password = RabbitMqConfiguration.Password,
            };
            
            var conn = await factory.CreateConnectionAsync();
            var channel = await conn.CreateChannelAsync();
            foreach (var notfication in messages)
            {
                var payload = JsonConvert.DeserializeObject(notfication.Payload);
                if (string.IsNullOrEmpty(notfication.Payload))
                {
                    payload = new Dictionary<string, bool>
                    {
                        { "fetchNotification", true },
                    };
                }
                var notifications = new NotificationRequestDto
                {
                    SocketIds = socketIds.Where(x => x.UserId == notfication.UserId).Select(x => x.SocketId).ToList(),
                    Payload = payload ,
                    Namespace = RabbitMqConfiguration.SocketNamespace
                };
                var json = JsonConvert.SerializeObject(notifications);
                var body = Encoding.UTF8.GetBytes(json);
                await channel.BasicPublishAsync(exchange: "", routingKey: StaticConstants.RabbitMq.NotificationQueue,
                    body: body);
            }
        }
    }

    public async Task BroadCastNotificationQueue(BoardCastNotificationInputDto message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = RabbitMqConfiguration.HostName,
            Port =  RabbitMqConfiguration.Port,
            UserName = RabbitMqConfiguration.UserName,
            Password = RabbitMqConfiguration.Password,
        };
        var conn = await factory.CreateConnectionAsync();
        var channel = await conn.CreateChannelAsync();
        var notifications = new NotificationRequestDto
        {
            SocketIds = { },
            Payload = message.Payload,
            Namespace = RabbitMqConfiguration.SocketNamespace,
        };
        var json = JsonConvert.SerializeObject(notifications);
        var body = Encoding.UTF8.GetBytes(json);
        await channel.BasicPublishAsync(exchange: "", routingKey: StaticConstants.RabbitMq.NotificationQueue, body: body);
    }
}