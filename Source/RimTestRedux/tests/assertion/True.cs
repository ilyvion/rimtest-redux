#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RimTestRedux.Tests.Assertions;

[TestSuite]
#pragma warning disable CA1716 // Identifiers should not match keywords
public static class True
#pragma warning restore CA1716
{
    [Test]
    public static void PassWhenTrue() => Assertion.Assert(true).To.Be.True();

    [Test]
    public static void ThrowWhenNotTrue()
    {
        try
        {
            Assertion.Assert(false).To.Be.True();
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
