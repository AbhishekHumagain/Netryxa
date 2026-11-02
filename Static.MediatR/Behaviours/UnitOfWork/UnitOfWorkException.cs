using System.Net;

namespace Static.MediatR.Behaviours.UnitOfWork;

public class UnitOfWorkException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public UnitOfWorkException(HttpStatusCode code, string message) : base(message)
    {
        StatusCode = code;
    }

    public UnitOfWorkException(HttpStatusCode code, string message, Exception exception) : base(message, exception)
    {
        StatusCode = code;
    }
}