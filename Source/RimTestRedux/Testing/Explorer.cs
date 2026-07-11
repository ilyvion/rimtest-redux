using System.Text.RegularExpressions;
using RimTestRedux.Util;

namespace RimTestRedux.Testing;

/// <summary>
/// Time profiling system
/// </summary>
internal static class TimeElapsedExplorer
{
    private static readonly Dictionary<Assembly, double> asm2TimeElapsed = [];
    private static readonly Dictionary<Type, double> testSuite2TimeElapsed = [];
    private static readonly Dictionary<MethodInfo, double> test2TimeElapsed = [];

    /// <summary>
    /// Recomputes the elapsed test time for every known assembly.
    /// </summary>
    public static void UpdateAllAssembliesTimeElapsed()
    {
        foreach (var asm in AssemblyExplorer.AllKnownAssemblies)
        {
            UpdateAssemblyTimeElapsed(asm);
        }
    }

    /// <summary>
    /// Records the elapsed test time for an assembly.
    /// </summary>
    /// <param name="asm">The assembly to record the time for.</param>
    /// <param name="time">The elapsed time, in seconds, or -1 if unavailable.</param>
    public static void SetAssemblyTimeElapsed(Assembly asm, double time) =>
        asm2TimeElapsed[asm] = time;

    /// <summary>
    /// Recomputes an assembly's elapsed test time as the sum of its test suites' elapsed times.
    /// </summary>
    /// <param name="asm">The assembly to recompute the elapsed time for.</param>
    public static void UpdateAssemblyTimeElapsed(Assembly asm)
    {
        double totaltime = 0;
        var anyValid = false;
        foreach (var ts in Assembly2TestSuiteLink.GetTestSuites(asm))
        {
            UpdateTestSuiteTimeElapsed(ts);
            var testtime = GetTestSuiteTimeElapsed(ts);
            if (testtime != -1)
            {
                totaltime += testtime;
                anyValid = true;
            }
        }
        SetAssemblyTimeElapsed(asm, anyValid ? totaltime : -1);
    }

    /// <summary>
    /// Gets an assembly's elapsed test time, computing it first if it hasn't been recorded yet.
    /// </summary>
    /// <param name="asm">The assembly to get the elapsed time for.</param>
    /// <returns>The elapsed time, in seconds, or -1 if unavailable.</returns>
    public static double GetAssemblyTimeElapsed(Assembly asm)
    {
        if (asm2TimeElapsed.TryGetValue(asm, out var value))
        {
            return value;
        }
        else
        {
            UpdateAssemblyTimeElapsed(asm);
        }

        return asm2TimeElapsed[asm];
    }

    /// <summary>
    /// Records the elapsed test time for a test suite.
    /// </summary>
    /// <param name="ts">The test suite to record the time for.</param>
    /// <param name="time">The elapsed time, in seconds, or -1 if unavailable.</param>
    public static void SetTestSuiteTimeElapsed(Type ts, double time) =>
        testSuite2TimeElapsed[ts] = time;

    /// <summary>
    /// Recomputes a test suite's elapsed test time as the sum of its tests' elapsed times.
    /// </summary>
    /// <param name="ts">The test suite to recompute the elapsed time for.</param>
    public static void UpdateTestSuiteTimeElapsed(Type ts)
    {
        double totaltime = 0;
        var anyValid = false;
        foreach (var test in TestSuite2TestLink.GetTests(ts))
        {
            var testtime = GetTestTimeElapsed(test);
            if (testtime != -1)
            {
                totaltime += testtime;
                anyValid = true;
            }
        }
        SetTestSuiteTimeElapsed(ts, anyValid ? totaltime : -1);
    }

    /// <summary>
    /// Gets a test suite's elapsed test time, computing it first if it hasn't been recorded yet.
    /// </summary>
    /// <param name="ts">The test suite to get the elapsed time for.</param>
    /// <returns>The elapsed time, in seconds, or -1 if unavailable.</returns>
    public static double GetTestSuiteTimeElapsed(Type ts)
    {
        if (testSuite2TimeElapsed.TryGetValue(ts, out var value))
        {
            return value;
        }
        else
        {
            UpdateTestSuiteTimeElapsed(ts);
        }

        return testSuite2TimeElapsed[ts];
    }

