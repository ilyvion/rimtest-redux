namespace RimTestRedux;

/// <summary>
/// Setup meta-attribute, used to mark a method to run before every test in a test suite.
/// </summary>
/// <remarks>
/// Only used on a method of a class with the meta-attribute TestSuite.
/// Valid BeforeEach methods are static, have a void signature and do not accept parameters.
/// A test suite may declare at most one BeforeEach method, see code example
/// </remarks>
/// <code>
/// [TestSuite]
/// public static class testSuiteA{
///     [BeforeEach]
///     public static void SetUp(){};
///
///     [Test]
///     public static void testA(){};
/// }
/// </code>
[AttributeUsage(AttributeTargets.Method)]
public sealed class BeforeEachAttribute : Attribute { }
