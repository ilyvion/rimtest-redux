namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Throw
{
    [Test]
    public static void PassWhenThrow() =>
        Assertion.AssertFunc(() => throw new ShouldHaveThrownException()).To.Throw();

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotThrow() => Assertion.AssertFunc(() => 1).To.Throw();
}