    /// <summary>
    /// Records the elapsed test time for a test.
    /// </summary>
    /// <param name="test">The test to record the time for.</param>
    /// <param name="time">The elapsed time, in seconds, or -1 if unavailable.</param>
    public static void SetTestTimeElapsed(MethodInfo test, double time) =>
        test2TimeElapsed[test] = time;

    /// <summary>
    /// Gets a test's elapsed time.
    /// </summary>
    /// <param name="test">The test to get the elapsed time for.</param>
    /// <returns>The elapsed time, in seconds, or -1 if it hasn't been recorded.</returns>
    public static double GetTestTimeElapsed(MethodInfo test) =>
        test2TimeElapsed.TryGetValue(test, out var value) ? value : -1;
}

/// <summary>
/// Keeps track of filtered tests for easy test finding
/// </summary>
[HotSwappable]
internal static class FilteredExplorer
{
    private static Regex filter = new(@"");

    /// <summary>
    /// Are failed state asm shown?
    /// </summary>
    internal static bool failEnabledAsm = true;

    /// <summary>
    /// Are warning state asm shown?
    /// </summary>
    internal static bool warningEnabledAsm = true;

    /// <summary>
    /// Are unknown state asm shown?
    /// </summary>
    internal static bool unknownEnabledAsm = true;

    /// <summary>
    /// Are passed state asm shown?
    /// </summary>
    internal static bool passEnabledAsm = true;

    /// <summary>
    /// Are failed state TS shown?
    /// </summary>
    internal static bool failEnabledTS = true;

    /// <summary>
    /// Are warning state TS shown?
    /// </summary>
    internal static bool warningEnabledTS = true;

    /// <summary>
    /// Are unknown state TS shown?
    /// </summary>
    internal static bool unknownEnabledTS = true;

    /// <summary>
    /// Are skipped state TS shown?
    /// </summary>
    internal static bool skipEnabledTS = true;

    /// <summary>
    /// Are passed state TS shown?
    /// </summary>
    internal static bool passEnabledTS = true;

    /// <summary>
    /// Replaces the free-text search filter used by the "matches filter" and "matches text filter" methods.
    /// </summary>
    /// <param name="filter">The new free-text search filter.</param>
    public static void UpdateFilter(Regex filter) => FilteredExplorer.filter = filter;

    /// <summary>
    /// Checks whether an assembly's current status is one of the currently enabled (shown) statuses.
    /// </summary>
    /// <param name="asm">The assembly to check.</param>
    /// <returns><c>true</c> if the assembly's status is enabled; otherwise, <c>false</c>.</returns>
    public static bool DoesAssemblyStatusMatchesFilter(Assembly asm) =>
        AssemblyExplorer.GetAssemblyStatus(asm) switch
        {
            AssemblyStatus.ERROR => failEnabledAsm,
            AssemblyStatus.WARNING => warningEnabledAsm,
            AssemblyStatus.UNKNOWN => unknownEnabledAsm,
            AssemblyStatus.PASS => passEnabledAsm,
            _ => false,
        };

    /// <summary>
    /// Checks whether a test suite's current status is one of the currently enabled (shown) statuses.
    /// </summary>
    /// <param name="ts">The test suite to check.</param>
    /// <returns><c>true</c> if the test suite's status is enabled; otherwise, <c>false</c>.</returns>
    public static bool DoesTestSuiteStatusMatchesFilter(Type ts) =>
        TestSuiteExplorer.GetTestSuiteStatus(ts) switch
        {
            TestSuiteStatus.SKIP => skipEnabledTS,
            TestSuiteStatus.ERROR => failEnabledTS,
            TestSuiteStatus.WARNING => warningEnabledTS,
            TestSuiteStatus.UNKNOWN => unknownEnabledTS,
            TestSuiteStatus.PASS => passEnabledTS,
            _ => false,
        };

    /// <summary>
    /// Checks whether an assembly matches both the free-text search filter and the enabled status toggles.
    /// </summary>
    /// <param name="asm">The assembly to check.</param>
    /// <returns><c>true</c> if the assembly should be shown; otherwise, <c>false</c>.</returns>
    public static bool DoesAssemblyMatchesFilter(Assembly asm) =>
        asm == null
            ? throw new ArgumentNullException(nameof(asm))
            : DoesAssemblyMatchesTextFilter(asm) && DoesAssemblyStatusMatchesFilter(asm);

