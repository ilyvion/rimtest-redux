using System.Runtime.Serialization;

namespace RimTestRedux;

/// <summary>The exception that is thrown when an assertion expected an exception to be thrown, but none was.</summary>
[Serializable]
public class ShouldHaveThrownException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="ShouldHaveThrownException"/> class.</summary>
    public ShouldHaveThrownException() { }

    /// <summary>Initializes a new instance of the <see cref="ShouldHaveThrownException"/> class with a specified error message.</summary>
    public ShouldHaveThrownException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="ShouldHaveThrownException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    public ShouldHaveThrownException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>Initializes a new instance of the <see cref="ShouldHaveThrownException"/> class with serialized data.</summary>
    protected ShouldHaveThrownException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
