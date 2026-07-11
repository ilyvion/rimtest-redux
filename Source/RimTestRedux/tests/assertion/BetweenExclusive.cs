namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class BetweenExclusive
{
    [Test]
    public static void PassWhenBetweenExclusive() => Assert.That(1).Is.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenEqualToMin() => Assert.That(0).Is.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenEqualToMax() => Assert.That(2).Is.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenEqualToBoth() => Assert.That(1).Is.BetweenExclusive(1, 1);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenUnder() => Assert.That(-1).Is.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenOver() => Assert.That(3).Is.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenInvalidLimits() => Assert.That(1).Is.BetweenExclusive(2, 0);
}
