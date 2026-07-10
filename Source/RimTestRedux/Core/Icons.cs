namespace RimTestRedux.Core;

/// <summary>
/// Toolbar icons used by the RimTestRedux settings/test runner window.
/// </summary>
[StaticConstructorOnStartup]
internal static class Icons
{
    public static readonly Texture2D Collapse = ContentFinder<Texture2D>.Get("UI/icon_collapse");
    public static readonly Texture2D Expand = ContentFinder<Texture2D>.Get("UI/icon_expand");
    public static readonly Texture2D ChevronCollapsed = ContentFinder<Texture2D>.Get(
        "UI/icon_chevron_collapsed"
    );
    public static readonly Texture2D ChevronExpanded = ContentFinder<Texture2D>.Get(
        "UI/icon_chevron_expanded"
    );
    public static readonly Texture2D Run = ContentFinder<Texture2D>.Get("UI/icon_run");
    public static readonly Texture2D Log = ContentFinder<Texture2D>.Get("UI/icon_log");
    public static readonly Texture2D Search = ContentFinder<Texture2D>.Get("UI/icon_search");
    public static readonly Texture2D StatusError = ContentFinder<Texture2D>.Get(
        "UI/icon_status_error"
    );
    public static readonly Texture2D StatusWarning = ContentFinder<Texture2D>.Get(
        "UI/icon_status_warning"
    );
    public static readonly Texture2D StatusSkip = ContentFinder<Texture2D>.Get(
        "UI/icon_status_skip"
    );
    public static readonly Texture2D StatusUnknown = ContentFinder<Texture2D>.Get(
        "UI/icon_status_unknown"
    );
    public static readonly Texture2D StatusPass = ContentFinder<Texture2D>.Get(
        "UI/icon_status_pass"
    );
}
