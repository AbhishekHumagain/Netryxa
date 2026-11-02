using Static.Results;

namespace Static.Application.Common.Interfaces.Shared.CloudFront;

public interface ICloudFrontService
{
    Task<Result> CreateCloudFrontInvalidation(List<string> invalidationItems);
}