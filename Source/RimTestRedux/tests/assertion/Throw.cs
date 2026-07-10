namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Throw
{
    [Test]
    public static void PassWhenThrow() =>
        Assertion.AssertFunc(() => throw new ShouldHaveThrownException()).To.Throw();

    [Test]
    public static void ThrowWhenNotThrow()
    {
        try
        {
            Assertion.AssertFunc(() => 1).To.Throw();
        }
        catch (Exception)
        {
            return;
        }
        throw new ShouldHaveThrownException("Should have thrown an exception.");
    }
}
