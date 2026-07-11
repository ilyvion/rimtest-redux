namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Equals
{
    [Test]
    public static void PassWhenEqual() => Assert.That(1).Is.EqualTo(1);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotEqual() => Assert.That(1).Is.EqualTo(2);
}
