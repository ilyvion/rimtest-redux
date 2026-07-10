using System.Text.RegularExpressions;
using RimTestRedux.Util;

namespace RimTestRedux.Testing;

/// <summary>
/// Time profiling system
/// </summary>
public static class TimeElapsedExplorer
{
    private static readonly Dictionary<Assembly, double> asm2TimeElapsed = [];
    private static readonly Dictionary<Type, double> testSuite2TimeElapsed = [];
    private static readonly Dictionary<MethodInfo, double> test2TimeElapsed = [];

    /// <summary>
    /// </summary>
    public static void UpdateAllAssembliesTimeElapsed()
    {
        foreach (var asm in AssemblyExplorer.AllKnownAssemblies)
        {
            UpdateAssemblyTimeElapsed(asm);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
    /// <param name="time"></param>
    public static void SetAssemblyTimeElapsed(Assembly asm, double time) =>
        asm2TimeElapsed[asm] = time;

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
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
    /// </summary>
    /// <param name="asm"></param>
    /// <returns></returns>
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
    /// </summary>
    /// <param name="ts"></param>
    /// <param name="time"></param>
    public static void SetTestSuiteTimeElapsed(Type ts, double time) =>
        testSuite2TimeElapsed[ts] = time;

    /// <summary>
    /// </summary>
    /// <param name="ts"></param>
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
    /// </summary>
    /// <param name="ts"></param>
    /// <returns></returns>
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
    /// </summary>
    /// <param name="test"></param>
    /// <param name="time"></param>
    public static void SetTestTimeElapsed(MethodInfo test, double time) =>
        test2TimeElapsed[test] = time;

    /// <summary>
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public static double GetTestTimeElapsed(MethodInfo test) =>
        test2TimeElapsed.TryGetValue(test, out var value) ? value : -1;
}

/// <summary>
/// Keeps track of filtered tests for easy test finding
/// </summary>
public static class FilteredExplorer
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
    /// Are failed state T shown?
    /// </summary>
    internal static bool failEnabledT = true;

    /// <summary>
    /// Are unknown state T shown?
    /// </summary>
    internal static bool unknownEnabledT = true;

    /// <summary>
    /// Are skipped state T shown?
    /// </summary>
    internal static bool skipEnabledT = true;

    /// <summary>
    /// Are passed state T shown?
    /// </summary>
    internal static bool passEnabledT = true;

    /// <summary>
    /// </summary>
    /// <param name="filter"></param>
    public static void UpdateFilter(Regex filter) => FilteredExplorer.filter = filter;

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
    /// <returns></returns>
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
    /// </summary>
    /// <param name="ts"></param>
    /// <returns></returns>
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
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool DoesTestStatusMatchesFilter(MethodInfo t) =>
        TestExplorer.GetTestStatus(t) switch
        {
            TestStatus.SKIP => skipEnabledT,
            TestStatus.ERROR => failEnabledT,
            TestStatus.UNKNOWN => unknownEnabledT,
            TestStatus.PASS => passEnabledT,
            _ => false,
        };

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
    /// <returns></returns>
    public static bool DoesAssemblyMatchesFilter(Assembly asm) =>
        asm == null
            ? throw new ArgumentNullException(nameof(asm))
            : (
                filter.IsMatch(asm.GetName().Name)
                || Assembly2TestSuiteLink.GetTestSuites(asm).Any(DoesTestSuiteMatchesFilter)
            ) && DoesAssemblyStatusMatchesFilter(asm);

    /// <summary>
    /// </summary>
    /// <param name="testSuite"></param>
    /// <returns></returns>
    public static bool DoesTestSuiteMatchesFilter(Type testSuite) =>
        testSuite == null
            ? throw new ArgumentNullException(nameof(testSuite))
            : (
                filter.IsMatch(testSuite.Name)
                || TestSuite2TestLink.GetTests(testSuite).Any(DoesTestMatchesFilter)
            ) && DoesTestSuiteStatusMatchesFilter(testSuite);

    /// <summary>
    /// </summary>
    /// <param name="test"></param>
    /// <returns></returns>
    public static bool DoesTestMatchesFilter(MethodInfo test) =>
        test == null
            ? throw new ArgumentNullException(nameof(test))
            : filter.IsMatch(test.Name) && DoesTestStatusMatchesFilter(test);

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetFilteredAssemblies() =>
        [.. AssemblyExplorer.AllKnownAssemblies.Where(DoesAssemblyMatchesFilter)];

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetFilteredTestSuites() =>
        [.. TestSuiteExplorer.AllKnownTestSuites.Where(DoesTestSuiteMatchesFilter)];

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetFilteredTestSuites(Assembly asm) =>
        [.. Assembly2TestSuiteLink.GetTestSuites(asm).Where(DoesTestSuiteMatchesFilter)];

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<MethodInfo> GetFilteredTests() =>
        [.. TestExplorer.AllKnownTests.Where(DoesTestMatchesFilter)];

    /// <summary>
    /// </summary>
    /// <param name="ts"></param>
    /// <returns></returns>
    public static IEnumerable<MethodInfo> GetFilteredTests(Type ts) =>
        [.. TestSuite2TestLink.GetTests(ts).Where(DoesTestMatchesFilter)];
}

/// <summary>
/// Keeps track of test statuses
/// </summary>
public static class StatusExplorer
{
    private static readonly Dictionary<AssemblyStatus, int> asmStatus2count = [];
    private static readonly Dictionary<TestSuiteStatus, int> tsStatus2count = [];
    private static readonly Dictionary<TestStatus, int> tStatus2count = [];

