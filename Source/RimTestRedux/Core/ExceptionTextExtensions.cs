namespace RimTestRedux.Core;

/// <summary>
/// Helpers for rendering test-failure exceptions, most of which arrive wrapped in a
/// <see cref="TargetInvocationException"/> from <c>MethodInfo.Invoke</c>.
/// </summary>
internal static class ExceptionTextExtensions
{
    /// <summary>
    /// The innermost exception's message, e.g. the assertion failure text rather than
    /// "Exception has been thrown by the target of an invocation."
    /// </summary>
    public static string ShortMessage(this Exception exception)
    {
        var inner = exception;
        while (inner.InnerException != null)
        {
            inner = inner.InnerException;
        }
        return inner.Message;
    }

    /// <summary>
    /// The full exception text, bypassing Harmony's stack-trace caching so a repeated failure
    /// renders its actual stack trace instead of "[Ref ...] Duplicate stacktrace, see ref for
    /// original".
    /// </summary>
    public static string FullText(this Exception exception)
    {
        ref var noStacktraceCaching = ref AccessTools.StaticFieldRefAccess<bool>(
            "HarmonyMod.HarmonyMain:noStacktraceCaching"
        );

        var original = noStacktraceCaching;
        noStacktraceCaching = true;
        try
        {
            return exception.ToString();
        }
        finally
        {
            noStacktraceCaching = original;
        }
    }
}
