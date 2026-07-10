namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class SameAs
{
    [Test]
    public static void PassWhenSame()
    {
        IComparable mock = "a";
        Assertion.Assert(mock).To.Be.TheSame(mock);
    }

    [Test]
    public static void ThrowWhenNotSame()
    {
        IComparable mock = "a";
        IComparable mock2 = "b";
        try
        {
            Assertion.Assert(mock).To.Be.TheSame(mock2);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
