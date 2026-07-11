namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class SameAs
{
    [Test]
    public static void PassWhenSameValue()
    {
        IComparable mock = "a";
        IComparable mock2 = "a";
        Assertion.Assert(mock).To.Be.SameValueAs(mock2);
    }

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotSameValue()
    {
        IComparable mock = "a";
        IComparable mock2 = "b";
        Assertion.Assert(mock).To.Be.SameValueAs(mock2);
    }

    [Test]
    public static void PassWhenSameReference()
    {
        IComparable mock = "a";
        Assertion.Assert(mock).To.Be.SameReferenceAs(mock);
    }

    [Test]
    public static void PassWhenNotSameReference()
    {
        IComparable mock = new string(['a']);
        IComparable mock2 = new string(['a']);
        Assertion.Assert(mock).Not.To.Be.SameReferenceAs(mock2);
    }
}
