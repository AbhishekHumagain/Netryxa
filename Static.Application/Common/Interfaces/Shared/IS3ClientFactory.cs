using Amazon.S3;

namespace Static.Application.Common.Interfaces.Shared;

public interface IS3ClientFactory
{
    IAmazonS3 GetClientForRegion(Amazon.RegionEndpoint region);
    IAmazonS3 GetDefaultClient();
}