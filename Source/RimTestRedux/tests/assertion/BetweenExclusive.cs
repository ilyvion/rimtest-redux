#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RimTestRedux.Tests.Assertions;

[TestSuite]
public static class BetweenExclusive
{
    [Test]
    public static void PassWhenBetweenExclusive() =>
        Assertion.Assert(1).To.Be.BetweenExclusive(0, 2);

    [Test]
    public static void ThrowWhenEqualToMin()
    {
        try
        {
            Assertion.Assert(0).To.Be.BetweenExclusive(0, 2);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }

    [Test]
    public static void ThrowWhenEqualToMax()
    {
        try
        {
            Assertion.Assert(2).To.Be.BetweenExclusive(0, 2);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }

    [Test]
    public static void ThrowWhenEqualToBoth()
    {
        try
        {
            Assertion.Assert(1).To.Be.BetweenExclusive(1, 1);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }

    [Test]
    public static void ThrowWhenUnder()
    {
        try
        {
            Assertion.Assert(-1).To.Be.BetweenExclusive(0, 2);
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
            Assertion.Assert(3).To.Be.BetweenExclusive(0, 2);
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
            Assertion.Assert(1).To.Be.BetweenExclusive(2, 0);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
