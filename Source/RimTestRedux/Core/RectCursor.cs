namespace RimTestRedux.Core;

/// <summary>
/// Small helpers for laying out toolbar/row-style UI by repeatedly slicing pixels off the left
/// of a <see cref="Rect"/> and advancing past them, shared between <see cref="Settings"/> and
/// <see cref="Window_TestSuiteDetails"/>.
/// </summary>
internal static class RectCursor
{
    /// <summary>
    /// Cuts <paramref name="width"/> pixels off the left of <paramref name="row"/>, returning
    /// them as <paramref name="taken"/> and advancing <paramref name="row"/> past them.
    /// </summary>
    public static Rect TakeLeft(ref Rect row, float width, out Rect taken)
    {
        taken = row.LeftPartPixels(width);
        row = row.RightPartPixels(row.width - width);
        return taken;
    }

    public static void TakeLeft(ref Rect row, float width) => _ = TakeLeft(ref row, width, out _);

    /// <summary>
    /// Returns the largest centered square that fits within <paramref name="outer"/>, so a
    /// texture drawn into it isn't stretched non-uniformly by a non-square slot.
    /// </summary>
    public static Rect CenterSquare(Rect outer)
    {
        var size = Mathf.Min(outer.width, outer.height);
        return new Rect(
            outer.x + ((outer.width - size) / 2f),
            outer.y + ((outer.height - size) / 2f),
            size,
            size
        );
    }

    /// <summary>
    /// Centers a rect of the given <paramref name="height"/> vertically within
    /// <paramref name="outer"/>, keeping its full width, so controls can be drawn a little
    /// shorter than their row for visual breathing room between rows.
    /// </summary>
    public static Rect CenterVertically(Rect outer, float height) =>
        new(outer.x, outer.y + ((outer.height - height) / 2f), outer.width, height);
}