    /// <summary>
    /// Checks whether a test suite matches both the free-text search filter and the enabled status toggles.
    /// </summary>
    /// <param name="testSuite">The test suite to check.</param>
    /// <returns><c>true</c> if the test suite should be shown; otherwise, <c>false</c>.</returns>
    public static bool DoesTestSuiteMatchesFilter(Type testSuite) =>
        testSuite == null
            ? throw new ArgumentNullException(nameof(testSuite))
            : DoesTestSuiteMatchesTextFilter(testSuite)
                && DoesTestSuiteStatusMatchesFilter(testSuite);

    /// <summary>
    /// Matches only against the free-text search filter, ignoring the show/hide status toggles.
    /// Used for status count badges, which should reflect real counts even when a status is hidden.
    /// </summary>
    /// <param name="asm">The assembly to check.</param>
    /// <returns><c>true</c> if the assembly's name or any of its test suites match the free-text search filter; otherwise, <c>false</c>.</returns>
    public static bool DoesAssemblyMatchesTextFilter(Assembly asm) =>
        asm == null
            ? throw new ArgumentNullException(nameof(asm))
            : filter.IsMatch(asm.GetName().Name)
                || Assembly2TestSuiteLink.GetTestSuites(asm).Any(DoesTestSuiteMatchesTextFilter);

    /// <summary>
    /// Matches only against the free-text search filter, ignoring the show/hide status toggles.
    /// Used for status count badges, which should reflect real counts even when a status is hidden.
    /// </summary>
    /// <param name="testSuite">The test suite to check.</param>
    /// <returns><c>true</c> if the test suite's name or any of its tests match the free-text search filter; otherwise, <c>false</c>.</returns>
    public static bool DoesTestSuiteMatchesTextFilter(Type testSuite) =>
        testSuite == null
            ? throw new ArgumentNullException(nameof(testSuite))
            : filter.IsMatch(testSuite.Name)
                || TestSuite2TestLink.GetTests(testSuite).Any(DoesTestMatchesTextFilter);

    /// <summary>
    /// Matches only against the free-text search filter, ignoring the show/hide status toggles.
    /// Used for status count badges, which should reflect real counts even when a status is hidden.
    /// </summary>
    /// <param name="test">The test to check.</param>
    /// <returns><c>true</c> if the test's name matches the free-text search filter; otherwise, <c>false</c>.</returns>
    public static bool DoesTestMatchesTextFilter(MethodInfo test) =>
        test == null ? throw new ArgumentNullException(nameof(test)) : filter.IsMatch(test.Name);

    /// <summary>
    /// Gets every known assembly that matches the current filter (free-text search and enabled statuses).
    /// </summary>
    /// <returns>The filtered assemblies.</returns>
    public static IEnumerable<Assembly> GetFilteredAssemblies() =>
        [.. AssemblyExplorer.AllKnownAssemblies.Where(DoesAssemblyMatchesFilter)];

    /// <summary>
    /// Gets every known test suite that matches the current filter (free-text search and enabled statuses).
    /// </summary>
    /// <returns>The filtered test suites.</returns>
    public static IEnumerable<Type> GetFilteredTestSuites() =>
        [.. TestSuiteExplorer.AllKnownTestSuites.Where(DoesTestSuiteMatchesFilter)];

    /// <summary>
    /// Gets the test suites of an assembly that match the current filter (free-text search and enabled statuses).
    /// </summary>
    /// <param name="asm">The assembly whose test suites to filter.</param>
    /// <returns>The filtered test suites.</returns>
    public static IEnumerable<Type> GetFilteredTestSuites(Assembly asm) =>
        [.. Assembly2TestSuiteLink.GetTestSuites(asm).Where(DoesTestSuiteMatchesFilter)];

    /// <summary>
    /// Gets every known test whose name matches the free-text search filter.
    /// </summary>
    /// <returns>The filtered tests.</returns>
    public static IEnumerable<MethodInfo> GetFilteredTests() =>
        [.. TestExplorer.AllKnownTests.Where(DoesTestMatchesTextFilter)];

    /// <summary>
    /// Gets the tests of a test suite whose names match the free-text search filter.
    /// </summary>
    /// <param name="ts">The test suite whose tests to filter.</param>
    /// <returns>The filtered tests.</returns>
    public static IEnumerable<MethodInfo> GetFilteredTests(Type ts) =>
        [.. TestSuite2TestLink.GetTests(ts).Where(DoesTestMatchesTextFilter)];
}

