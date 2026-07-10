#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RimTestRedux.Tests.Assertions;

[TestSuite]
#pragma warning disable CA1716 // Identifiers should not match keywords
public static class False
#pragma warning restore CA1716
{
    [Test]
    public static void PassWhenFalse() => Assertion.Assert(false).To.Be.False();

    [Test]
    public static void ThrowWhenNotFalse()
    {
        try
        {
            Assertion.Assert(true).To.Be.False();
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
