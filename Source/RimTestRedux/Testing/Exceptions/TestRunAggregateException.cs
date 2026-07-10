#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.Serialization;

namespace RimTestRedux;

[Serializable]
public class TestRunAggregateException : Exception
{
    public TestRunAggregateException() { }

    public TestRunAggregateException(string message)
        : base(message) { }

    public TestRunAggregateException(string message, Exception innerException)
        : base(message, innerException) { }

    protected TestRunAggregateException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