    /// <summary>
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
    /// </summary>
    /// <param name="status"></param>
    public static void UpdateAssemblyStatusCount(AssemblyStatus status)
    {
        var value = FilteredExplorer
            .GetFilteredAssemblies()
            .Count(asm => AssemblyExplorer.GetAssemblyStatus(asm) == status);
        if (!asmStatus2count.TryAdd(status, value))
        {
            asmStatus2count[status] = value;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static int GetAssemblyStatusCount(AssemblyStatus status)
    {
        if (!asmStatus2count.ContainsKey(status))
        {
            UpdateAssemblyStatusCount(status);
        }

        return asmStatus2count[status];
    }

    /// <summary>
    /// </summary>
    /// <param name="status"></param>
    public static void UpdateTestSuiteStatusCount(TestSuiteStatus status)
    {
        var value = FilteredExplorer
            .GetFilteredTestSuites()
            .Count(ts => TestSuiteExplorer.GetTestSuiteStatus(ts) == status);
        if (!tsStatus2count.TryAdd(status, value))
        {
            tsStatus2count[status] = value;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="status"></param>
    public static int GetTestSuiteStatusCount(TestSuiteStatus status)
    {
        if (!tsStatus2count.ContainsKey(status))
        {
            UpdateTestSuiteStatusCount(status);
        }

        return tsStatus2count[status];
    }

    /// <summary>
    /// </summary>
    /// <param name="status"></param>
    public static void UpdateTestStatusCount(TestStatus status)
    {
        var value = FilteredExplorer
            .GetFilteredTests()
            .Count(t => TestExplorer.GetTestStatus(t) == status);
        if (!tStatus2count.TryAdd(status, value))
        {
            tStatus2count[status] = value;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="status"></param>
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
public static class AssemblyExplorer
{
    private static readonly Dictionary<Assembly, AssemblyStatus> asm2Status = [];
    private static readonly Dictionary<Assembly, Exception?> asm2Error = [];

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static ICollection<Assembly> AllKnownAssemblies => asm2Status.Keys;

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
    /// <param name="status"></param>
    public static void SetAssemblyStatus(Assembly asm, AssemblyStatus status) =>
        asm2Status[asm] = status;

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
    /// <param name="error"></param>
    public static void SetAssemblyError(Assembly asm, Exception? error) => asm2Error[asm] = error;

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
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
    /// </summary>
    /// <param name="asm"></param>
    /// <returns>The current registered Exception else null if none registered</returns>
    public static Exception? GetAssemblyError(Assembly asm) =>
        asm2Error.TryGetValue(asm, out var value) ? value : null;

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
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
public static class Assembly2TestSuiteLink
{
    private static readonly IDictionary<Assembly, ISet<Type>> asm2TestSuites =
        new Dictionary<Assembly, ISet<Type>>();

    /// <summary>
    /// Can register multiple unique test suites to the same assembly. Will not store duplicates.
    /// </summary>
    /// <param name="testSuite"></param>
    /// <param name="asm"></param>
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
    /// </summary>
    /// <returns>A list of all known assemblies</returns>
    public static List<Assembly> Assemblies => [.. asm2TestSuites.Keys];

    /// <summary>
    /// </summary>
    /// <param name="asm"></param>
    /// <returns>A set of registered tests suites for this assembly. Returns an empty set if the assembly is not registered.</returns>
    public static ISet<Type> GetTestSuites(Assembly asm) =>
        asm2TestSuites.TryGetValue(asm, out var value) ? value : new HashSet<Type>();
}

/// <summary>
/// Stores and manages Test Suite level data, aka known statuses and exceptions.
/// </summary>
public static class TestSuiteExplorer
{
    private static readonly Dictionary<Type, TestSuiteStatus> testSuite2Status = [];
    private static readonly Dictionary<Type, Exception?> testSuite2Error = [];

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static ICollection<Type> AllKnownTestSuites => testSuite2Status.Keys;

    /// <summary>
    /// </summary>
    /// <param name="testSuite"></param>
    /// <param name="status"></param>
    public static void SetTestSuiteStatus(Type testSuite, TestSuiteStatus status) =>
        testSuite2Status[testSuite] = status;

    /// <summary>
    /// </summary>
    /// <param name="testSuite"></param>
    /// <param name="error"></param>
    public static void SetTestSuiteError(Type testSuite, Exception? error) =>
        testSuite2Error[testSuite] = error;

    /// <summary>
    /// </summary>
    /// <param name="testSuite"></param>
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
    /// </summary>
    /// <param name="testSuite"></param>
    /// <returns>current registered Exception else null if none registered</returns>
    public static Exception? GetTestSuiteError(Type testSuite) =>
        testSuite2Error.TryGetValue(testSuite, out var value) ? value : null;

    /// <summary>
    /// </summary>
    /// <param name="testSuite"></param>
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
public static class TestSuite2TestLink
{
    private static readonly Dictionary<Type, ISet<MethodInfo>> testSuite2Tests = [];

    /// <summary>
    /// </summary>
    /// <param name="test"></param>
    /// <param name="testSuite"></param>
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
    /// </summary>
    /// <param name="testSuite"></param>
    /// <returns>A set of registered tests for this test suite. Returns an empty set if the test suite is not registered.</returns>
    public static ISet<MethodInfo> GetTests(Type testSuite) =>
        testSuite2Tests.TryGetValue(testSuite, out var value) ? value : new HashSet<MethodInfo>();
}

/// <summary>
/// Stores and manages Test level data, aka known statuses and exceptions.
/// </summary>
public static class TestExplorer
{
    private static readonly Dictionary<MethodInfo, TestStatus> test2Status = [];
    private static readonly Dictionary<MethodInfo, Exception?> test2Error = [];

    /// <summary>
    /// </summary>
    /// <param name="test"></param>
    /// <param name="status"></param>
    public static void SetTestStatus(MethodInfo test, TestStatus status) =>
        test2Status[test] = status;

    /// <summary>
    /// </summary>
    /// <param name="test"></param>
    /// <param name="error"></param>
    public static void SetTestError(MethodInfo test, Exception? error) => test2Error[test] = error;

    /// <summary>
    /// </summary>
    /// <param name="test"></param>
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
    /// </summary>
    /// <param name="test"></param>
    /// <returns>current registered Exception else null if none registered</returns>
    public static Exception? GetTestError(MethodInfo test) =>
        test2Error.TryGetValue(test, out var value) ? value : null;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static ICollection<MethodInfo> AllKnownTests => test2Status.Keys;
}

/// <summary>
/// Explores and registers every tested assembly, test suites and tests currently loaded.
/// </summary>
public static class Explorer
{
    /// <summary>
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
    /// </summary>
    /// <param name="asm">Entry point</param>
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
    /// </summary>
    /// <param name="testSuite">Entry point</param>
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
