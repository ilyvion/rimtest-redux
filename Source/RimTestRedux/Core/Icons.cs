namespace RimTestRedux.Core;

/// <summary>
/// Toolbar icons used by the RimTestRedux settings/test runner window.
/// </summary>
[StaticConstructorOnStartup]
internal static class Icons
{
    public static readonly Texture2D Collapse = ContentFinder<Texture2D>.Get("UI/RTRIconCollapse");
    public static readonly Texture2D Expand = ContentFinder<Texture2D>.Get("UI/RTRIconExpand");
    public static readonly Texture2D ChevronCollapsed = ContentFinder<Texture2D>.Get(
        "UI/RTRIconChevronCollapsed"
    );
    public static readonly Texture2D ChevronExpanded = ContentFinder<Texture2D>.Get(
        "UI/RTRIconChevronExpanded"
    );
    public static readonly Texture2D Run = ContentFinder<Texture2D>.Get("UI/RTRIconRun");
    public static readonly Texture2D Testing = ContentFinder<Texture2D>.Get("UI/RTRIconTesting");
    public static readonly Texture2D Log = ContentFinder<Texture2D>.Get("UI/RTRIconLog");
    public static readonly Texture2D Search = ContentFinder<Texture2D>.Get("UI/RTRIconSearch");
    public static readonly Texture2D StatusError = ContentFinder<Texture2D>.Get(
        "UI/RTRIconStatusError"
    );
    public static readonly Texture2D StatusWarning = ContentFinder<Texture2D>.Get(
        "UI/RTRIconStatusWarning"
    );
    public static readonly Texture2D StatusSkip = ContentFinder<Texture2D>.Get(
        "UI/RTRIconStatusSkip"
    );
    public static readonly Texture2D StatusUnknown = ContentFinder<Texture2D>.Get(
        "UI/RTRIconStatusUnknown"
    );
    public static readonly Texture2D StatusPass = ContentFinder<Texture2D>.Get(
        "UI/RTRIconStatusPass"
    );
}
