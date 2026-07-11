namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Collection
{
    internal static readonly int[] three = [1, 2, 3];
    internal static readonly int[] one = [1];

    [Test]
    public static void PassWhenContains() => Assertion.AssertCollection(three).To.Contain(2);

    [Test]
    public static void PassWhenDoesNotContain() =>
        Assertion.AssertCollection(three).Not.To.Contain(4);

    [Test]
    public static void PassWhenEmpty() =>
        Assertion.AssertCollection(Array.Empty<int>()).To.Be.Empty();

    [Test]
    public static void PassWhenNotEmpty() => Assertion.AssertCollection(one).Not.To.Be.Empty();

    [Test]
    public static void PassWhenCountMatches() => Assertion.AssertCollection(three).To.Have.Count(3);

    [Test]
    public static void PassWhenCountDoesNotMatch() =>
        Assertion.AssertCollection(three).Not.To.Have.Count(2);
}
