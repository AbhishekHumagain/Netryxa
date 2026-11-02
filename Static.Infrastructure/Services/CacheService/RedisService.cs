using Static.Application.Common.Dto;
using Static.Application.Common.Interfaces;
using Static.Helper.Constants;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Static.Infrastructure.Services.Cache;

public class RedisService(IConfiguration configuration) : IRedisService
{
    public Metadata Metadata()
    {
        //call redis here and create jwt using currentKey
        var packageManagementAuthKey = GetPackageManagementKey();
        var redisKey = JsonConvert.DeserializeObject<PackageManagementKeyDto>(packageManagementAuthKey);
        var jwtToken = GenerateJsonWebToken(redisKey.CurrentKey);
        var metadata = new Metadata { { "token", $"{jwtToken}" } };

        return metadata;
    }

    public string GetPackageManagementKey()
    {
        var url = configuration.GetConnectionString("Redis");
        var key = StaticConstants.RedisKeys.Package;
        var redis = ConnectionMultiplexer.Connect(url);
        var value = redis.GetDatabase().StringGet(key);
        return value;
    }
    
    public string GenerateJsonWebToken(string key)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // var token = new JwtSecurityToken(_jwtConfiguration.Issuer,
        //     _jwtConfiguration.Audience,
        //     null,
        //     expires: DateTime.Now.AddMinutes(120),
        //     signingCredentials: credentials);

        //TODO: get jwt configuration like above commented
        var token = new JwtSecurityToken();
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}