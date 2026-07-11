namespace RimTestRedux.Tests;

[TestSuite]
internal static class BetweenInclusive
{
    [Test]
    public static void PassWhenBetweenInclusive() => Assert.That(1).Is.BetweenInclusive(0, 2);

    [Test]
    public static void PassWhenEqualsMin() => Assert.That(0).Is.BetweenInclusive(0, 2);

    [Test]
    public static void PassWhenEqualsMax() => Assert.That(2).Is.BetweenInclusive(0, 2);

    [Test]
    public static void PassWhenEqualsBoth() => Assert.That(1).Is.BetweenInclusive(1, 1);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenUnder() => Assert.That(-1).Is.BetweenInclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenOver() => Assert.That(3).Is.BetweenInclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenInvalidLimits() => Assert.That(1).Is.BetweenInclusive(2, 0);
}
