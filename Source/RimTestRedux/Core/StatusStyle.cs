using RimTestRedux.Testing;

namespace RimTestRedux.Core;

/// <summary>
/// Single source of truth for how each test/test suite/assembly status is represented visually
/// (color and icon), so the status-to-visual mapping isn't duplicated at every draw site.
/// </summary>
internal static class StatusStyle
{
    public static readonly Color ColorFail = new(0.878f, 0.224f, 0.231f);
    public static readonly Color ColorWarn = new(1f, 0.639f, 0.267f);
    public static readonly Color ColorPass = new(0.706f, 0.933f, 0.251f);
    public static readonly Color ColorUnknown = new(0.184f, 0.533f, 0.631f);

    public static Color GetColor(AssemblyStatus status) =>
        status switch
        {
            AssemblyStatus.ERROR => ColorFail,
            AssemblyStatus.WARNING => ColorWarn,
            AssemblyStatus.PASS => ColorPass,
            AssemblyStatus.UNKNOWN => ColorUnknown,
            _ => throw new NotImplementedException(),
        };

    public static Color GetColor(TestSuiteStatus status) =>
        status switch
        {
            TestSuiteStatus.WARNING or TestSuiteStatus.SKIP => ColorWarn,
            TestSuiteStatus.ERROR => ColorFail,
            TestSuiteStatus.PASS => ColorPass,
            TestSuiteStatus.UNKNOWN => ColorUnknown,
            _ => throw new NotImplementedException(),
        };

    public static Color GetColor(TestStatus status) =>
        status switch
        {
            TestStatus.SKIP => ColorWarn,
            TestStatus.ERROR => ColorFail,
            TestStatus.PASS => ColorPass,
            TestStatus.UNKNOWN => ColorUnknown,
            _ => throw new NotImplementedException(),
        };

    public static Texture2D GetIcon(TestStatus status) =>
        status switch
        {
            TestStatus.SKIP => Icons.StatusSkip,
            TestStatus.ERROR => Icons.StatusError,
            TestStatus.PASS => Icons.StatusPass,
            TestStatus.UNKNOWN => Icons.StatusUnknown,
            _ => throw new NotImplementedException(),
        };
}
