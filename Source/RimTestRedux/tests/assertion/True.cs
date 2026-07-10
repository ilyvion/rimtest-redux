namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class True
{
    [Test]
    public static void PassWhenTrue() => Assertion.Assert(true).To.Be.True();

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotTrue() => Assertion.Assert(false).To.Be.True();
}
