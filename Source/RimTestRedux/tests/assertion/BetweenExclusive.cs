namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class BetweenExclusive
{
    [Test]
    public static void PassWhenBetweenExclusive() =>
        Assertion.Assert(1).To.Be.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenEqualToMin() => Assertion.Assert(0).To.Be.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenEqualToMax() => Assertion.Assert(2).To.Be.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenEqualToBoth() => Assertion.Assert(1).To.Be.BetweenExclusive(1, 1);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenUnder() => Assertion.Assert(-1).To.Be.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenOver() => Assertion.Assert(3).To.Be.BetweenExclusive(0, 2);

    [Test]
    [ShouldThrow]
    public static void ThrowWhenInvalidLimits() => Assertion.Assert(1).To.Be.BetweenExclusive(2, 0);
}
