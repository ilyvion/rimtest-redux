#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RimTestRedux.Tests.Assertions;

[TestSuite]
public static class Null
{
    [Test]
    public static void PassWhenNull() => Assertion.Assert(null).To.Be.Null();

    [Test]
    public static void ThrowWhenNotNull()
    {
        try
        {
            Assertion.Assert(1).To.Be.Null();
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
