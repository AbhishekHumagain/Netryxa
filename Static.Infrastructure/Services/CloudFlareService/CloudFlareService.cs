using Static.Application.Common.Dto.CloudFlare.DeleteDto;
using Static.Application.Common.Dto.CloudFlare.RequestDto;
using Static.Application.Common.Dto.CloudFlare.ResponseDto;
using Static.Application.Common.Interfaces.Shared.CloudFlare;
using Static.Domain.Errors;
using Static.Helper.Constants;
using Static.Results;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Static.Infrastructure.Services.CloudFlareService;

public class CloudFlareService(IConfiguration configuration, IHttpClientFactory httpClientFactory) : ICloudFlareService
{
    public async Task<Result<CloudFlareResponseDto>> CheckIfSubDomainExists(string subDomain)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(StaticConstants.HttpClients.CloudFlare);
            var type = configuration["CloudFlare:Type"];
            var zoneName = configuration["CloudFlare:ZoneName"];
            var requestUrl = $"?type={type}&name={subDomain}.{zoneName}";
            var response = await httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return HttpContextError.Conflict($"CloudFlare client error: {response.ReasonPhrase}");
            }

            var strResponse = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<Static.Application.Common.Dto.CloudFlare.ResponseDto.Root>(strResponse);
            if (res == null || res.ResultInfo.TotalCount == 0) return new CloudFlareResponseDto();
            var result = new CloudFlareResponseDto
            {
                Identifier = res.Result.FirstOrDefault().Id,
                SubDomain = res.Result.FirstOrDefault().Name,
                CreationTime = res.Result.FirstOrDefault().CreatedOn,
                ModificationTime = res.Result.FirstOrDefault().ModifiedOn,
                ResultCount = res.Result.Count
            };
            return result;
        }
        catch (Exception ex)
        {
            return HttpContextError.Conflict($"CloudFlare client error: {ex.Message}");
        }
    }

    public async Task<Result<CloudFlareRequestDto>> CreateSubDomain(string subDomain)
    {
        try
        {
            var json = new JObject
            {
                ["content"] = configuration["CloudFlare:DnsContent"],
                ["name"] = subDomain,
                ["proxied"] = false,
                ["type"] = configuration["CloudFlare:Type"],
                ["comment"] = "SpecialLaunchPage",
                ["ttl"] = 1
            }.ToString();
            var httpClient = httpClientFactory.CreateClient(StaticConstants.HttpClients.CloudFlare);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(string.Empty, content);
            if (!response.IsSuccessStatusCode)
            {
                return HttpContextError.Conflict($"CloudFlare client error: {response.ReasonPhrase}");
            }

            var strResponse = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<Static.Application.Common.Dto.CloudFlare.RequestDto.Root>(strResponse);
            var result = new CloudFlareRequestDto
            {
                Identifier = res.Result.Id,
                SubDomain = res.Result.Name,
                CreationTime = res.Result.CreatedOn,
                ModificationTime = res.Result.ModifiedOn
            };

            return result;
        }
        catch (Exception ex)
        {
            return HttpContextError.Conflict($"Error while processing the request: {ex.Message}");
        }
    }

    public async Task<Result<CloudFlareDeleteDto>> DeleteSubDomain(string identifier)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(StaticConstants.HttpClients.CloudFlare);
            var requestUrl = $"dns_records/{identifier}";
            var response = await httpClient.DeleteAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return HttpContextError.Ambiguous($"CloudFlare client error: {response.ReasonPhrase}");
            }

            var strResponse = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<Static.Application.Common.Dto.CloudFlare.DeleteDto.Root>(strResponse);
            var result = new CloudFlareDeleteDto { Identifier = res.Result.Id };

            return result;
        }
        catch (Exception ex)
        {
            return HttpContextError.Conflict($"CloudFlare client error: {ex.Message}");
        }
    }
}