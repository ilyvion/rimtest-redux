namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class SameAs
{
    [Test]
    public static void PassWhenSameValue()
    {
        IComparable mock = "a";
        IComparable mock2 = "a";
        Assert.That(mock).Is.The.SameValueAs(mock2);
    }

    [Test]
    [ShouldThrow]
    public static void ThrowWhenNotSameValue()
    {
        IComparable mock = "a";
        IComparable mock2 = "b";
        Assert.That(mock).Is.The.SameValueAs(mock2);
    }

    [Test]
    public static void PassWhenSameReference()
    {
        IComparable mock = "a";
        Assert.That(mock).Is.The.SameReferenceAs(mock);
    }

    [Test]
    public static void PassWhenNotSameReference()
    {
        IComparable mock = new string(['a']);
        IComparable mock2 = new string(['a']);
        Assert.That(mock).Is.Not.The.SameReferenceAs(mock2);
    }
}
