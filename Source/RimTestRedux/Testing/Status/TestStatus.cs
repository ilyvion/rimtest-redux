namespace RimTestRedux.Testing;

/// <summary>
/// Registered Test current execution status.
/// </summary>
internal enum TestStatus
{
    /// <summary>
    /// invalid test
    /// </summary>
    SKIP,

    /// <summary>
    /// not run yet
    /// </summary>
    UNKNOWN,

    /// <summary>
    /// failed
    /// </summary>
    ERROR,

    /// <summary>
    /// passed
    /// </summary>
    PASS,
}

/// <summary>
/// Utils for TestStatus
/// </summary>
internal static class TestStatusExtension
{
    /// <summary>
    /// Value to string symbol converter
    /// </summary>
    /// <param name="status"></param>
    /// <returns>user friendly string representation</returns>
    public static string StatusSymbol(TestStatus status) =>
        status switch
        {
            TestStatus.UNKNOWN => "??",
            TestStatus.SKIP => "➥",
            TestStatus.PASS => "✓",
            TestStatus.ERROR => "✘",
            _ => "",
        };
}
