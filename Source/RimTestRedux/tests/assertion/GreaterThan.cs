namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class GreaterThan
{
    [Test]
    public static void PassWhenGreater() => Assert.That(1).Is.GreaterThan(0);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotGreater() => Assert.That(1).Is.GreaterThan(1);
}
