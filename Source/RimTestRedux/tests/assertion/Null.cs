namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Null
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
