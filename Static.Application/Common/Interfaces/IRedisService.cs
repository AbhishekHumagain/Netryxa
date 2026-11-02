using Grpc.Core;

namespace Static.Application.Common.Interfaces;

public interface IRedisService
{
    string GetPackageManagementKey();
    string GenerateJsonWebToken(string key);

    Metadata Metadata();
}