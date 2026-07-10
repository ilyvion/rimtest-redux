namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class LessThan
{
    [Test]
    public static void PassWhenLess() => Assertion.Assert(1).To.Be.LessThan(2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotLess() => Assertion.Assert(1).To.Be.LessThan(1);
}
