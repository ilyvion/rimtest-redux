using System.Runtime.Serialization;

namespace RimTestRedux;

/// <summary>The exception that aggregates the failures encountered during a test run.</summary>
[Serializable]
public class TestRunAggregateException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="TestRunAggregateException"/> class.</summary>
    public TestRunAggregateException() { }

    /// <summary>Initializes a new instance of the <see cref="TestRunAggregateException"/> class with a specified error message.</summary>
    public TestRunAggregateException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="TestRunAggregateException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    public TestRunAggregateException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>Initializes a new instance of the <see cref="TestRunAggregateException"/> class with serialized data.</summary>
    protected TestRunAggregateException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
