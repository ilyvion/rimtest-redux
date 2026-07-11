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
        Assert
            .ThatFunc(() => Validator.IsValidTestSuite(typeof(MockValidTestSuite)))
            .Does.Not.Throw();

    [Test]
    public static void PassWhenNonPublic() =>
        Assert
            .ThatFunc(() => Validator.IsValidTestSuite(typeof(MockNonPublicTestSuite)))
            .Does.Not.Throw();

    [Test]
    public static void IsValidTestSuiteThrowWhenNull() =>
        Assert.ThatFunc(() => Validator.IsValidTestSuite(null!)).Does.Throw();

    [Test]
    public static void ChecksAreFalseWhenNull() =>
        Assert.That(Validator.CheckTestSuiteIsStatic(null!)).Is.False();

    [Test]
    public static void PassWhenStatic() =>
        Assert.That(Validator.CheckTestSuiteIsStatic(typeof(MockValidTestSuite))).Is.True();

    [Test]
    public static void ThrowWhenNonStatic()
    {
        var type = typeof(MockNonStaticTestSuite);
        Assert.That(Validator.CheckTestSuiteIsStatic(type)).Is.False();
        Assert.ThatFunc(() => Validator.IsValidTestSuite(type)).Does.Throw();
    }

    [Test]
    public static void ThrowWhenInvalid() =>
        Assert
            .ThatFunc(() => Validator.IsValidTestSuite(typeof(MockNonStaticTestSuite)))
            .Does.Throw();
}
