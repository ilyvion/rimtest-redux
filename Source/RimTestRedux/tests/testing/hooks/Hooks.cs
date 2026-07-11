using RimTestRedux.Testing;

namespace RimTestRedux.Tests;

internal static class MockHooksSuite
{
    internal static readonly List<string> Log = [];

    [BeforeEach]
    public static void SetUp() => Log.Add("before");

    [Test]
    public static void PassingTest() => Log.Add("test");

    [Test]
    [ShouldThrow]
    public static void ThrowingTest()
    {
        Log.Add("test");
        throw new InvalidOperationException();
    }

    [AfterEach]
    public static void TearDown() => Log.Add("after");
}

internal static class MockNoHooksSuite
{
    [Test]
    public static void PassingTest() { }
}

internal static class MockTooManyBeforeEachSuite
{
    [BeforeEach]
    public static void SetUpA() { }

    [BeforeEach]
    public static void SetUpB() { }
}

internal static class MockTooManyAfterEachSuite
{
    [AfterEach]
    public static void TearDownA() { }

    [AfterEach]
    public static void TearDownB() { }
}

internal static class MockInvalidHookSuite
{
    [BeforeEach]
    public static bool SetUp() => true;
}

[TestSuite]
internal static class Hooks
{
    [Test]
    public static void PassWhenValidHook() =>
        Assert
            .ThatFunc(() =>
                Validator.IsValidHook(
                    typeof(MockHooksSuite).GetMethod(nameof(MockHooksSuite.SetUp))
                )
            )
            .Does.Not.Throw();

    [Test]
    public static void IsValidHookThrowWhenNull() =>
        Assert.ThatFunc(() => Validator.IsValidHook(null!)).Does.Throw();

    [Test]
    public static void IsValidHookThrowWhenNonVoidReturn() =>
        Assert
            .ThatFunc(() =>
                Validator.IsValidHook(
                    typeof(MockInvalidHookSuite).GetMethod(nameof(MockInvalidHookSuite.SetUp))
                )
            )
            .Does.Throw();

    [Test]
    public static void ExploreAndRegisterHooksRegistersBeforeAndAfterEach()
    {
        var suite = typeof(MockHooksSuite);
        Explorer.ExploreAndRegisterHooks(suite);

        Assert
            .That(TestSuite2HookLink.GetBeforeEach(suite)?.Name)
            .Is.EqualTo(nameof(MockHooksSuite.SetUp));
        Assert
            .That(TestSuite2HookLink.GetAfterEach(suite)?.Name)
            .Is.EqualTo(nameof(MockHooksSuite.TearDown));
    }

    [Test]
    public static void ExploreAndRegisterHooksLeavesUnsetWhenNoneDeclared()
    {
        var suite = typeof(MockNoHooksSuite);
        Explorer.ExploreAndRegisterHooks(suite);

        Assert.That(TestSuite2HookLink.GetBeforeEach(suite) is null).Is.True();
        Assert.That(TestSuite2HookLink.GetAfterEach(suite) is null).Is.True();
    }

    [Test]
    public static void ExploreAndRegisterHooksThrowWhenMultipleBeforeEach() =>
        Assert
            .ThatFunc(() => Explorer.ExploreAndRegisterHooks(typeof(MockTooManyBeforeEachSuite)))
            .Does.Throw();

    [Test]
    public static void ExploreAndRegisterHooksThrowWhenMultipleAfterEach() =>
        Assert
            .ThatFunc(() => Explorer.ExploreAndRegisterHooks(typeof(MockTooManyAfterEachSuite)))
            .Does.Throw();

    [Test]
    public static void RunTestInvokesBeforeAndAfterEachAroundTheTest()
    {
        var suite = typeof(MockHooksSuite);
        Explorer.ExploreAndRegisterHooks(suite);
        MockHooksSuite.Log.Clear();

        Runner.RunTest(suite.GetMethod(nameof(MockHooksSuite.PassingTest)));

        Assert.ThatCollection(MockHooksSuite.Log).Has.Count(3);
        Assert.That(MockHooksSuite.Log[0]).Is.EqualTo("before");
        Assert.That(MockHooksSuite.Log[1]).Is.EqualTo("test");
        Assert.That(MockHooksSuite.Log[2]).Is.EqualTo("after");
    }

    [Test]
    public static void RunTestInvokesAfterEachEvenWhenTestThrows()
    {
        var suite = typeof(MockHooksSuite);
        Explorer.ExploreAndRegisterHooks(suite);
        MockHooksSuite.Log.Clear();

        Runner.RunTest(suite.GetMethod(nameof(MockHooksSuite.ThrowingTest)));

        Assert.ThatCollection(MockHooksSuite.Log).Has.Count(3);
        Assert.That(MockHooksSuite.Log[2]).Is.EqualTo("after");
    }
}
