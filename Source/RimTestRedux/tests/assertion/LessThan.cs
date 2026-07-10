namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class LessThan
{
    [Test]
    public static void PassWhenLess() => Assertion.Assert(1).To.Be.LessThan(2);

    [Test]
    public static void ThrowWhenNotLess()
    {
        try
        {
            Assertion.Assert(1).To.Be.LessThan(1);
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