/// <summary>
/// Keeps track of test statuses
/// </summary>
internal static class StatusExplorer
{
    private static readonly Dictionary<AssemblyStatus, int> asmStatus2count = [];
    private static readonly Dictionary<TestSuiteStatus, int> tsStatus2count = [];
    private static readonly Dictionary<TestStatus, int> tStatus2count = [];

    /// <summary>
    /// Recomputes the cached counts for every assembly, test suite, and test status.
    /// </summary>
    public static void UpdateAllStatusCounts()
    {
        foreach (AssemblyStatus status in Enum.GetValues(typeof(AssemblyStatus)))
        {
            UpdateAssemblyStatusCount(status);
        }
        foreach (TestSuiteStatus status in Enum.GetValues(typeof(TestSuiteStatus)))
        {
            UpdateTestSuiteStatusCount(status);
        }
        foreach (TestStatus status in Enum.GetValues(typeof(TestStatus)))
        {
            UpdateTestStatusCount(status);
        }
    }

    /// <summary>
    /// Recomputes the cached count of (filtered) assemblies currently at the given status.
    /// </summary>
    /// <param name="status">The status to recompute the count for.</param>
    public static void UpdateAssemblyStatusCount(AssemblyStatus status)
    {
        var value = AssemblyExplorer
            .AllKnownAssemblies.Where(FilteredExplorer.DoesAssemblyMatchesTextFilter)
            .Count(asm => AssemblyExplorer.GetAssemblyStatus(asm) == status);
        if (!asmStatus2count.TryAdd(status, value))
        {
            asmStatus2count[status] = value;
        }
    }

    /// <summary>
    /// Gets the cached count of (filtered) assemblies currently at the given status, computing it first if it hasn't been recorded yet.
    /// </summary>
    /// <param name="status">The status to get the count for.</param>
    /// <returns>The number of matching assemblies.</returns>
    public static int GetAssemblyStatusCount(AssemblyStatus status)
    {
        if (!asmStatus2count.ContainsKey(status))
        {
            UpdateAssemblyStatusCount(status);
        }

        return asmStatus2count[status];
    }

    /// <summary>
    /// Recomputes the cached count of (filtered) test suites currently at the given status.
    /// </summary>
    /// <param name="status">The status to recompute the count for.</param>
    public static void UpdateTestSuiteStatusCount(TestSuiteStatus status)
    {
        var value = TestSuiteExplorer
            .AllKnownTestSuites.Where(FilteredExplorer.DoesTestSuiteMatchesTextFilter)
            .Count(ts => TestSuiteExplorer.GetTestSuiteStatus(ts) == status);
        if (!tsStatus2count.TryAdd(status, value))
        {
            tsStatus2count[status] = value;
        }
    }

    /// <summary>
    /// Gets the cached count of (filtered) test suites currently at the given status, computing it first if it hasn't been recorded yet.
    /// </summary>
    /// <param name="status">The status to get the count for.</param>
    /// <returns>The number of matching test suites.</returns>
    public static int GetTestSuiteStatusCount(TestSuiteStatus status)
    {
        if (!tsStatus2count.ContainsKey(status))
        {
            UpdateTestSuiteStatusCount(status);
        }

        return tsStatus2count[status];
    }

    /// <summary>
    /// Recomputes the cached count of (filtered) tests currently at the given status.
    /// </summary>
    /// <param name="status">The status to recompute the count for.</param>
    public static void UpdateTestStatusCount(TestStatus status)
    {
        var value = TestExplorer
            .AllKnownTests.Where(FilteredExplorer.DoesTestMatchesTextFilter)
            .Count(t => TestExplorer.GetTestStatus(t) == status);
        if (!tStatus2count.TryAdd(status, value))
        {
            tStatus2count[status] = value;
        }
    }

    /// <summary>
    /// Gets the cached count of (filtered) tests currently at the given status, computing it first if it hasn't been recorded yet.
    /// </summary>
    /// <param name="status">The status to get the count for.</param>
    /// <returns>The number of matching tests.</returns>
    public static int GetTestStatusCount(TestStatus status)
    {
        if (!tStatus2count.ContainsKey(status))
        {
            UpdateTestStatusCount(status);
        }

        return tStatus2count[status];
    }
}

