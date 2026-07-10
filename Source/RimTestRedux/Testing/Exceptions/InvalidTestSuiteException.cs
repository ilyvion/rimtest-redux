#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.Serialization;

namespace RimTestRedux;

[Serializable]
public class InvalidTestSuiteException : Exception
{
    public InvalidTestSuiteException() { }

    public InvalidTestSuiteException(string message)
        : base(message) { }

    public InvalidTestSuiteException(string message, Exception innerException)
        : base(message, innerException) { }

    protected InvalidTestSuiteException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
