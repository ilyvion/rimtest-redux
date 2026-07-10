namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class True
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
