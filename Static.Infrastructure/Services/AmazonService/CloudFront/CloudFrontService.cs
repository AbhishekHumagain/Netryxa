using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.Runtime;
using Static.Application.Common.Interfaces;
using Static.Application.Common.Interfaces.Shared.CloudFront;
using Static.Domain.Errors;
using Static.Helper.Constants;
using Static.Results;
using Microsoft.Extensions.Configuration;

namespace Static.Infrastructure.Services.AmazonService.CloudFront;

public class CloudFrontService(IAwsConfiguration awsConfiguration, IConfiguration configuration) : ICloudFrontService
{
    public async Task<Result> CreateCloudFrontInvalidation(List<string> invalidationItems)
    {
        var distributionId = configuration["AWS:CloudFrontDistributionId"];

        var cloudFrontClient = new AmazonCloudFrontClient(
            awsConfiguration.AccessKey,
            awsConfiguration.SecretKey,
            StaticConstants.AwsRegionEndpoint);

        try
        {
            await cloudFrontClient.CreateInvalidationAsync(new CreateInvalidationRequest
            {
                DistributionId = distributionId,
                InvalidationBatch = new InvalidationBatch
                {
                    Paths = new Paths { Items = invalidationItems, Quantity = invalidationItems.Count },
                    CallerReference = Guid.NewGuid().ToString()
                }
            });
        }
        catch (AmazonClientException ex)
        {
            Console.WriteLine($"AWS Client Error: {ex.Message}");
            return HttpContextError.Ambiguous($"AWS Client Error:{ex.Message}");
        }

        return Result.Success();
    }
}