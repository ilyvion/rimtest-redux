using RimTestRedux.Testing;

namespace RimTestRedux.Tests;

internal sealed class MockTests
{
    internal static void NonPublicTest() { }

#pragma warning disable CA1822 // Mark members as static
    public void NonStaticTest() { }
#pragma warning restore CA1822 // Mark members as static

    public static bool NonVoidReturnTest() => true;

    public static void NonParameterFreeTest(bool value)
    {
        var _ = value;
    }

    public static void ValidTest() { }
}

[TestSuite]
#pragma warning disable CA1724
internal static class Testing
#pragma warning restore CA1724
{
    private static MethodInfo GetMethodInfo(string methodName) =>
        typeof(MockTests)
            .GetTypeInfo()
            .GetMethod(
                methodName,
                BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Static
                    | BindingFlags.Instance
            );

    [Test]
    public static void PassWhenStatic() =>
        Assert.That(Validator.CheckTestIsStatic(GetMethodInfo("ValidTest"))).Is.True();

    [Test]
    public static void PassWhenParameterFree() =>
        Assert.That(Validator.CheckTestIsParameterFree(GetMethodInfo("ValidTest"))).Is.True();

    [Test]
    public static void PassWhenReturnsVoid() =>
        Assert.That(Validator.CheckTestReturnsVoid(GetMethodInfo("ValidTest"))).Is.True();

    [Test]
    public static void PassWhenValid() =>
        Assert
            .ThatFunc(
                delegate
                {
                    Validator.IsValidTest(GetMethodInfo("ValidTest"));
                }
            )
            .Does.Not.Throw();

    [Test]
    public static void IsValidTestThrowWhenNull() =>
        Assert.ThatFunc(() => Validator.IsValidTest(null!)).Does.Throw();

    [Test]
    public static void ChecksAreFalseWhenNull()
    {
        Assert.That(Validator.CheckTestIsStatic(null!)).Is.False();
        Assert.That(Validator.CheckTestReturnsVoid(null!)).Is.False();
        Assert.That(Validator.CheckTestIsParameterFree(null!)).Is.False();
    }

    [Test]
    public static void PassWhenNonPublic() =>
        Assert
            .ThatFunc(() => Validator.IsValidTest(GetMethodInfo("NonPublicTest")))
            .Does.Not.Throw();

    [Test]
    public static void CheckIsFalseWhenNonStatic() =>
        Assert.That(Validator.CheckTestIsStatic(GetMethodInfo("NonStaticTest"))).Is.False();

    [Test]
    public static void CheckIsFalseWhenNonVoidReturnType() =>
        Assert.That(Validator.CheckTestReturnsVoid(GetMethodInfo("NonVoidReturnTest"))).Is.False();

    [Test]
    public static void CheckIsFalseWhenAcceptsParameters() =>
        Assert
            .That(Validator.CheckTestIsParameterFree(GetMethodInfo("NonParameterFreeTest")))
            .Is.False();

    [Test]
    public static void ThrowWhenInvalid()
    {
        Assert
            .ThatFunc(() => Validator.IsValidTest(GetMethodInfo("NonParameterFreeTest")))
            .Does.Throw();

        Assert
            .ThatFunc(() => Validator.IsValidTest(GetMethodInfo("NonVoidReturnTest")))
            .Does.Throw();

        Assert.ThatFunc(() => Validator.IsValidTest(GetMethodInfo("NonStaticTest"))).Does.Throw();
    }

    // -----

    [Test]
    public static void DoubleCheckIsFalseWhenNonStatic() =>
        Assert.That(Validator.CheckTestIsStatic(GetMethodInfo("NonStaticTest"))).Is.False();

    [Test]
    public static void DoubleCheckIsFalseWhenNonVoidReturnType() =>
        Assert.That(Validator.CheckTestReturnsVoid(GetMethodInfo("NonVoidReturnTest"))).Is.False();

    [Test]
    public static void DoubleCheckIsFalseWhenAcceptsParameters() =>
        Assert
            .That(Validator.CheckTestIsParameterFree(GetMethodInfo("NonParameterFreeTest")))
            .Is.False();

    [Test]
    public static void DoubleThrowWhenInvalid()
    {
        Assert
            .ThatFunc(() => Validator.IsValidTest(GetMethodInfo("NonParameterFreeTest")))
            .Does.Throw();

        Assert
            .ThatFunc(() => Validator.IsValidTest(GetMethodInfo("NonVoidReturnTest")))
            .Does.Throw();

        Assert.ThatFunc(() => Validator.IsValidTest(GetMethodInfo("NonStaticTest"))).Does.Throw();
    }
}
