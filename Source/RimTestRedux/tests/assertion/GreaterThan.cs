namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class GreaterThan
{
    [Test]
    public static void PassWhenGreater() => Assertion.Assert(1).To.Be.GreaterThan(0);

    [Test]
    public static void ThrowWhenNotGreater()
    {
        try
        {
            Assertion.Assert(1).To.Be.GreaterThan(1);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
