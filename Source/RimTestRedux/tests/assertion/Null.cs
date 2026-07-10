namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Null
{
    [Test]
    public static void PassWhenNull() => Assertion.Assert(null).To.Be.Null();

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotNull() => Assertion.Assert(1).To.Be.Null();
}
