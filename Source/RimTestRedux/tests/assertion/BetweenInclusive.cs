#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RimTestRedux.Tests;

[TestSuite]
public static class BetweenInclusive
{
    [Test]
    public static void PassWhenBetweenInclusive() =>
        Assertion.Assert(1).To.Be.BetweenInclusive(0, 2);

    [Test]
    public static void PassWhenEqualsMin() => Assertion.Assert(0).To.Be.BetweenInclusive(0, 2);

    [Test]
    public static void PassWhenEqualsMax() => Assertion.Assert(2).To.Be.BetweenInclusive(0, 2);

    [Test]
    public static void PassWhenEqualsBoth() => Assertion.Assert(1).To.Be.BetweenInclusive(1, 1);

    [Test]
    public static void ThrowWhenUnder()
    {
        try
        {
            Assertion.Assert(-1).To.Be.BetweenInclusive(0, 2);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }

    [Test]
    public static void ThrowWhenOver()
    {
        try
        {
            Assertion.Assert(3).To.Be.BetweenInclusive(0, 2);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }

    [Test]
    public static void ThrowWhenInvalidLimits()
    {
        try
        {
            Assertion.Assert(1).To.Be.BetweenInclusive(2, 0);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
