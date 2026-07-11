using System.Diagnostics;
using RimTestRedux.Util;

namespace RimTestRedux.Testing;

/// <summary>
/// Execute registered tests and update the status of registered tests, test suites and tested assemblies
/// </summary>
public static class Runner
{
    /// <summary>
    /// Check a test validity and run it if possible.
    /// Update the status to TestStatus.SKIP if invalid, TestStatus.PASS if succesfull, TestStatus.ERROR if failed.
    /// Registers any detected error to the test.
    /// If the test's suite has a [BeforeEach]/[AfterEach] hook registered, it's run immediately before/after the
    /// test, respectively; [AfterEach] is guaranteed to run (via try/finally) even if [BeforeEach] or the test itself throws.
    /// </summary>
    /// <param name="test"></param>
    /// <seealso cref="TestStatus"/>
    public static void RunTest(MethodInfo test)
    {
        try
        {
            Validator.IsValidTest(test);
        }
        catch (Exception e)
        {
            TestExplorer.SetTestStatus(test, TestStatus.SKIP);
            TestExplorer.SetTestError(test, e);
            return;
        }
        var expectedException = test.TryGetAttribute<ShouldThrowAttribute>();
        var beforeEach = TestSuite2HookLink.GetBeforeEach(test.DeclaringType!);
        var afterEach = TestSuite2HookLink.GetAfterEach(test.DeclaringType!);
        var stopwatch = new Stopwatch();
        try
        {
            try
            {
                // hooks and tests are static (null reference object) and do NOT accept arguments (null parameters array)
                _ = beforeEach?.Invoke(null, null);
                stopwatch.Start();
                _ = test.Invoke(null, null);
                stopwatch.Stop();
            }
            finally
            {
                _ = afterEach?.Invoke(null, null);
            }

            if (expectedException != null)
            {
                TestExplorer.SetTestStatus(test, TestStatus.ERROR);
                TestExplorer.SetTestError(
                    test,
                    new ShouldHaveThrownException(
                        expectedException.ExpectedType != null
                            ? "RimTestRedux.Runner.ExpectedExceptionType".Translate(
                                expectedException.ExpectedType.Name
                            )
                            : "RimTestRedux.Runner.ShouldHaveThrown".Translate()
                    )
                );
                return;
            }

            TimeElapsedExplorer.SetTestTimeElapsed(test, stopwatch.Elapsed.TotalMilliseconds);
            TestExplorer.SetTestStatus(test, TestStatus.PASS);
            TestExplorer.SetTestError(test, null);
        }
        catch (Exception e)
        {
            stopwatch.Stop();

            var actual = e is TargetInvocationException { InnerException: not null } tie
                ? tie.InnerException
                : e;

            if (
                expectedException != null
                && (
                    expectedException.ExpectedType == null
                    || expectedException.ExpectedType.IsInstanceOfType(actual)
                )
            )
            {
                TimeElapsedExplorer.SetTestTimeElapsed(test, stopwatch.Elapsed.TotalMilliseconds);
                TestExplorer.SetTestStatus(test, TestStatus.PASS);
                TestExplorer.SetTestError(test, null);
                return;
            }

            TestExplorer.SetTestStatus(test, TestStatus.ERROR);
            TestExplorer.SetTestError(test, actual);
        }
    }

