using RimTestRedux.Testing;

namespace RimTestRedux.Tests;

/// <summary>Mock test suite that is valid, used to validate <see cref="Validator.IsValidTestSuite"/>.</summary>
public static class MockValidTestSuite { }

/// <summary>Mock test suite that is not static, used to validate <see cref="Validator.CheckTestSuiteIsStatic"/>.</summary>
public class MockNonStaticTestSuite { }

internal static class MockNonPublicTestSuite { }

[TestSuite]
internal static class TestSuites
{
    [Test]
    public static void PassWhenValid() =>
        Assertion
            .AssertFunc(() => Validator.IsValidTestSuite(typeof(MockValidTestSuite)))
            .Not.To.Throw();

    [Test]
    public static void PassWhenNonPublic() =>
        Assertion
            .AssertFunc(() => Validator.IsValidTestSuite(typeof(MockNonPublicTestSuite)))
            .Not.To.Throw();

    [Test]
    public static void IsValidTestSuiteThrowWhenNull() =>
        Assertion.AssertFunc(() => Validator.IsValidTestSuite(null!)).To.Throw();

    [Test]
    public static void ChecksAreFalseWhenNull() =>
        Assertion.Assert(Validator.CheckTestSuiteIsStatic(null!)).To.Be.False();

    [Test]
    public static void PassWhenStatic() =>
        Assertion.Assert(Validator.CheckTestSuiteIsStatic(typeof(MockValidTestSuite))).To.Be.True();

    [Test]
    public static void ThrowWhenNonStatic()
    {
        var type = typeof(MockNonStaticTestSuite);
        Assertion.Assert(Validator.CheckTestSuiteIsStatic(type)).To.Be.False();
        Assertion.AssertFunc(() => Validator.IsValidTestSuite(type)).To.Throw();
    }

    [Test]
    public static void ThrowWhenInvalid() =>
        Assertion
            .AssertFunc(() => Validator.IsValidTestSuite(typeof(MockNonStaticTestSuite)))
            .To.Throw();
}
