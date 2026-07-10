namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class GreaterThan
{
    [Test]
    public static void PassWhenGreater() => Assertion.Assert(1).To.Be.GreaterThan(0);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotGreater() => Assertion.Assert(1).To.Be.GreaterThan(1);
}
