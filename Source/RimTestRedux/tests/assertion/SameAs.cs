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
    [ShouldThrow]
    public static void ThrowWhenNotSame()
    {
        IComparable mock = "a";
        IComparable mock2 = "b";
            Assertion.Assert(mock).To.Be.TheSame(mock2);
    }
}
