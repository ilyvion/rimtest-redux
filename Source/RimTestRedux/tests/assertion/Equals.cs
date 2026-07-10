namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Equals
{
    [Test]
    public static void PassWhenEqual() => Assertion.Assert(1).To.Be.EqualTo(1);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotEqual() => Assertion.Assert(1).To.Be.EqualTo(2);
}
