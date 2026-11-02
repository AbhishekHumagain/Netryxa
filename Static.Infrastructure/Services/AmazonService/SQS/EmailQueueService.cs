using Static.Application.Common.Interfaces;
using Static.Application.Common.Models;
using Static.Helper.Constants;
using Newtonsoft.Json;

namespace Static.Infrastructure.Services.AmazonService.SQS
{
    public class EmailQueueService(IAwsConfiguration awsConfiguration, ISQSService sqsService) : IEmailQueueService
    {
        public async Task<bool> SendEmailToQueue(EmailQueueRequestDto emailQueueRequestDto)
        {
            return await sqsService.SendMessage(awsConfiguration.EmailSqsUrl, JsonConvert.SerializeObject(emailQueueRequestDto), StaticConstants.QueueType.EmailQueue);
        }
    }
}
