using System.Text.RegularExpressions;
using RimTestRedux.Testing;

namespace RimTestRedux.Core;

/// <summary>
/// Main RimTestRedux test-runner dashboard: a CI-style overview of all registered assemblies and
/// their test suites, with per-suite drill-down details.
/// </summary>
[HotSwappable]
internal sealed class Window_TestRunner : Window
{
    private const int HEIGHT_CONTROLS = 26;
    private const int SIZE_CONTROLS_ICON_BTN = HEIGHT_CONTROLS;
    private const int WIDTH_CONTROLS_CONTROL_GAP = 6;
    private const int WIDTH_CONTROLS_SECTION_GAP = 18;

    private const int WIDTH_ASSEMBLY_RUN_BTN = SIZE_CONTROLS_ICON_BTN;
    private const int WIDTH_ASSEMBLY_TIME = 70;
    private const int HEIGHT_ASSEMBLY_STATS = 50;
    private const int WIDTH_ASSEMBLY_STAT = 90;

    private const int HEIGHT_SUITE_ROW = 40;
    private const int WIDTH_SUITE_RUN_BTN = 24;
    private const int WIDTH_SUITE_TIME = 60;
    private const int WIDTH_SUITE_RESULT_BAR = 140;
    private const int HEIGHT_SUITE_RESULT_BAR = 10;
    private const int WIDTH_SUITE_DETAILS_BTN = 80;
    private const int HEIGHT_SUITE_DETAILS_BTN = 28;

    private const int GAP_ASSEMBLY_BLOCK = 14;

    private readonly Dictionary<Assembly, bool> assemblyVisibility = [];
    private string searchRegex = @"";
    private Vector2 scrollPosition = Vector2.zero;

    public Window_TestRunner()
    {
        doCloseX = true;
        onlyOneOfTypeAllowed = true;
    }

    public override Vector2 InitialSize => new(900, 700);

    /// <summary>
    /// Draws the RimTestRedux test-runner dashboard
    /// </summary>
    public override void DoWindowContents(Rect canvas)
    {
        StatusExplorer.UpdateAllStatusCounts();
        var options = new Listing_Standard();

        options.Begin(canvas);
        DrawControls(options);
        options.End();

        var GUIRect = canvas.BottomPartPixels(canvas.height - options.CurHeight);
        var viewRect = GUIRect.LeftPartPixels(GUIRect.width - 16).AtZero();
        viewRect.Set(viewRect.xMin, viewRect.yMin, viewRect.width, CalcTotalHeight()); // update height to accomodate to all the data

        var testlisting = new Listing_Standard();
        Widgets.BeginScrollView(GUIRect, ref scrollPosition, viewRect, true);
        GUI.BeginGroup(viewRect);
        testlisting.Begin(viewRect);
        DrawAssemblies(testlisting);
        testlisting.End();
        GUI.EndGroup();
        Widgets.EndScrollView();

        //reset fonts
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
    }

