using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Static.Application.Common.Interfaces;
using Static.Helper.Constants;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Static.Infrastructure.Services.AmazonService.SQS
{
    public class SQSService(IAwsConfiguration awsConfiguration, ILogger<IRequest> logger)
        : ISQSService
    {
        private readonly ILogger _logger = logger;

        public async Task<bool> SendMessage(string queueUrl, string messageBody, string queueType)
        {
            var sqsCredentials =
            new BasicAWSCredentials(awsConfiguration.AccessKey, awsConfiguration.SecretKey);

            var sqsClient = new AmazonSQSClient(sqsCredentials, StaticConstants.AwsRegionEndpoint);

            var sqsMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            if (queueUrl.EndsWith(".fifo"))
            {
                var currentDateTime = DateTime.Now;
                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var timeSinceEpoch = currentDateTime.ToUniversalTime() - unixEpoch;
                var unixTimestamp = (long)timeSinceEpoch.TotalSeconds;

                // FIFO queue needs these
                sqsMessageRequest.MessageGroupId = queueType;
                sqsMessageRequest.MessageDeduplicationId = queueType + messageBody.GetHashCode() + unixTimestamp;
            }

            // Logger.Warn("Email sent." + sqsMessageRequest.ToJsonString());

            try
            {
                var sqsMessageResponse = await sqsClient.SendMessageAsync(sqsMessageRequest);
                if (sqsMessageResponse != null && sqsMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }

                _logger.LogInformation("SQS SendMessageResponse: {@SqsMessageResponse}",
                    JsonConvert.SerializeObject(sqsMessageResponse));
            }
            catch (Exception e)
            {
                _logger.LogInformation("SQS SendMessage Exception, {0}",
                    e);
            }

            return false;
        }
    }
}
