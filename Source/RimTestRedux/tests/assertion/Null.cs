namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Null
{
    [Test]
    public static void PassWhenNull() => Assert.That(null).Is.Null();

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotNull() => Assert.That(1).Is.Null();
}
