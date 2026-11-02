using Static.Results;
using System.Net;

namespace Static.Domain.Errors
{
    public static class HttpContextError
    {
        public static Error NotFound(string message) => new(HttpStatusCode.NotFound, message);

        public static Error GenericError(HttpStatusCode code,  string message, bool commitTransaction = false) => new(code, message) { CommitTransaction = commitTransaction };

        public static Error BadRequest(string message) => new(HttpStatusCode.BadRequest, message);

        public static Error Ambiguous(string message) => new(HttpStatusCode.Ambiguous, message);

        public static Error Forbidden(string message) => new(HttpStatusCode.Forbidden, message);

        public static Error NotAcceptable(string message) => new(HttpStatusCode.NotAcceptable, message);

        public static Error InternalServerError(string message) => new(HttpStatusCode.InternalServerError, message);

        public static Error UnAuthorized(string message) => new(HttpStatusCode.Unauthorized, message);

        public static Error CommittableUnAuthorized(string message) => new(HttpStatusCode.Unauthorized, message)  { CommitTransaction = true };

        public static Error Conflict(string message) => new(HttpStatusCode.Conflict, message);
    }
}