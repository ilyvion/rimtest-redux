#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RimTestRedux.Tests.Assertions;

[TestSuite]
public static class GreaterThan
{
    [Test]
    public static void PassWhenGreater() => Assertion.Assert(1).To.Be.GreaterThan(0);

    [Test]
    public static void ThrowWhenNotGreater()
    {
        try
        {
            Assertion.Assert(1).To.Be.GreaterThan(1);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
