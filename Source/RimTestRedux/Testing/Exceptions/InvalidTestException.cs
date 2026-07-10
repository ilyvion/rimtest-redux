#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.Serialization;

namespace RimTestRedux;

[Serializable]
public class InvalidTestException : Exception
{
    public InvalidTestException() { }

    public InvalidTestException(string message)
        : base(message) { }

    public InvalidTestException(string message, Exception innerException)
        : base(message, innerException) { }

    protected InvalidTestException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
