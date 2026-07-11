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
        Assertion
            .AssertFunc(() =>
                Validator.IsValidHook(
                    typeof(MockHooksSuite).GetMethod(nameof(MockHooksSuite.SetUp))
                )
            )
            .Not.To.Throw();

    [Test]
    public static void IsValidHookThrowWhenNull() =>
        Assertion.AssertFunc(() => Validator.IsValidHook(null!)).To.Throw();

    [Test]
    public static void IsValidHookThrowWhenNonVoidReturn() =>
        Assertion
            .AssertFunc(() =>
                Validator.IsValidHook(
                    typeof(MockInvalidHookSuite).GetMethod(nameof(MockInvalidHookSuite.SetUp))
                )
            )
            .To.Throw();

    [Test]
    public static void ExploreAndRegisterHooksRegistersBeforeAndAfterEach()
    {
        var suite = typeof(MockHooksSuite);
        Explorer.ExploreAndRegisterHooks(suite);

        Assertion
            .Assert(TestSuite2HookLink.GetBeforeEach(suite)?.Name)
            .EqualTo(nameof(MockHooksSuite.SetUp));
        Assertion
            .Assert(TestSuite2HookLink.GetAfterEach(suite)?.Name)
            .EqualTo(nameof(MockHooksSuite.TearDown));
    }

    [Test]
    public static void ExploreAndRegisterHooksLeavesUnsetWhenNoneDeclared()
    {
        var suite = typeof(MockNoHooksSuite);
        Explorer.ExploreAndRegisterHooks(suite);

        Assertion.Assert(TestSuite2HookLink.GetBeforeEach(suite) is null).To.Be.True();
        Assertion.Assert(TestSuite2HookLink.GetAfterEach(suite) is null).To.Be.True();
    }

    [Test]
    public static void ExploreAndRegisterHooksThrowWhenMultipleBeforeEach() =>
        Assertion
            .AssertFunc(() => Explorer.ExploreAndRegisterHooks(typeof(MockTooManyBeforeEachSuite)))
            .To.Throw();

    [Test]
    public static void ExploreAndRegisterHooksThrowWhenMultipleAfterEach() =>
        Assertion
            .AssertFunc(() => Explorer.ExploreAndRegisterHooks(typeof(MockTooManyAfterEachSuite)))
            .To.Throw();

    [Test]
    public static void RunTestInvokesBeforeAndAfterEachAroundTheTest()
    {
        var suite = typeof(MockHooksSuite);
        Explorer.ExploreAndRegisterHooks(suite);
        MockHooksSuite.Log.Clear();

        Runner.RunTest(suite.GetMethod(nameof(MockHooksSuite.PassingTest)));

        Assertion.AssertCollection(MockHooksSuite.Log).To.Have.Count(3);
        Assertion.Assert(MockHooksSuite.Log[0]).EqualTo("before");
        Assertion.Assert(MockHooksSuite.Log[1]).EqualTo("test");
        Assertion.Assert(MockHooksSuite.Log[2]).EqualTo("after");
    }

    [Test]
    public static void RunTestInvokesAfterEachEvenWhenTestThrows()
    {
        var suite = typeof(MockHooksSuite);
        Explorer.ExploreAndRegisterHooks(suite);
        MockHooksSuite.Log.Clear();

        Runner.RunTest(suite.GetMethod(nameof(MockHooksSuite.ThrowingTest)));

        Assertion.AssertCollection(MockHooksSuite.Log).To.Have.Count(3);
        Assertion.Assert(MockHooksSuite.Log[2]).EqualTo("after");
    }
}
