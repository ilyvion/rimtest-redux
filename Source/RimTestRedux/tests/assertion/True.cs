namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class True
{
    [Test]
    public static void PassWhenTrue() => Assert.That(true).Is.True();

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotTrue() => Assert.That(false).Is.True();
}
