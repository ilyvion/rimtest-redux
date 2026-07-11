namespace RimTestRedux;

/// <summary>
/// Teardown meta-attribute, used to mark a method to run after every test in a test suite.
/// </summary>
/// <remarks>
/// Only used on a method of a class with the meta-attribute TestSuite.
/// Valid AfterEach methods are static, have a void signature and do not accept parameters.
/// Runs even if the test it followed threw, so it's safe to rely on for cleaning up shared fixtures.
/// A test suite may declare at most one AfterEach method, see code example
/// </remarks>
/// <code>
/// [TestSuite]
/// public static class testSuiteA{
///     [AfterEach]
///     public static void TearDown(){};
///
///     [Test]
///     public static void testA(){};
/// }
/// </code>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AfterEachAttribute : Attribute { }
