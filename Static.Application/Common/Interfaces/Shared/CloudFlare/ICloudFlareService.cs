using Static.Results;
using Static.Application.Common.Dto.CloudFlare.DeleteDto;
using Static.Application.Common.Dto.CloudFlare.RequestDto;
using Static.Application.Common.Dto.CloudFlare.ResponseDto;

namespace Static.Application.Common.Interfaces.Shared.CloudFlare;

public interface ICloudFlareService
{
    Task<Result<CloudFlareResponseDto>> CheckIfSubDomainExists(string subDomain);

    Task<Result<CloudFlareRequestDto>> CreateSubDomain(string subDomain);

    Task<Result<CloudFlareDeleteDto>> DeleteSubDomain(string identifier);
}