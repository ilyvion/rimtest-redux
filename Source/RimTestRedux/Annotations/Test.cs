namespace RimTestRedux;

/// <summary>
/// Test function meta-attribute, used to register a function as a test to be discovered by RimTestRedux.
/// </summary>
/// <remarks>
/// Only used when the test function is a method of a class with the meta-attribute TestSuite.
/// Valid Tests have a void signature, are static, public and do not accept parameters, see code example
/// </remarks>
/// <code>
/// [Test]
/// public static void testA(){};
/// </code>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TestAttribute : Attribute { }
