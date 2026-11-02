using Static.Helper.Constants;
using Static.Results;
using Microsoft.AspNetCore.Mvc;
using Static.API.Controllers.Helper;
using System.Net;

namespace Static.API.Controllers.Enduser.Utils;

/// <summary>
/// Subject Controller
/// </summary>
public class UtilsController : ApiControllerBase
{
    private readonly bool _isNotProduction =
        new[] { "development", "staging", "testing" }.Contains(
            Environment.GetEnvironmentVariable("DEPLOYMENT_ENV"));

    /// <summary>
    /// This will fetch the current biz environment  api/biz-environment
    /// </summary>
    [HttpGet("biz-environment")]
    public async Task<Result<string>> GetBizEnvironment()
    {
        if (_isNotProduction)
        {
            return string.IsNullOrEmpty(StaticConstants.BizEnv) ? "Global" : StaticConstants.BizEnv;
        }

        return new Error(HttpStatusCode.Forbidden, "Environment not available");
    }
}