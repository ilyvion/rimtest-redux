namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class LessThan
{
    [Test]
    public static void PassWhenLess() => Assert.That(1).Is.LessThan(2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotLess() => Assert.That(1).Is.LessThan(1);
}
