namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class False
{
    [Test]
    public static void PassWhenFalse() => Assert.That(false).Is.False();

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotFalse() => Assert.That(true).Is.False();
}
