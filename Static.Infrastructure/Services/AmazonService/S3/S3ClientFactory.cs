using Amazon.Runtime;
using Amazon.S3;
using Static.Application.Common.Interfaces;
using Static.Application.Common.Interfaces.Shared;

namespace Static.Infrastructure.Services.AmazonService.S3;

public class S3ClientFactory : IS3ClientFactory
{
    private readonly IAmazonS3 _defaultClient;
    private readonly IAwsConfiguration _awsConfiguration;

    public S3ClientFactory(IAmazonS3 defaultClient, IAwsConfiguration awsConfiguration)
    {
        _defaultClient = defaultClient;
        _awsConfiguration = awsConfiguration;
    }

    public IAmazonS3 GetClientForRegion(Amazon.RegionEndpoint region)
    {
        var credentials = new BasicAWSCredentials(_awsConfiguration.AccessKey, _awsConfiguration.SecretKey);
        return new AmazonS3Client(credentials, region);
    }

    public IAmazonS3 GetDefaultClient()
    {
        return _defaultClient;
    }
}