    private void DrawControls(Listing_Standard bar)
    {
        var row = bar.GetRect(HEIGHT_CONTROLS);

        DrawIconButton(
            ref row,
            Icons.Run,
            "RimTestRedux.RunAllTestsTooltip".Translate(),
            () =>
            {
                Runner.RunAllRegisteredTests();
                StatusExplorer.UpdateAllStatusCounts();
                TimeElapsedExplorer.UpdateAllAssembliesTimeElapsed();
            }
        );
        DrawIconButton(
            ref row,
            Icons.Log,
            "RimTestRedux.LogResultsTooltip".Translate(),
            Viewer.LogTestsResults
        );
        RectCursor.TakeLeft(ref row, WIDTH_CONTROLS_SECTION_GAP);

        DrawIconButton(
            ref row,
            Icons.Collapse,
            "RimTestRedux.CollapseAllTooltip".Translate(),
            CollapseAllAssemblies
        );
        DrawIconButton(
            ref row,
            Icons.Expand,
            "RimTestRedux.ExpandAllTooltip".Translate(),
            ExpandAllAssemblies
        );
        RectCursor.TakeLeft(ref row, WIDTH_CONTROLS_SECTION_GAP);

        _ = RectCursor.TakeLeft(ref row, SIZE_CONTROLS_ICON_BTN, out var searchIconRect);
        GUI.DrawTexture(searchIconRect, Icons.Search);
        TooltipHandler.TipRegion(searchIconRect, "RimTestRedux.SearchTooltip".Translate());
        RectCursor.TakeLeft(ref row, WIDTH_CONTROLS_CONTROL_GAP);

        try
        {
            var searchRegexTmp = Widgets.TextField(row, searchRegex);
            if (searchRegexTmp != searchRegex)
            {
                searchRegex = searchRegexTmp;
                FilteredExplorer.UpdateFilter(new Regex(searchRegex, RegexOptions.IgnoreCase));
            }
        }
        catch (ArgumentException)
        {
            FilteredExplorer.UpdateFilter(new Regex(@""));
        }
    }

    private static void DrawIconButton(ref Rect row, Texture2D icon, string tooltip, Action onClick)
    {
        _ = RectCursor.TakeLeft(ref row, SIZE_CONTROLS_ICON_BTN, out var rect);
        TooltipHandler.TipRegion(rect, tooltip);
        if (Widgets.ButtonImage(rect, icon))
        {
            onClick();
        }
        RectCursor.TakeLeft(ref row, WIDTH_CONTROLS_CONTROL_GAP);
    }

    private int CalcTotalHeight()
    {
        var height = 0;
        foreach (var asm in FilteredExplorer.GetFilteredAssemblies())
        {
            // name row + stat cards + gapline + "Test Suites" sub-header, all always visible
            height += HEIGHT_CONTROLS + HEIGHT_ASSEMBLY_STATS + 4 + HEIGHT_CONTROLS;
            if (GetAssemblyVisibility(asm))
            {
                height += FilteredExplorer.GetFilteredTestSuites(asm).Count() * HEIGHT_SUITE_ROW;
            }
            height += GAP_ASSEMBLY_BLOCK;
        }
        return height;
    }

    private void ToggleAllAssemblies(bool toggle)
    {
        foreach (var asm in FilteredExplorer.GetFilteredAssemblies())
        {
            assemblyVisibility[asm] = toggle;
        }
    }

    private void CollapseAllAssemblies() => ToggleAllAssemblies(false);

    private void ExpandAllAssemblies() => ToggleAllAssemblies(true);

    private void DrawAssemblies(Listing_Standard listing)
    {
        foreach (var asm in FilteredExplorer.GetFilteredAssemblies())
        {
            DrawAssemblyBlock(listing, asm);
        }
    }

    private bool GetAssemblyVisibility(Assembly asm)
    {
        if (!assemblyVisibility.TryGetValue(asm, out var value))
        {
            value = true;
            assemblyVisibility[asm] = value;
        }

        return value;
    }

    private void ToggleAssemblyVisibility(Assembly asm) =>
        assemblyVisibility[asm] = !GetAssemblyVisibility(asm);

