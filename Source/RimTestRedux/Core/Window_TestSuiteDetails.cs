using RimTestRedux.Testing;

namespace RimTestRedux.Core;

/// <summary>
/// Drill-down dialog listing the individual test cases of a single test suite, with expandable
/// rows revealing the failure/skip reason in place of the test-case "attachments" a typical CI
/// test report would show.
/// </summary>
[HotSwappable]
internal sealed class Window_TestSuiteDetails : Window
{
    private const int HEIGHT_SUMMARY_STAT = 50;
    private const int WIDTH_SUMMARY_STAT = 100;
    private const int HEIGHT_TEST_ROW = 32;
    private const int HEIGHT_TEST_DETAIL = 160;
    private const int WIDTH_STATUS_ICON = 22;
    private const int WIDTH_TEST_RUN_BTN = 40;
    private const int WIDTH_TEST_NAME = 150;
    private const int WIDTH_TEST_TIME = 60;
    private const int WIDTH_EXPAND_BTN = 24;
    private const int GAP = 4;

    private readonly Type testSuite;
    private readonly HashSet<MethodInfo> expandedTests = [];
    private readonly Dictionary<MethodInfo, Vector2> detailScrollPositions = [];
    private Vector2 scrollPosition = Vector2.zero;

    // These filters are local to this window rather than sharing FilteredExplorer's static
    // state, so hiding a status here doesn't also filter the main test runner window's tree
    // and summary counts.
    private bool failEnabledT = true;
    private bool skipEnabledT = true;
    private bool unknownEnabledT = true;
    private bool passEnabledT = true;

    public Window_TestSuiteDetails(Type testSuite)
    {
        this.testSuite = testSuite;
        doCloseX = true;
        onlyOneOfTypeAllowed = false;
    }

    public override Vector2 InitialSize => new(640, 640);

    public override void DoWindowContents(Rect inRect)
    {
        var listing = new Listing_Standard();
        listing.Begin(inRect);

        Text.Font = GameFont.Medium;
        _ = listing.Label(testSuite.Name);
        Text.Font = GameFont.Small;

        var suiteError = TestSuiteExplorer.GetTestSuiteError(testSuite);
        if (suiteError != null)
        {
            var prevColor = GUI.color;
            GUI.color = StatusStyle.ColorFail;
            _ = listing.Label(suiteError.FullText());
            GUI.color = prevColor;
        }

        listing.GapLine();

        var tally = TestSuiteExplorer.TallyTestStatuses(testSuite);
        var statsRow = listing.GetRect(HEIGHT_SUMMARY_STAT);
        DrawSummaryStat(
            ref statsRow,
            "RimTestRedux.StatPassed".Translate(),
            tally[TestStatus.PASS],
            StatusStyle.ColorPass
        );
        DrawSummaryStat(
            ref statsRow,
            "RimTestRedux.StatFailed".Translate(),
            tally[TestStatus.ERROR],
            StatusStyle.ColorFail
        );
        DrawSummaryStat(
            ref statsRow,
            "RimTestRedux.StatSkipped".Translate(),
            tally[TestStatus.SKIP],
            StatusStyle.ColorWarn
        );
        DrawSummaryStat(
            ref statsRow,
            "RimTestRedux.StatNotRun".Translate(),
            tally[TestStatus.UNKNOWN],
            StatusStyle.ColorUnknown
        );

        listing.GapLine();

        var filterRow = listing.GetRect(HEIGHT_TEST_ROW);
        StatusFilterBadge.Draw(
            ref filterRow,
            Icons.StatusError,
            "RimTestRedux.StatFailed".Translate(),
            tally[TestStatus.ERROR],
            ref failEnabledT
        );
        StatusFilterBadge.Draw(
            ref filterRow,
            Icons.StatusSkip,
            "RimTestRedux.StatSkipped".Translate(),
            tally[TestStatus.SKIP],
            ref skipEnabledT
        );
        StatusFilterBadge.Draw(
            ref filterRow,
            Icons.StatusUnknown,
            "RimTestRedux.StatNotRun".Translate(),
            tally[TestStatus.UNKNOWN],
            ref unknownEnabledT
        );
        StatusFilterBadge.Draw(
            ref filterRow,
            Icons.StatusPass,
            "RimTestRedux.StatPassed".Translate(),
            tally[TestStatus.PASS],
            ref passEnabledT
        );

        listing.GapLine();

        var tests = TestSuite2TestLink
            .GetTests(testSuite)
            .Where(t =>
                TestExplorer.GetTestStatus(t) switch
                {
                    TestStatus.SKIP => skipEnabledT,
                    TestStatus.ERROR => failEnabledT,
                    TestStatus.UNKNOWN => unknownEnabledT,
                    TestStatus.PASS => passEnabledT,
                    _ => false,
                }
            )
            .OrderBy(t => t.Name)
            .ToList();

        var listRect = listing.GetRect(inRect.height - listing.CurHeight - GAP);
        var viewRect = new Rect(0, 0, listRect.width - GenUI.ScrollBarWidth, CalcListHeight(tests));
        Widgets.BeginScrollView(listRect, ref scrollPosition, viewRect);
        var innerListing = new Listing_Standard();
        innerListing.Begin(viewRect);
        foreach (var test in tests)
        {
            DrawTestRow(innerListing, test);
        }
        innerListing.End();
        Widgets.EndScrollView();

        listing.End();
    }

