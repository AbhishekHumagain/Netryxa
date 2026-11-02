using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Static.Application.Common.Dto;
using Static.Application.Common.Interfaces;
using Static.Helper.Constants;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace Static.Infrastructure.Services.AmazonService.SNS;

public class SNSService(IAwsConfiguration awsConfiguration, ILogger<SNSService> logger)
    : ISnsService, INotification
{
    public async Task<bool> IvysnsReporting(SnsNotificationRequest request)
    {
        try
        {
            var awsCredentials =
                new BasicAWSCredentials(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            var snsClient = new AmazonSimpleNotificationServiceClient(awsCredentials, StaticConstants.GetAwsRegionEndpoint("SNS"));
            var topicArn = awsConfiguration.WebSocketSnsTopicArn;
            var publishRequest = new PublishRequest
            {
                Message = JsonConvert.SerializeObject(request),
                TopicArn = topicArn
            };
        
            logger.LogWarning("Notification sent. \n" + JsonConvert.SerializeObject(publishRequest));
            var snsResponse = await snsClient.PublishAsync(publishRequest);
            if (snsResponse != null && snsResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
        
            logger.LogError(
               $"SNS PublishResponse: {JsonConvert.SerializeObject(snsResponse)}");
        }
        catch (Exception e)
        {
            logger.LogError(
               $"SNS SendNotification: {e.Message}", e);
        }

        return false;
    }
    
}