    /// <summary>
    /// Check a test suite validity and run its tests if possible.
    /// Update the status to TestSuiteStatus.SKIP if invalid, then TestSuiteStatus.ERROR if any test fails, then TestSuiteStatus.WARNING if any test is skipped, then TestSuiteStatus.UNKNOWN if any test is not run, then TestSuiteStatus.PASS if succesfull.
    /// Registers any detected error to the test suite.
    /// </summary>
    /// <param name="testSuite"></param>
    /// <seealso cref="TestSuiteStatus"/>
    public static void RunTestSuite(Type testSuite)
    {
        try
        {
            Validator.IsValidTestSuite(testSuite);
        }
        catch (Exception e)
        {
            TestSuiteExplorer.SetTestSuiteStatus(testSuite, TestSuiteStatus.SKIP);
            TestSuiteExplorer.SetTestSuiteError(testSuite, e);
            return;
        }
        Tally<TestStatus> tally = [];
        foreach (var test in TestSuite2TestLink.GetTests(testSuite))
        {
            RunTest(test);
            tally[TestExplorer.GetTestStatus(test)]++;
        }

        if (tally[TestStatus.ERROR] != 0)
        {
            TestSuiteExplorer.SetTestSuiteStatus(testSuite, TestSuiteStatus.ERROR);
            TestSuiteExplorer.SetTestSuiteError(
                testSuite,
                new TestRunAggregateException(
                    "RimTestRedux.Runner.TestsFailed".Translate(tally[TestStatus.ERROR])
                )
            );
        }
        else if (tally[TestStatus.SKIP] != 0)
        {
            TestSuiteExplorer.SetTestSuiteStatus(testSuite, TestSuiteStatus.WARNING);
            TestSuiteExplorer.SetTestSuiteError(
                testSuite,
                new TestRunAggregateException(
                    "RimTestRedux.Runner.TestsSkipped".Translate(tally[TestStatus.SKIP])
                )
            );
        }
        else if (tally[TestStatus.UNKNOWN] != 0)
        {
            TestSuiteExplorer.SetTestSuiteStatus(testSuite, TestSuiteStatus.UNKNOWN);
            TestSuiteExplorer.SetTestSuiteError(
                testSuite,
                new TestRunAggregateException(
                    "RimTestRedux.Runner.TestsNotRun".Translate(tally[TestStatus.UNKNOWN])
                )
            );
        }
        else
        {
            TestSuiteExplorer.SetTestSuiteStatus(testSuite, TestSuiteStatus.PASS);
            TestSuiteExplorer.SetTestSuiteError(testSuite, null);
        }
    }

    /// <summary>
    /// Run a assembly test suites.
    /// Update the status to AssemblyStatus.ERROR if any test suite fails, then AssemblyStatus.WARNING if any test suite is skipped or have warnings, then AssemblyStatus.UNKNOWN if any test suite is not run, then AssemblyStatus.PASS if succesfull.
    /// Registers any detected error to the test suite.
    /// </summary>
    /// <param name="asm"></param>
    /// <seealso cref="AssemblyStatus"/>
    public static void RunAssembly(Assembly asm)
    {
        Tally<TestSuiteStatus> tally = [];
        foreach (var testSuite in Assembly2TestSuiteLink.GetTestSuites(asm))
        {
            RunTestSuite(testSuite);
            tally[TestSuiteExplorer.GetTestSuiteStatus(testSuite)]++;
        }

        if (tally[TestSuiteStatus.ERROR] != 0)
        {
            AssemblyExplorer.SetAssemblyStatus(asm, AssemblyStatus.ERROR);
            AssemblyExplorer.SetAssemblyError(
                asm,
                new TestRunAggregateException(
                    "RimTestRedux.Runner.TestSuitesFailed".Translate(tally[TestSuiteStatus.ERROR])
                )
            );
        }
        else if (tally[TestSuiteStatus.SKIP] != 0)
        {
            AssemblyExplorer.SetAssemblyStatus(asm, AssemblyStatus.WARNING);
            AssemblyExplorer.SetAssemblyError(
                asm,
                new TestRunAggregateException(
                    "RimTestRedux.Runner.TestSuitesSkipped".Translate(tally[TestSuiteStatus.SKIP])
                )
            );
        }
        else if (tally[TestSuiteStatus.WARNING] != 0)
        {
            AssemblyExplorer.SetAssemblyStatus(asm, AssemblyStatus.WARNING);
            AssemblyExplorer.SetAssemblyError(
                asm,
                new TestRunAggregateException(
                    "RimTestRedux.Runner.TestSuitesWarning".Translate(
                        tally[TestSuiteStatus.WARNING]
                    )
                )
            );
        }
        else if (tally[TestSuiteStatus.UNKNOWN] != 0)
        {
            AssemblyExplorer.SetAssemblyStatus(asm, AssemblyStatus.UNKNOWN);
            AssemblyExplorer.SetAssemblyError(
                asm,
                new TestRunAggregateException(
                    "RimTestRedux.Runner.TestSuitesNotRun".Translate(tally[TestSuiteStatus.UNKNOWN])
                )
            );
        }
        else
        {
            AssemblyExplorer.SetAssemblyStatus(asm, AssemblyStatus.PASS);
            AssemblyExplorer.SetAssemblyError(asm, null);
        }
    }

    /// <summary>
    /// </summary>
    public static void RunAllRegisteredTests()
    {
        foreach (var asm in Assembly2TestSuiteLink.Assemblies)
        {
            if (!RimTestReduxMod.Settings.RunOwnTests && asm == Assembly.GetExecutingAssembly())
            {
                continue;
            }

            RunAssembly(asm);
        }
    }
}
