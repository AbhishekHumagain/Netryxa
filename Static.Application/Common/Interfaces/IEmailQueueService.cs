using Static.Application.Common.Models;

namespace Static.Application.Common.Interfaces
{
    public interface IEmailQueueService
    {
        Task<bool> SendEmailToQueue(EmailQueueRequestDto emailQueueRequestDto);
    }
}
