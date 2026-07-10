using System.Runtime.Serialization;

namespace RimTestRedux;

/// <summary>The exception that is thrown when a test suite is found to be invalid.</summary>
[Serializable]
public class InvalidTestSuiteException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="InvalidTestSuiteException"/> class.</summary>
    public InvalidTestSuiteException() { }

    /// <summary>Initializes a new instance of the <see cref="InvalidTestSuiteException"/> class with a specified error message.</summary>
    public InvalidTestSuiteException(string message)
        : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="InvalidTestSuiteException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    public InvalidTestSuiteException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>Initializes a new instance of the <see cref="InvalidTestSuiteException"/> class with serialized data.</summary>
    protected InvalidTestSuiteException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
