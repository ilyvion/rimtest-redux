namespace RimTestRedux.Core;

/// <summary>
/// Compact icon+count control-bar badge that toggles a status filter bool on click, shared
/// between <see cref="Window_TestRunner"/> (assembly/test-suite tiers) and
/// <see cref="Window_TestSuiteDetails"/> (test tier).
/// </summary>
[HotSwappable]
internal static class StatusFilterBadge
{
    private const int SIZE_ICON = 26;
    private const int WIDTH_COUNT = 28;
    private const int GAP_ICON_COUNT = 4;
    private const int WIDTH_BADGE = SIZE_ICON + GAP_ICON_COUNT + WIDTH_COUNT;
    private const int WIDTH_GAP = 14;

    /// <summary>
    /// Draws a single badge, cutting it off the left of <paramref name="row"/>, and toggles
    /// <paramref name="enabled"/> when clicked.
    /// </summary>
    public static void Draw(ref Rect row, Texture2D icon, string label, int count, ref bool enabled)
    {
        _ = RectCursor.TakeLeft(ref row, WIDTH_BADGE, out var rect);
        var iconRect = RectCursor.CenterVertically(
            new Rect(rect.x, rect.y, SIZE_ICON, rect.height),
            SIZE_ICON
        );
        var countRect = new Rect(iconRect.xMax + GAP_ICON_COUNT, rect.y, WIDTH_COUNT, rect.height);

        if (Mouse.IsOver(rect))
        {
            Widgets.DrawHighlight(rect);
        }

        var prevColor = GUI.color;
        GUI.color = enabled ? Color.white : new Color(1f, 1f, 1f, 0.35f);
        GUI.DrawTexture(iconRect, icon);
        GUI.color = prevColor;

        var prevAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(countRect, count.ToString(CultureInfo.InvariantCulture));
        Text.Anchor = prevAnchor;

        TooltipHandler.TipRegion(
            rect,
            enabled
                ? "RimTestRedux.FilterHideTooltip".Translate(label, count)
                : "RimTestRedux.FilterShowTooltip".Translate(label, count)
        );

        if (Widgets.ButtonInvisible(rect))
        {
            enabled = !enabled;
        }

        RectCursor.TakeLeft(ref row, WIDTH_GAP);
    }
}
