namespace RimTestRedux.Tests.Assertions;

[TestSuite]
internal static class Collection
{
    internal static readonly int[] three = [1, 2, 3];
    internal static readonly int[] one = [1];

    [Test]
    public static void PassWhenContains() => Assert.ThatCollection(three).Does.Contain(2);

    [Test]
    public static void PassWhenDoesNotContain() => Assert.ThatCollection(three).Does.Not.Contain(4);

    [Test]
    public static void PassWhenEmpty() => Assert.ThatCollection(Array.Empty<int>()).Is.Empty();

    [Test]
    public static void PassWhenNotEmpty() => Assert.ThatCollection(one).Is.Not.Empty();

    [Test]
    public static void PassWhenCountMatches() => Assert.ThatCollection(three).Has.Count(3);

    [Test]
    public static void PassWhenCountDoesNotMatch() =>
        Assert.ThatCollection(three).Does.Not.Have.Count(2);
}
