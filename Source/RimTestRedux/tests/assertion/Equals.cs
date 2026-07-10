#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RimTestRedux.Tests.Assertions;

[TestSuite]
public static class Equals
{
    [Test]
    public static void PassWhenEqual() => Assertion.Assert(1).To.Be.EqualTo(1);

    [Test]
    public static void ThrowWhenNotEqual()
    {
        try
        {
            Assertion.Assert(1).To.Be.EqualTo(2);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