    private int CalcListHeight(List<MethodInfo> tests)
    {
        var height = 0;
        foreach (var test in tests)
        {
            height += HEIGHT_TEST_ROW;
            if (expandedTests.Contains(test) && TestExplorer.GetTestError(test) != null)
            {
                height += HEIGHT_TEST_DETAIL;
            }
        }
        return height;
    }

    private static void DrawSummaryStat(ref Rect row, string label, int count, Color valueColor)
    {
        _ = RectCursor.TakeLeft(ref row, WIDTH_SUMMARY_STAT, out var rect);
        var numberRect = rect.TopPartPixels(rect.height * 0.6f);
        var labelRect = rect.BottomPartPixels(rect.height * 0.4f);

        var prevAnchor = Text.Anchor;
        var prevFont = Text.Font;
        var prevColor = GUI.color;

        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleLeft;
        GUI.color = valueColor;
        Widgets.Label(numberRect, count.ToString(CultureInfo.InvariantCulture));
        GUI.color = prevColor;

        Text.Font = GameFont.Tiny;
        Widgets.Label(labelRect, label);

        Text.Font = prevFont;
        Text.Anchor = prevAnchor;
    }

    private void DrawTestRow(Listing_Standard listing, MethodInfo test)
    {
        var row = listing.GetRect(HEIGHT_TEST_ROW);
        if (Mouse.IsOver(row))
        {
            Widgets.DrawHighlight(row);
        }

        var status = TestExplorer.GetTestStatus(test);
        var error = TestExplorer.GetTestError(test);

        _ = RectCursor.TakeLeft(ref row, WIDTH_STATUS_ICON, out var iconSlot);
        var iconRect = RectCursor.CenterSquare(iconSlot);
        GUI.DrawTexture(iconRect, StatusStyle.GetIcon(status));
        RectCursor.TakeLeft(ref row, GAP);

        _ = RectCursor.TakeLeft(ref row, WIDTH_TEST_RUN_BTN, out var runSlot);
        var runRect = RectCursor.CenterSquare(runSlot);
        TooltipHandler.TipRegion(runRect, "RimTestRedux.RunTestTooltip".Translate());
        if (Widgets.ButtonImage(runRect, Icons.Run))
        {
            Runner.RunTest(test);
            StatusExplorer.UpdateAllStatusCounts();
            TimeElapsedExplorer.UpdateAssemblyTimeElapsed(test.DeclaringType!.Assembly);
        }
        var prevFont = Text.Font;
        var prevAnchor = Text.Anchor;
        RectCursor.TakeLeft(ref row, GAP);

        var expandRect = row.RightPartPixels(WIDTH_EXPAND_BTN);
        row = row.LeftPartPixels(row.width - WIDTH_EXPAND_BTN - GAP);
        var timeRect = row.RightPartPixels(WIDTH_TEST_TIME);
        row = row.LeftPartPixels(row.width - WIDTH_TEST_TIME - GAP);

        Rect nameRect;
        Rect messageRect = default;
        if (error != null)
        {
            _ = RectCursor.TakeLeft(ref row, WIDTH_TEST_NAME, out nameRect);
            RectCursor.TakeLeft(ref row, GAP);
            messageRect = row;
        }
        else
        {
            nameRect = row;
        }

        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(nameRect, test.Name);

        if (error != null)
        {
            var prevColor = GUI.color;
            GUI.color = StatusStyle.GetColor(status);
            Text.Font = GameFont.Tiny;
            Widgets.Label(messageRect, error.ShortMessage());
            Text.Font = prevFont;
            GUI.color = prevColor;
        }

        Text.Anchor = TextAnchor.MiddleRight;
        var elapsed = TimeElapsedExplorer.GetTestTimeElapsed(test);
        Widgets.Label(timeRect, elapsed < 0 ? "--" : $"{elapsed:0.###} ms");

        var isExpanded = expandedTests.Contains(test);
        if (error != null)
        {
            var chevronRect = RectCursor.CenterSquare(expandRect);
            if (
                Widgets.ButtonImage(
                    chevronRect,
                    isExpanded ? Icons.ChevronExpanded : Icons.ChevronCollapsed
                )
            )
            {
                _ = isExpanded ? expandedTests.Remove(test) : expandedTests.Add(test);
            }
        }
        Text.Anchor = prevAnchor;

        if (error != null && isExpanded)
        {
            var detailRect = listing.GetRect(HEIGHT_TEST_DETAIL);
            Text.Font = GameFont.Tiny;

            var innerWidth = detailRect.width - GenUI.ScrollBarWidth - 2;
            var fullText = error.FullText();
            var contentHeight = Mathf.Max(Text.CalcHeight(fullText, innerWidth), detailRect.height);
            var viewRect = new Rect(0f, 0f, innerWidth, contentHeight + 16f);

            var detailScroll = detailScrollPositions.TryGetValue(test, out var scroll)
                ? scroll
                : Vector2.zero;
            Widgets.BeginScrollView(detailRect, ref detailScroll, viewRect);
            var prevColor = GUI.color;
            GUI.color = StatusStyle.GetColor(status);
            Widgets.Label(viewRect, fullText);
            GUI.color = prevColor;
            Widgets.EndScrollView();
            detailScrollPositions[test] = detailScroll;

            Text.Font = prevFont;
        }
    }
}
