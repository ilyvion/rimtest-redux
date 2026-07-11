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
        Assertion.Assert(Validator.CheckTestIsStatic(GetMethodInfo("ValidTest"))).To.Be.True();

    [Test]
    public static void PassWhenParameterFree() =>
        Assertion
            .Assert(Validator.CheckTestIsParameterFree(GetMethodInfo("ValidTest")))
            .To.Be.True();

    [Test]
    public static void PassWhenReturnsVoid() =>
        Assertion.Assert(Validator.CheckTestReturnsVoid(GetMethodInfo("ValidTest"))).To.Be.True();

    [Test]
    public static void PassWhenValid() =>
        Assertion
            .AssertFunc(
                delegate
                {
                    Validator.IsValidTest(GetMethodInfo("ValidTest"));
                }
            )
            .Not.To.Throw();

    [Test]
    public static void IsValidTestThrowWhenNull() =>
        Assertion.AssertFunc(() => Validator.IsValidTest(null!)).To.Throw();

    [Test]
    public static void ChecksAreFalseWhenNull()
    {
        Assertion.Assert(Validator.CheckTestIsStatic(null!)).To.Be.False();
        Assertion.Assert(Validator.CheckTestReturnsVoid(null!)).To.Be.False();
        Assertion.Assert(Validator.CheckTestIsParameterFree(null!)).To.Be.False();
    }

    [Test]
    public static void PassWhenNonPublic() =>
        Assertion
            .AssertFunc(() => Validator.IsValidTest(GetMethodInfo("NonPublicTest")))
            .Not.To.Throw();

    [Test]
    public static void CheckIsFalseWhenNonStatic() =>
        Assertion.Assert(Validator.CheckTestIsStatic(GetMethodInfo("NonStaticTest"))).To.Be.False();

    [Test]
    public static void CheckIsFalseWhenNonVoidReturnType() =>
        Assertion
            .Assert(Validator.CheckTestReturnsVoid(GetMethodInfo("NonVoidReturnTest")))
            .To.Be.False();

    [Test]
    public static void CheckIsFalseWhenAcceptsParameters() =>
        Assertion
            .Assert(Validator.CheckTestIsParameterFree(GetMethodInfo("NonParameterFreeTest")))
            .To.Be.False();

    [Test]
    public static void ThrowWhenInvalid()
    {
        Assertion
            .AssertFunc(() => Validator.IsValidTest(GetMethodInfo("NonParameterFreeTest")))
            .To.Throw();

        Assertion
            .AssertFunc(() => Validator.IsValidTest(GetMethodInfo("NonVoidReturnTest")))
            .To.Throw();

        Assertion
            .AssertFunc(() => Validator.IsValidTest(GetMethodInfo("NonStaticTest")))
            .To.Throw();
    }

    // -----

    [Test]
    public static void DoubleCheckIsFalseWhenNonStatic() =>
        Assertion.Assert(Validator.CheckTestIsStatic(GetMethodInfo("NonStaticTest"))).To.Be.False();

    [Test]
    public static void DoubleCheckIsFalseWhenNonVoidReturnType() =>
        Assertion
            .Assert(Validator.CheckTestReturnsVoid(GetMethodInfo("NonVoidReturnTest")))
            .To.Be.False();

    [Test]
    public static void DoubleCheckIsFalseWhenAcceptsParameters() =>
        Assertion
            .Assert(Validator.CheckTestIsParameterFree(GetMethodInfo("NonParameterFreeTest")))
            .To.Be.False();

    [Test]
    public static void DoubleThrowWhenInvalid()
    {
        Assertion
            .AssertFunc(() => Validator.IsValidTest(GetMethodInfo("NonParameterFreeTest")))
            .To.Throw();

        Assertion
            .AssertFunc(() => Validator.IsValidTest(GetMethodInfo("NonVoidReturnTest")))
            .To.Throw();

        Assertion
            .AssertFunc(() => Validator.IsValidTest(GetMethodInfo("NonStaticTest")))
            .To.Throw();
    }
}
