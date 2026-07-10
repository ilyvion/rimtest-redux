namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class False
{
    [Test]
    public static void PassWhenFalse() => Assertion.Assert(false).To.Be.False();

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotFalse() => Assertion.Assert(true).To.Be.False();
}
