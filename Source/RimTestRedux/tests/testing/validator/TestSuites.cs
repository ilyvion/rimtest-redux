#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using RimTestRedux.Testing;

namespace RimTestRedux.Tests;

public static class MockValidTestSuite { }

public class MockNonStaticTestSuite { }

internal static class MockNonPublicTestSuite { }

[TestSuite]
public static class TestSuites
{
    [Test]
    public static void PassWhenValid() =>
        Assertion
            .AssertFunc(() => Validator.IsValidTestSuite(typeof(MockValidTestSuite)))
            .Not.To.Throw();

    [Test]
    public static void PassWhenPublic() =>
        Assertion.Assert(Validator.CheckTestSuiteIsPublic(typeof(MockValidTestSuite))).To.Be.True();

    [Test]
    public static void ThrowWhenNonPublic()
    {
        var type = typeof(MockNonPublicTestSuite);
        Assertion.Assert(Validator.CheckTestSuiteIsPublic(type)).To.Be.False();
        Assertion.AssertFunc(() => Validator.IsValidTestSuite(type)).To.Throw();
    }

    [Test]
    public static void IsValidTestSuiteThrowWhenNull() =>
        Assertion.AssertFunc(() => Validator.IsValidTestSuite(null!)).To.Throw();

    [Test]
    public static void ChecksAreFalseWhenNull()
    {
        Assertion.Assert(Validator.CheckTestSuiteIsPublic(null!)).To.Be.False();
        Assertion.Assert(Validator.CheckTestSuiteIsStatic(null!)).To.Be.False();
    }

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
    public static void ThrowWhenInvalid()
    {
        Assertion
            .AssertFunc(() => Validator.IsValidTestSuite(typeof(MockNonStaticTestSuite)))
            .To.Throw();

        Assertion
            .AssertFunc(() => Validator.IsValidTestSuite(typeof(MockNonPublicTestSuite)))
            .To.Throw();
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