/// <summary>
/// Stores and manages Assembly level data, aka known statuses and exceptions.
/// </summary>
internal static class AssemblyExplorer
{
    private static readonly Dictionary<Assembly, AssemblyStatus> asm2Status = [];
    private static readonly Dictionary<Assembly, Exception?> asm2Error = [];

    /// <summary>
    /// Gets every assembly that has a registered status.
    /// </summary>
    /// <returns>The known assemblies.</returns>
    public static ICollection<Assembly> AllKnownAssemblies => asm2Status.Keys;

    /// <summary>
    /// Records the status for an assembly.
    /// </summary>
    /// <param name="asm">The assembly to record the status for.</param>
    /// <param name="status">The status to record.</param>
    public static void SetAssemblyStatus(Assembly asm, AssemblyStatus status) =>
        asm2Status[asm] = status;

    /// <summary>
    /// Records the exception that caused an assembly's tests to fail, if any.
    /// </summary>
    /// <param name="asm">The assembly to record the exception for.</param>
    /// <param name="error">The exception to record, or <c>null</c> to clear it.</param>
    public static void SetAssemblyError(Assembly asm, Exception? error) => asm2Error[asm] = error;

    /// <summary>
    /// Gets an assembly's registered status, registering it as <see cref="AssemblyStatus.UNKNOWN"/> first if it hasn't been seen yet.
    /// </summary>
    /// <param name="asm">The assembly to get the status for.</param>
    /// <returns>The current registered AssemblyStatus else AssemblyStatus.UNKNOWN</returns>
    /// <seealso cref="AssemblyStatus"/>
    public static AssemblyStatus GetAssemblyStatus(Assembly asm)
    {
        if (!asm2Status.TryGetValue(asm, out var value))
        {
            value = AssemblyStatus.UNKNOWN;
            asm2Status.Add(asm, value);
        }

        return value;
    }

    /// <summary>
    /// Gets the exception that caused an assembly's tests to fail, if any.
    /// </summary>
    /// <param name="asm">The assembly to get the exception for.</param>
    /// <returns>The current registered Exception else null if none registered</returns>
    public static Exception? GetAssemblyError(Assembly asm) =>
        asm2Error.TryGetValue(asm, out var value) ? value : null;

    /// <summary>
    /// Tallies how many of an assembly's (filtered) tests are at each <see cref="TestStatus"/>.
    /// </summary>
    /// <param name="asm">The assembly to tally the test statuses for.</param>
    /// <returns>A tally of how many of this assembly's (filtered) tests are at each <see cref="TestStatus"/>, summed across all of its test suites.</returns>
    public static Tally<TestStatus> TallyTestStatuses(Assembly asm)
    {
        var tally = new Tally<TestStatus>();
        foreach (var ts in FilteredExplorer.GetFilteredTestSuites(asm))
        {
            foreach (var (status, count) in TestSuiteExplorer.TallyTestStatuses(ts))
            {
                tally[status] += count;
            }
        }
        return tally;
    }
}

/// <summary>
/// Stores and manage the links between registered assemblies and their registered test suites
/// </summary>
internal static class Assembly2TestSuiteLink
{
    private static readonly IDictionary<Assembly, ISet<Type>> asm2TestSuites =
        new Dictionary<Assembly, ISet<Type>>();

    /// <summary>
    /// Can register multiple unique test suites to the same assembly. Will not store duplicates.
    /// </summary>
    /// <param name="testSuite">The test suite to register.</param>
    /// <param name="asm">The assembly to register the test suite to.</param>
    public static void RegisterTestSuite2Asm(Type testSuite, Assembly asm)
    {
        if (!asm2TestSuites.TryGetValue(asm, out var value))
        {
            value = new HashSet<Type>();
            asm2TestSuites[asm] = value;
        }

        _ = value.Add(testSuite);
    }

    /// <summary>
    /// Gets every assembly that has at least one registered test suite.
    /// </summary>
    /// <returns>A list of all known assemblies</returns>
    public static List<Assembly> Assemblies => [.. asm2TestSuites.Keys];

    /// <summary>
    /// Gets the test suites registered to an assembly.
    /// </summary>
    /// <param name="asm">The assembly to get the test suites for.</param>
    /// <returns>A set of registered tests suites for this assembly. Returns an empty set if the assembly is not registered.</returns>
    public static ISet<Type> GetTestSuites(Assembly asm) =>
        asm2TestSuites.TryGetValue(asm, out var value) ? value : new HashSet<Type>();
}

