namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class False
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
