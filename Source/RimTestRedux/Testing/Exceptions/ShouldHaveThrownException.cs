#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.Serialization;

namespace RimTestRedux;

[Serializable]
public class ShouldHaveThrownException : Exception
{
    public ShouldHaveThrownException() { }

    public ShouldHaveThrownException(string message)
        : base(message) { }

    public ShouldHaveThrownException(string message, Exception innerException)
        : base(message, innerException) { }

    protected ShouldHaveThrownException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