/// <summary>
/// Stores and manages Test Suite level data, aka known statuses and exceptions.
/// </summary>
internal static class TestSuiteExplorer
{
    private static readonly Dictionary<Type, TestSuiteStatus> testSuite2Status = [];
    private static readonly Dictionary<Type, Exception?> testSuite2Error = [];

    /// <summary>
    /// Gets every test suite that has a registered status.
    /// </summary>
    /// <returns>The known test suites.</returns>
    public static ICollection<Type> AllKnownTestSuites => testSuite2Status.Keys;

    /// <summary>
    /// Records the status for a test suite.
    /// </summary>
    /// <param name="testSuite">The test suite to record the status for.</param>
    /// <param name="status">The status to record.</param>
    public static void SetTestSuiteStatus(Type testSuite, TestSuiteStatus status) =>
        testSuite2Status[testSuite] = status;

    /// <summary>
    /// Records the exception that caused a test suite to be skipped or fail, if any.
    /// </summary>
    /// <param name="testSuite">The test suite to record the exception for.</param>
    /// <param name="error">The exception to record, or <c>null</c> to clear it.</param>
    public static void SetTestSuiteError(Type testSuite, Exception? error) =>
        testSuite2Error[testSuite] = error;

    /// <summary>
    /// Gets a test suite's registered status, registering it as <see cref="TestSuiteStatus.UNKNOWN"/> first if it hasn't been seen yet.
    /// </summary>
    /// <param name="testSuite">The test suite to get the status for.</param>
    /// <returns>current registered TestSuiteStatus else TestSuiteStatus.UNKNOWN</returns>
    public static TestSuiteStatus GetTestSuiteStatus(Type testSuite)
    {
        if (!testSuite2Status.TryGetValue(testSuite, out var value))
        {
            value = TestSuiteStatus.UNKNOWN;
            testSuite2Status.Add(testSuite, value);
        }

        return value;
    }

    /// <summary>
    /// Gets the exception that caused a test suite to be skipped or fail, if any.
    /// </summary>
    /// <param name="testSuite">The test suite to get the exception for.</param>
    /// <returns>current registered Exception else null if none registered</returns>
    public static Exception? GetTestSuiteError(Type testSuite) =>
        testSuite2Error.TryGetValue(testSuite, out var value) ? value : null;

    /// <summary>
    /// Tallies how many of a test suite's (filtered) tests are at each <see cref="TestStatus"/>.
    /// </summary>
    /// <param name="testSuite">The test suite to tally the test statuses for.</param>
    /// <returns>A tally of how many of this suite's (filtered) tests are at each <see cref="TestStatus"/>.</returns>
    public static Tally<TestStatus> TallyTestStatuses(Type testSuite)
    {
        var tally = new Tally<TestStatus>();
        foreach (var test in FilteredExplorer.GetFilteredTests(testSuite))
        {
            tally[TestExplorer.GetTestStatus(test)]++;
        }
        return tally;
    }
}

/// <summary>
/// Stores and manage the links between registered test suites and their registered tests
/// </summary>
internal static class TestSuite2TestLink
{
    private static readonly Dictionary<Type, ISet<MethodInfo>> testSuite2Tests = [];

    /// <summary>
    /// Registers a test to a test suite. Can register multiple unique tests to the same test suite. Will not store duplicates.
    /// </summary>
    /// <param name="test">The test to register.</param>
    /// <param name="testSuite">The test suite to register the test to.</param>
    public static void RegisterTest2TestSuite(MethodInfo test, Type testSuite)
    {
        if (!testSuite2Tests.TryGetValue(testSuite, out var value))
        {
            value = new HashSet<MethodInfo>();
            testSuite2Tests[testSuite] = value;
        }

        _ = value.Add(test);
    }

    /// <summary>
    /// Gets the tests registered to a test suite.
    /// </summary>
    /// <param name="testSuite">The test suite to get the tests for.</param>
    /// <returns>A set of registered tests for this test suite. Returns an empty set if the test suite is not registered.</returns>
    public static ISet<MethodInfo> GetTests(Type testSuite) =>
        testSuite2Tests.TryGetValue(testSuite, out var value) ? value : new HashSet<MethodInfo>();
}