    private void DrawAssemblyBlock(Listing_Standard listing, Assembly asm)
    {
        // header: name .......... [run] time -- always visible, does not collapse
        var headerRow = listing.GetRect(HEIGHT_CONTROLS);
        var actionsRect = headerRow.RightPartPixels(WIDTH_ASSEMBLY_RUN_BTN + WIDTH_ASSEMBLY_TIME);
        var nameRect = headerRow.LeftPartPixels(
            headerRow.width - actionsRect.width - WIDTH_CONTROLS_CONTROL_GAP
        );

        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(nameRect, asm.GetName().Name);
        Text.Font = GameFont.Small;

        DrawIconButton(
            ref actionsRect,
            Icons.Run,
            "RimTestRedux.RunAssemblyTooltip".Translate(),
            () =>
            {
                Runner.RunAssembly(asm);
                StatusExplorer.UpdateAllStatusCounts();
                TimeElapsedExplorer.UpdateAssemblyTimeElapsed(asm);
            }
        );
        Text.Anchor = TextAnchor.MiddleLeft;
        var asmTimeElapsed = TimeElapsedExplorer.GetAssemblyTimeElapsed(asm);
        Widgets.Label(actionsRect, asmTimeElapsed < 0 ? "--" : $"{asmTimeElapsed:0} ms");
        Text.Anchor = TextAnchor.UpperLeft;

        // overview stat cards: Tests / Passed / Failed / Skipped / Not run
        var statsRow = listing.GetRect(HEIGHT_ASSEMBLY_STATS);
        var tally = AssemblyExplorer.TallyTestStatuses(asm);
        var total = tally.Values.Sum();
        DrawStatCard(ref statsRow, "RimTestRedux.StatTests".Translate(), total, Color.white);
        DrawStatCard(
            ref statsRow,
            "RimTestRedux.StatPassed".Translate(),
            tally[TestStatus.PASS],
            StatusStyle.ColorPass
        );
        DrawStatCard(
            ref statsRow,
            "RimTestRedux.StatFailed".Translate(),
            tally[TestStatus.ERROR],
            StatusStyle.ColorFail
        );
        DrawStatCard(
            ref statsRow,
            "RimTestRedux.StatSkipped".Translate(),
            tally[TestStatus.SKIP],
            StatusStyle.ColorWarn
        );
        DrawStatCard(
            ref statsRow,
            "RimTestRedux.StatNotRun".Translate(),
            tally[TestStatus.UNKNOWN],
            StatusStyle.ColorUnknown
        );

        listing.GapLine(4f);

        // "Test Suites" sub-header: this row (and only this row) owns the collapse/expand
        // toggle for the suite list directly beneath it, so it's clear the stat cards above
        // always stay visible regardless of the toggle state.
        var suitesCount = FilteredExplorer.GetFilteredTestSuites(asm).Count();
        var isExpanded = GetAssemblyVisibility(asm);
        var suitesHeaderRow = listing.GetRect(HEIGHT_CONTROLS);
        var suitesHeaderFullRect = suitesHeaderRow;
        if (Mouse.IsOver(suitesHeaderFullRect))
        {
            Widgets.DrawHighlight(suitesHeaderFullRect);
        }
        _ = RectCursor.TakeLeft(ref suitesHeaderRow, SIZE_CONTROLS_ICON_BTN, out var chevronRect);
        GUI.DrawTexture(chevronRect, isExpanded ? Icons.ChevronExpanded : Icons.ChevronCollapsed);
        RectCursor.TakeLeft(ref suitesHeaderRow, WIDTH_CONTROLS_CONTROL_GAP);
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(suitesHeaderRow, "RimTestRedux.TestSuitesHeader".Translate(suitesCount));
        Text.Anchor = TextAnchor.UpperLeft;
        if (Widgets.ButtonInvisible(suitesHeaderFullRect))
        {
            ToggleAssemblyVisibility(asm);
        }

        if (isExpanded)
        {
            foreach (var ts in FilteredExplorer.GetFilteredTestSuites(asm))
            {
                DrawTestSuiteRow(listing, ts);
            }
        }

        listing.Gap(GAP_ASSEMBLY_BLOCK);
    }

    private static void DrawStatCard(ref Rect row, string label, int count, Color valueColor)
    {
        _ = RectCursor.TakeLeft(ref row, WIDTH_ASSEMBLY_STAT, out var rect);
        var numberRect = rect.TopPartPixels(rect.height * 0.6f);
        var labelRect = rect.BottomPartPixels(rect.height * 0.4f);

        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleLeft;
        var prevColor = GUI.color;
        GUI.color = valueColor;
        Widgets.Label(numberRect, count.ToString(CultureInfo.InvariantCulture));
        GUI.color = prevColor;

        Text.Font = GameFont.Tiny;
        Widgets.Label(labelRect, label);

        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
    }

