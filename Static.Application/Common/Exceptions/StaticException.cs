using System.Runtime.Serialization;

namespace Static.Application.Common.Exceptions
{
    [Serializable]
    public class StaticException : Exception
    {
        //
        // Summary:
        //     Creates a new StaticException object.
        public StaticException()
        {
        }

        //
        // Summary:
        //     Creates a new StaticException object.
        public StaticException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        //
        // Summary:
        //     Creates a new StaticException object.
        //
        // Parameters:
        //   message:
        //     Exception message
        public StaticException(string message)
            : base(message)
        {
        }

        //
        // Summary:
        //     Creates a new StaticException object.
        //
        // Parameters:
        //   message:
        //     Exception message
        //
        //   innerException:
        //     Inner exception
        public StaticException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
