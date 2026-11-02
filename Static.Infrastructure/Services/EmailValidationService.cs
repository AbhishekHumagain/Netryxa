using Static.Application.Common.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Static.Infrastructure.Services;

public class EmailValidationService(
    HttpClient httpClient,
    IEmailValidationConfiguration emailValidationConfiguration)
    : IEmailValidationService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> ValidateEmailAddress(string emailAddress)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(emailValidationConfiguration.ValidationUrl + "?address=" +
                                             System.Web.HttpUtility.UrlEncode(emailAddress));
        var result = await response.Content.ReadAsStringAsync();
        var resultObject = JsonConvert.DeserializeObject<object>(result);
        var isValid = Convert.ToBoolean(JObject.Parse(resultObject.ToString())["valid"]);
        return isValid;
    }
}