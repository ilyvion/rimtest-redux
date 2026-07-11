namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Throw
{
    [Test]
    public static void PassWhenThrow() =>
        Assert.ThatFunc(() => throw new ShouldHaveThrownException()).Does.Throw();

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotThrow() => Assert.ThatFunc(() => 1).Does.Throw();
}
