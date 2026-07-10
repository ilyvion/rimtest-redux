using System.Runtime.Serialization;

namespace RimTestRedux;

/// <summary>The exception that is thrown when a test is found to be invalid.</summary>
[Serializable]
public class InvalidTestException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="InvalidTestException"/> class.</summary>
    public InvalidTestException() { }

    /// <summary>Initializes a new instance of the <see cref="InvalidTestException"/> class with a specified error message.</summary>
    public InvalidTestException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="InvalidTestException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    public InvalidTestException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>Initializes a new instance of the <see cref="InvalidTestException"/> class with serialized data.</summary>
    protected InvalidTestException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
