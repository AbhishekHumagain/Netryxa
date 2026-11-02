using System.Collections.Specialized;

namespace Static.Application.Common.Interfaces;

public interface IRoleAction
{
    Task<List<string>> CreateUserAction(string body);
    Task<List<string?>>   UpdateUserAction(NameValueCollection queryParams);
    Task<List<string?>> DeleteUserAction(NameValueCollection queryParams);
    Task<List<string?>> GetByIdAction(NameValueCollection queryParams);
    Task<List<string?>> GetAllAction(NameValueCollection queryParams);

}