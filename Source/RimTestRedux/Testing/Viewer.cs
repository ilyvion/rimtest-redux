using System.Text;
using RimTestRedux.Util;

namespace RimTestRedux.Testing;

internal static class Viewer
{
    private static void LogDetailedErrors(ICollection<Assembly> asms)
    {
        foreach (var asm in asms)
        {
            var asmStatus = AssemblyExplorer.GetAssemblyStatus(asm);
            var asmName = asm.GetName().Name;
            switch (asmStatus)
            {
                case AssemblyStatus.WARNING:
                    RimTestReduxMod.Instance.LogWarning(
                        $"[{AssemblyStatusExtension.StatusSymbol(asmStatus)}] {asmName} > {AssemblyExplorer.GetAssemblyError(asm)?.Message}"
                    );
                    break;
                case AssemblyStatus.ERROR:
                    RimTestReduxMod.Instance.LogError(
                        $"[{AssemblyStatusExtension.StatusSymbol(asmStatus)}] {asmName} > {AssemblyExplorer.GetAssemblyError(asm)}"
                    );
                    break;
                case AssemblyStatus.UNKNOWN:
                    RimTestReduxMod.Instance.LogMessage(
                        $"[{AssemblyStatusExtension.StatusSymbol(asmStatus)}] {asmName} > Not Run Yet"
                    );
                    break;
                case AssemblyStatus.PASS:
                default:
                    break;
            }
            //Errored tests display
            foreach (var testSuite in Assembly2TestSuiteLink.GetTestSuites(asm))
            {
                var tsStatus = TestSuiteExplorer.GetTestSuiteStatus(testSuite);
                if (tsStatus is TestSuiteStatus.PASS)
                {
                    continue;
                }

                switch (tsStatus)
                {
                    case TestSuiteStatus.WARNING:
                    case TestSuiteStatus.SKIP:
                        RimTestReduxMod.Instance.LogWarning(
                            $"    [{TestSuiteStatusExtension.StatusSymbol(tsStatus)}] {testSuite.Name} > {TestSuiteExplorer.GetTestSuiteError(testSuite)}"
                        );
                        break;
                    case TestSuiteStatus.ERROR:
                        RimTestReduxMod.Instance.LogError(
                            $"    [{TestSuiteStatusExtension.StatusSymbol(tsStatus)}] {testSuite.Name} > {TestSuiteExplorer.GetTestSuiteError(testSuite)}"
                        );
                        break;
                    case TestSuiteStatus.UNKNOWN:
                        RimTestReduxMod.Instance.LogMessage(
                            $"    [{TestSuiteStatusExtension.StatusSymbol(tsStatus)}] {testSuite.Name} > Not Run Yet"
                        );
                        break;
                    case TestSuiteStatus.PASS:
                    default:
                        break;
                }

                foreach (var test in TestSuite2TestLink.GetTests(testSuite))
                {
                    var tStatus = TestExplorer.GetTestStatus(test);
                    switch (tStatus)
                    {
                        case TestStatus.SKIP:
                            RimTestReduxMod.Instance.LogWarning(
                                $"        [{TestStatusExtension.StatusSymbol(tStatus)}] {testSuite.Name}.{test.Name} > {TestExplorer.GetTestError(test)}"
                            );
                            continue;
                        case TestStatus.ERROR:
                            RimTestReduxMod.Instance.LogError(
                                $"        [{TestStatusExtension.StatusSymbol(tStatus)}] {testSuite.Name}.{test.Name} > {TestExplorer.GetTestError(test)}"
                            );
                            continue;
                        case TestStatus.UNKNOWN:
                            RimTestReduxMod.Instance.LogMessage(
                                $"        [{TestStatusExtension.StatusSymbol(tStatus)}] {testSuite.Name}.{test.Name} > Not Run Yet"
                            );
                            continue;
                        case TestStatus.PASS:
                        default:
                            break;
                    }
                }
            }
        }
    }

    private static string BuildAsmSummary(
        Assembly asm,
        Tally<TestSuiteStatus> tsTally,
        Tally<TestStatus> tTally
    )
    {
        var builder = new StringBuilder();

        _ = builder.Append(
            $"[{AssemblyStatusExtension.StatusSymbol(AssemblyExplorer.GetAssemblyStatus(asm))}] {asm.GetName().Name} "
        );
        _ = builder.Append($"|| Test Suites :");
        foreach (TestSuiteStatus status in Enum.GetValues(typeof(TestSuiteStatus)))
        {
            var tally = tsTally[status];
            if (tally != 0)
            {
                _ = builder.Append($" {tally} {TestSuiteStatusExtension.StatusSymbol(status)} ");
            }
        }

        _ = builder.Append($"|| Tests :");
        foreach (TestStatus status in Enum.GetValues(typeof(TestStatus)))
        {
            var tally = tTally[status];
            if (tally != 0)
            {
                _ = builder.Append($" {tally} {TestStatusExtension.StatusSymbol(status)} ");
            }
        }

        return builder.ToString();
    }

    private static void LogSummary(ICollection<Assembly> asms)
    {
        foreach (var asm in asms)
        {
            Tally<TestSuiteStatus> tsTally = [];
            Tally<TestStatus> tTally = [];

            // test results tallying
            foreach (var testSuite in Assembly2TestSuiteLink.GetTestSuites(asm))
            {
                tsTally[TestSuiteExplorer.GetTestSuiteStatus(testSuite)]++;

                foreach (var test in TestSuite2TestLink.GetTests(testSuite))
                {
                    tTally[TestExplorer.GetTestStatus(test)]++;
                }
            }

            var asmResult = BuildAsmSummary(asm, tsTally, tTally);

            switch (AssemblyExplorer.GetAssemblyStatus(asm))
            {
                case AssemblyStatus.UNKNOWN:
                case AssemblyStatus.PASS:
                    RimTestReduxMod.Instance.LogMessage(asmResult);
                    continue;
                case AssemblyStatus.WARNING:
                    RimTestReduxMod.Instance.LogWarning(asmResult);
                    break;
                case AssemblyStatus.ERROR:
                    RimTestReduxMod.Instance.LogError(asmResult);
                    break;
                default:
                    break;
            }
        }
    }

    public static void LogTestsResults()
    {
        var asms = Assembly2TestSuiteLink.Assemblies;
        asms.SortBy(asm => asm.FullName);
        if (!RimTestReduxMod.Settings.RunOwnTests)
        {
            _ = asms.Remove(Assembly.GetExecutingAssembly());
        }

        RimTestReduxMod.Instance.LogMessage("TESTING START");
        RimTestReduxMod.Instance.LogMessage("SUMMARY");
        LogSummary(asms);
        RimTestReduxMod.Instance.LogMessage("ERRORS");
        LogDetailedErrors(asms);
        RimTestReduxMod.Instance.LogMessage("TESTING END");
    }
}