/// <summary>
/// Stores and manages a test suite's registered [BeforeEach]/[AfterEach] hooks.
/// </summary>
internal static class TestSuite2HookLink
{
    private static readonly Dictionary<Type, MethodInfo?> testSuite2BeforeEach = [];
    private static readonly Dictionary<Type, MethodInfo?> testSuite2AfterEach = [];

    /// <summary>
    /// Records the [BeforeEach] hook method for a test suite.
    /// </summary>
    /// <param name="testSuite">The test suite to record the hook for.</param>
    /// <param name="method">The hook method to record, or <c>null</c> to clear it.</param>
    public static void SetBeforeEach(Type testSuite, MethodInfo? method) =>
        testSuite2BeforeEach[testSuite] = method;

    /// <summary>
    /// Gets the [BeforeEach] hook method registered for a test suite, if any.
    /// </summary>
    /// <param name="testSuite">The test suite to get the hook for.</param>
    /// <returns>The registered [BeforeEach] method for this test suite, or null if none is registered.</returns>
    public static MethodInfo? GetBeforeEach(Type testSuite) =>
        testSuite2BeforeEach.TryGetValue(testSuite, out var value) ? value : null;

    /// <summary>
    /// Records the [AfterEach] hook method for a test suite.
    /// </summary>
    /// <param name="testSuite">The test suite to record the hook for.</param>
    /// <param name="method">The hook method to record, or <c>null</c> to clear it.</param>
    public static void SetAfterEach(Type testSuite, MethodInfo? method) =>
        testSuite2AfterEach[testSuite] = method;

    /// <summary>
    /// Gets the [AfterEach] hook method registered for a test suite, if any.
    /// </summary>
    /// <param name="testSuite">The test suite to get the hook for.</param>
    /// <returns>The registered [AfterEach] method for this test suite, or null if none is registered.</returns>
    public static MethodInfo? GetAfterEach(Type testSuite) =>
        testSuite2AfterEach.TryGetValue(testSuite, out var value) ? value : null;
}

/// <summary>
/// Stores and manages Test level data, aka known statuses and exceptions.
/// </summary>
internal static class TestExplorer
{
    private static readonly Dictionary<MethodInfo, TestStatus> test2Status = [];
    private static readonly Dictionary<MethodInfo, Exception?> test2Error = [];

    /// <summary>
    /// Records the status for a test.
    /// </summary>
    /// <param name="test">The test to record the status for.</param>
    /// <param name="status">The status to record.</param>
    public static void SetTestStatus(MethodInfo test, TestStatus status) =>
        test2Status[test] = status;

    /// <summary>
    /// Records the exception that caused a test to be skipped or fail, if any.
    /// </summary>
    /// <param name="test">The test to record the exception for.</param>
    /// <param name="error">The exception to record, or <c>null</c> to clear it.</param>
    public static void SetTestError(MethodInfo test, Exception? error) => test2Error[test] = error;

    /// <summary>
    /// Gets a test's registered status, registering it as <see cref="TestStatus.UNKNOWN"/> first if it hasn't been seen yet.
    /// </summary>
    /// <param name="test">The test to get the status for.</param>
    /// <returns>current registered TestStatus else TestStatus.UNKNOWN</returns>
    public static TestStatus GetTestStatus(MethodInfo test)
    {
        if (!test2Status.TryGetValue(test, out var value))
        {
            value = TestStatus.UNKNOWN;
            test2Status.Add(test, value);
        }

        return value;
    }

    /// <summary>
    /// Gets the exception that caused a test to be skipped or fail, if any.
    /// </summary>
    /// <param name="test">The test to get the exception for.</param>
    /// <returns>current registered Exception else null if none registered</returns>
    public static Exception? GetTestError(MethodInfo test) =>
        test2Error.TryGetValue(test, out var value) ? value : null;

    /// <summary>
    /// Gets every test that has a registered status.
    /// </summary>
    /// <returns>The known tests.</returns>
    public static ICollection<MethodInfo> AllKnownTests => test2Status.Keys;
}