    private static void DrawTestSuiteRow(Listing_Standard listing, Type ts)
    {
        var row = listing.GetRect(HEIGHT_SUITE_ROW);
        if (Mouse.IsOver(row))
        {
            Widgets.DrawHighlight(row);
        }

        _ = RectCursor.TakeLeft(ref row, WIDTH_SUITE_RUN_BTN, out var runSlot);
        var runRect = RectCursor.CenterSquare(runSlot);
        TooltipHandler.TipRegion(runRect, "RimTestRedux.RunSuiteTooltip".Translate());
        if (Widgets.ButtonImage(runRect, Icons.Run))
        {
            Runner.RunTestSuite(ts);
            StatusExplorer.UpdateAllStatusCounts();
            TimeElapsedExplorer.UpdateAssemblyTimeElapsed(ts.Assembly);
        }
        RectCursor.TakeLeft(ref row, WIDTH_CONTROLS_CONTROL_GAP);

        var detailsSlot = row.RightPartPixels(WIDTH_SUITE_DETAILS_BTN);
        var detailsRect = RectCursor.CenterVertically(detailsSlot, HEIGHT_SUITE_DETAILS_BTN);
        row = row.LeftPartPixels(row.width - WIDTH_SUITE_DETAILS_BTN - WIDTH_CONTROLS_CONTROL_GAP);

        var timeRect = row.RightPartPixels(WIDTH_SUITE_TIME);
        row = row.LeftPartPixels(row.width - WIDTH_SUITE_TIME - WIDTH_CONTROLS_CONTROL_GAP);

        var barSlot = row.RightPartPixels(WIDTH_SUITE_RESULT_BAR);
        var barRect = RectCursor.CenterVertically(barSlot, HEIGHT_SUITE_RESULT_BAR);
        row = row.LeftPartPixels(row.width - WIDTH_SUITE_RESULT_BAR - WIDTH_CONTROLS_CONTROL_GAP);

        var nameRect = row;

        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(nameRect, ts.Name);

        Text.Anchor = TextAnchor.MiddleRight;
        var timeElapsed = TimeElapsedExplorer.GetTestSuiteTimeElapsed(ts);
        Widgets.Label(timeRect, timeElapsed < 0 ? "--" : $"{timeElapsed:0} ms");
        Text.Anchor = TextAnchor.UpperLeft;

        DrawResultBar(barRect, TestSuiteExplorer.TallyTestStatuses(ts));

        Text.Anchor = TextAnchor.MiddleCenter;
        if (Widgets.ButtonText(detailsRect, "RimTestRedux.DetailsButton".Translate()))
        {
            Find.WindowStack.Add(new Window_TestSuiteDetails(ts));
        }
        Text.Anchor = TextAnchor.UpperLeft;
    }

    /// <summary>
    /// Draws a stacked bar showing the proportion of a test suite's tests at each status
    /// (passed/failed/skipped/not run), similar to a CI test-report result bar.
    /// </summary>
    private static void DrawResultBar(Rect rect, Util.Tally<TestStatus> tally)
    {
        Widgets.DrawBoxSolid(rect, new Color(1f, 1f, 1f, 0.08f));

        var total = tally.Values.Sum();
        if (total <= 0)
        {
            return;
        }

        var x = rect.x;
        void Segment(TestStatus status)
        {
            var count = tally[status];
            if (count <= 0)
            {
                return;
            }
            var width = rect.width * count / total;
            Widgets.DrawBoxSolid(
                new Rect(x, rect.y, width, rect.height),
                StatusStyle.GetColor(status)
            );
            x += width;
        }

        Segment(TestStatus.PASS);
        Segment(TestStatus.ERROR);
        Segment(TestStatus.SKIP);
        Segment(TestStatus.UNKNOWN);
    }
}
