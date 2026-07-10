namespace RimTestRedux;

/// <summary>
/// Meta-attribute marking a Test method as expected to throw an exception.
/// </summary>
/// <remarks>
/// Only used on methods also decorated with the meta-attribute Test.
/// If the decorated method returns without throwing, the test fails with a ShouldHaveThrownException.
/// If an ExpectedType is specified, the thrown exception (or a subtype of it) must match it,
/// otherwise the test fails as if the wrong exception were thrown; if no ExpectedType is specified,
/// any thrown exception is accepted, see code example
/// </remarks>
/// <code>
/// [Test]
/// [ShouldThrow]
/// public static void testA(){ throw new Exception(); };
///
/// [Test]
/// [ShouldThrow(typeof(ArgumentNullException))]
/// public static void testB(){ throw new ArgumentNullException(); };
/// </code>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ShouldThrowAttribute : Attribute
{
    /// <summary>
    /// The expected exception type, or null to accept any exception type.
    /// </summary>
    public Type? ExpectedType { get; }

    /// <summary>Initializes a new instance of the <see cref="ShouldThrowAttribute"/> class accepting any exception type.</summary>
    public ShouldThrowAttribute() { }

    /// <summary>Initializes a new instance of the <see cref="ShouldThrowAttribute"/> class requiring a specific exception type.</summary>
    public ShouldThrowAttribute(Type expectedType)
    {
        ExpectedType = expectedType;
    }
}