/// <summary>
/// Explores and registers every tested assembly, test suites and tests currently loaded.
/// </summary>
internal static class Explorer
{
    /// <summary>
    /// Explores every assembly loaded in the current app domain, registering any test suites (and their tests and hooks) found within.
    /// </summary>
    public static void ExploreAndRegisterAssemblies()
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            ExploreAndRegisterTestSuites(asm);
            if (Assembly2TestSuiteLink.GetTestSuites(asm).Count != 0)
            {
                AssemblyExplorer.SetAssemblyStatus(asm, AssemblyStatus.UNKNOWN);
            }
        }
    }

    /// <summary>
    /// Explores an assembly, registering any test suites (and their tests and hooks) found within.
    /// Test suites that fail validation are registered with a <see cref="TestSuiteStatus.SKIP"/> status and their error recorded.
    /// </summary>
    /// <param name="asm">The assembly to explore.</param>
    public static void ExploreAndRegisterTestSuites(Assembly asm)
    {
        if (asm == null)
        {
            throw new ArgumentNullException(nameof(asm));
        }

        foreach (
            var testSuite in asm.GetTypes()
                .Where(type => type.TryGetAttribute<TestSuiteAttribute>() != null)
        )
        {
            try
            {
                Assembly2TestSuiteLink.RegisterTestSuite2Asm(testSuite, asm);
                Validator.IsValidTestSuite(testSuite);
                ExploreAndRegisterHooks(testSuite);
                TestSuiteExplorer.SetTestSuiteStatus(testSuite, TestSuiteStatus.UNKNOWN);
            }
            catch (Exception e)
            {
                TestSuiteExplorer.SetTestSuiteError(testSuite, e);
                TestSuiteExplorer.SetTestSuiteStatus(testSuite, TestSuiteStatus.SKIP);
                continue;
            }
            ExploreAndRegisterTests(testSuite);
        }
    }

    /// <summary>
    /// Discovers a test suite's [BeforeEach] and [AfterEach] hook methods, if any, and registers them
    /// with <see cref="TestSuite2HookLink"/>. Throws if a suite declares more than one of either, or if
    /// a declared hook doesn't meet the same static/void/parameter-free requirements as a test.
    /// </summary>
    /// <param name="testSuite">Entry point</param>
    public static void ExploreAndRegisterHooks(Type testSuite)
    {
        if (testSuite == null)
        {
            throw new ArgumentNullException(nameof(testSuite));
        }

        var methods = testSuite.GetMethods(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
        );

        var beforeEachMethods = methods
            .Where(info => info.TryGetAttribute<BeforeEachAttribute>() != null)
            .ToList();
        if (beforeEachMethods.Count > 1)
        {
            throw new InvalidTestSuiteException(
                $"{testSuite.Name}: a test suite may only declare one [BeforeEach] method."
            );
        }
        if (beforeEachMethods.Count == 1)
        {
            Validator.IsValidHook(beforeEachMethods[0]);
            TestSuite2HookLink.SetBeforeEach(testSuite, beforeEachMethods[0]);
        }

        var afterEachMethods = methods
            .Where(info => info.TryGetAttribute<AfterEachAttribute>() != null)
            .ToList();
        if (afterEachMethods.Count > 1)
        {
            throw new InvalidTestSuiteException(
                $"{testSuite.Name}: a test suite may only declare one [AfterEach] method."
            );
        }
        if (afterEachMethods.Count == 1)
        {
            Validator.IsValidHook(afterEachMethods[0]);
            TestSuite2HookLink.SetAfterEach(testSuite, afterEachMethods[0]);
        }
    }

    /// <summary>
    /// Explores a test suite, registering any tests found within.
    /// Tests that fail validation are registered with a <see cref="TestStatus.SKIP"/> status and their error recorded.
    /// </summary>
    /// <param name="testSuite">The test suite to explore.</param>
    public static void ExploreAndRegisterTests(Type testSuite)
    {
        if (testSuite == null)
        {
            throw new ArgumentNullException(nameof(testSuite));
        }

        foreach (
            var test in testSuite
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(info => info.TryGetAttribute<TestAttribute>() != null)
        )
        {
            try
            {
                TestSuite2TestLink.RegisterTest2TestSuite(test, testSuite);
                Validator.IsValidTest(test);
                TestExplorer.SetTestStatus(test, TestStatus.UNKNOWN);
            }
            catch (Exception e)
            {
                TestExplorer.SetTestError(test, e);
                TestExplorer.SetTestStatus(test, TestStatus.SKIP);
                continue;
            }
        }
    }
}
