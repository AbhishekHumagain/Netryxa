using System.Net;

namespace Static.Application.Common.Exceptions;

public class UnitOfWorkExceptions : Exception
{
    public UnitOfWorkExceptions(string message, HttpStatusCode code) : base(message)
    {
        
    }
}