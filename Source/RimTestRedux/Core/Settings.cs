using System.Text;
using System.Text.RegularExpressions;
using RimTestRedux.Testing;

namespace RimTestRedux.Core;

/// <summary>
/// Main RimTestRedux UI
/// </summary>
public class Settings : ModSettings
{
    /// <summary>
    /// Decides if the mod run it's own tests or not
    /// </summary>
    private bool _runOwnTests;

    /// <summary>
    /// Decides if the mod run it's own tests or not
    /// </summary>
    public bool RunOwnTests
    {
        get => _runOwnTests;
        set => _runOwnTests = value;
    }

    /// <summary>
    /// Decides if the mod runs all the available non filtered tests at launch or not
    /// </summary>
    private bool _runAtStartup = true;

    /// <summary>
    /// Decides if the mod runs all the available non filtered tests at launch or not
    /// </summary>
    public bool RunAtStartup
    {
        get => _runAtStartup;
        set => _runAtStartup = value;
    }

    private readonly Dictionary<Assembly, bool> assemblyVisibility = [];
    private readonly Dictionary<Type, bool> testSuiteVisibility = [];

    private const int HEIGHT_ROW = 40;
    private const int HEIGHT_CONTROLS = 20;
    private const int WIDTH_ROW_RUN = 40;
    private const int WIDTH_TOGGLE_LABEL = 300;
    private const int WIDTH_TIME_LABEL = 40;
    private const int WIDTH_DETAILS_BTN = 80;
    private const int ERROR_CHAR_LIMIT = 120;
    private const int WIDTH_ERROR_LEVEL = 7;
    private const int WIDTH_LEVEL_INDENT = WIDTH_ERROR_LEVEL;

    private const int WIDTH_CONTROLS_ASM_LABEL = 80;
    private const int WIDTH_CONTROLS_TS_LABEL = WIDTH_CONTROLS_ASM_LABEL;

    private const int WIDTH_CONTROLS_ASM_COLLAPSE = 90;
    private const int WIDTH_CONTROLS_TS_COLLAPSE = WIDTH_CONTROLS_ASM_COLLAPSE;
    private const int WIDTH_CONTROLS_ASM_EXPAND = 90;
    private const int WIDTH_CONTROLS_TS_EXPAND = WIDTH_CONTROLS_ASM_EXPAND;

    private const int WIDTH_CONTROLS_SEARCH_LABEL = 150;
    private const int WIDTH_CONTROLS_VIEW_LABEL = 60;
    private const int WIDTH_CONTROLS_VIEW_CONTROL = 80;
    private const int WIDTH_CONTROLS_SECTION_GAP = 20;
    private const int WIDTH_CONTROLS_CONTROL_GAP = 10;

    private static Color COLOR_FAIL = new(0.808f, 0.216f, 0.549f);
    private static Color COLOR_WARN = new(1f, 0.639f, 0.267f);
    private static Color COLOR_PASS = new(0.706f, 0.933f, 0.251f);
    private static Color COLOR_UNKNOWN = new(0.184f, 0.533f, 0.631f);

    private string searchRegex = @"";

    private static Vector2 ScrollPosition = Vector2.zero;

    /// <summary>
    /// Data exposition for serialization and keeping settings saved between game runs
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _runOwnTests, "RunOwnTests", true);
        Scribe_Values.Look(ref _runAtStartup, "RunAtStartup", true);
    }

    /// <summary>
    /// Draws the RimTestRedux UI main canvas
    /// </summary>
    /// <param name="canvas"></param>
    public void DoWindowContents(Rect canvas)
    {
        StatusExplorer.UpdateAllStatusCounts();
        var options = new Listing_Standard();

        options.Begin(canvas);
        options.CheckboxLabeled(
            "Include RimTest Redux' own test suite",
            ref _runOwnTests,
            "if enabled, RimTestRedux will run its' own test suite as well as any mod test suites."
        );
        options.CheckboxLabeled(
            "Run tests at startup",
            ref _runAtStartup,
            "if enabled, RimTestRedux will run every valid tests at the game startup."
        );
        options.GapLine();
        //if (options.ButtonText("Run everything"))
        //{
        //    Runner.RunAllRegisteredTests();
        //    StatusExplorer.UpdateAllStatusCounts();
        //}
        //if(options.ButtonText("Log all current results to Debug log"))
        //{
        //    Viewer.LogTestsResults();
        //}
        //options.GapLine();
        DrawControls(options);
        options.End();

        var GUIRect = canvas.BottomPartPixels(canvas.height - options.CurHeight);
        var viewRect = GUIRect.LeftPartPixels(GUIRect.width - 16).AtZero();
        viewRect.Set(viewRect.xMin, viewRect.yMin, viewRect.width, CalcTotalHeight()); // update height to accomodate to all the data

        var testlisting = new Listing_Standard();
        //testlisting.BeginScrollView(GUIRect, ref ScrollPosition, ref viewRect);
        Widgets.BeginScrollView(GUIRect, ref ScrollPosition, viewRect, true);
        GUI.BeginGroup(viewRect);
        testlisting.Begin(viewRect);
        DrawTests(testlisting);
        testlisting.End();
        GUI.EndGroup();
        //testlisting.EndScrollView(ref viewRect);
        Widgets.EndScrollView();

        //reset fonts
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;

        //Widgets.CustomButtonText(ref labelRect, "test assembly", Color.black, Color.white, Color.red);

        // RimTestRedux -- displayA  displayB ...
        //                                 Status: ✘ERROR  !WARNING  ➥SKIPPED  ?NOT RAN  ✓PASS
        // Assemblies: |collapse| |expand |         [x] 2    [v] 1                [v] 1    [v] 1
        // TestSuites: |collapse| |expand |         [x] 3    [v] 1    [x] 1       [v] 12   [v] 4
        // Tests:      |run all | |log all|         [x] 10            [x] 4       [v] 23   [v] 16
        // filter regex: |_________________
        // V THIS PART V
        // |run| |time|   ||v [v] Assembly 1|       |
        // |run| |time|   ||^ [x] Assembly 2|       | error msg
        // |run| |time|    ||v [v] TestSuite 2.1|   |
        // |run| |time|    ||^ [x] TestSuite 2.2|   | error msg
        // |run| |time|     ||v [x] Test 2.2.1|     | error msg
        // |run| |time|     ||^ [v] Test 2.2.1|     |
        // ...

        //TODO Status filter , utiliser colonnes + sliders
        //-----          [Skipped]  |  [Passed]
        //[Assemblies]   [x] [100]  |
        //[Test Suites][x][010] |
        //[Tests][x][000] |

        // TODO Statuses symbols
        // ?? pas ouf
        // !! pas ouf

        // TODO control rows size doivent être aussi gros que les checkbox.
    }

    private void DrawControls(Listing_Standard bar)
    {
        var rowAsm = bar.GetRect(HEIGHT_CONTROLS);
        var rowTS = bar.GetRect(HEIGHT_CONTROLS);
        var rowT = bar.GetRect(HEIGHT_CONTROLS);

        // ASM - COLLAPSE / EXPAND - STATUSES
        var asmLabelRect = rowAsm.LeftPartPixels(WIDTH_CONTROLS_ASM_LABEL);
        var asmCollapseRect = rowAsm
            .RightPartPixels(rowAsm.width - asmLabelRect.xMax)
            .LeftPartPixels(WIDTH_CONTROLS_ASM_COLLAPSE);
        var asmExpandRect = rowAsm
            .RightPartPixels(rowAsm.width - asmCollapseRect.xMax)
            .LeftPartPixels(WIDTH_CONTROLS_ASM_EXPAND);
        Widgets.Label(asmLabelRect, "Assemblies");
        if (Widgets.ButtonText(asmCollapseRect, "Collapse"))
        {
            CollapseAllAssemblies();
        }
        if (Widgets.ButtonText(asmExpandRect, "Expand"))
        {
            ExpandAllAssemblies();
        }
        var statusAsmLabelRect = rowAsm
            .RightPartPixels(rowAsm.width - asmExpandRect.xMax - WIDTH_CONTROLS_SECTION_GAP)
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_LABEL);
        var statusAsmControllerRect = rowAsm.RightPartPixels(
            rowAsm.width - statusAsmLabelRect.xMax
        );

        var statusAsmFailRect = statusAsmControllerRect.LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusAsmWarnRect = statusAsmControllerRect
            .RightPartPixels(
                statusAsmControllerRect.width - statusAsmFailRect.width - WIDTH_CONTROLS_CONTROL_GAP
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusAsmSkipRect = statusAsmControllerRect
            .RightPartPixels(
                statusAsmControllerRect.width
                    - (2 * statusAsmWarnRect.width)
                    - (2 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusAsmUnknownRect = statusAsmControllerRect
            .RightPartPixels(
                statusAsmControllerRect.width
                    - (3 * statusAsmSkipRect.width)
                    - (3 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusAsmPassRect = statusAsmControllerRect
            .RightPartPixels(
                statusAsmControllerRect.width
                    - (4 * statusAsmUnknownRect.width)
                    - (4 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);

        Widgets.CheckboxLabeled(
            statusAsmFailRect,
            $"{StatusExplorer.GetAssemblyStatusCount(AssemblyStatus.ERROR)} {AssemblyStatusExtension.StatusSymbol(AssemblyStatus.ERROR)}",
            ref FilteredExplorer.failEnabledAsm
        );

        Widgets.CheckboxLabeled(
            statusAsmWarnRect,
            $"{StatusExplorer.GetAssemblyStatusCount(AssemblyStatus.WARNING)} {AssemblyStatusExtension.StatusSymbol(AssemblyStatus.WARNING)}",
            ref FilteredExplorer.warningEnabledAsm
        );

        Widgets.CheckboxLabeled(
            statusAsmUnknownRect,
            $"{StatusExplorer.GetAssemblyStatusCount(AssemblyStatus.UNKNOWN)} {AssemblyStatusExtension.StatusSymbol(AssemblyStatus.UNKNOWN)}",
            ref FilteredExplorer.unknownEnabledAsm
        );

        Widgets.CheckboxLabeled(
            statusAsmPassRect,
            $"{StatusExplorer.GetAssemblyStatusCount(AssemblyStatus.PASS)} {AssemblyStatusExtension.StatusSymbol(AssemblyStatus.PASS)}",
            ref FilteredExplorer.passEnabledAsm
        );

        // TESTSUITE - COLLAPSE / EXPAND - STATUSES
        var tsLabelRect = rowTS.LeftPartPixels(WIDTH_CONTROLS_TS_LABEL);
        var tsCollapseRect = rowTS
            .RightPartPixels(rowTS.width - tsLabelRect.xMax)
            .LeftPartPixels(WIDTH_CONTROLS_TS_COLLAPSE);
        var tsExpandRect = rowTS
            .RightPartPixels(rowTS.width - tsCollapseRect.xMax)
            .LeftPartPixels(WIDTH_CONTROLS_TS_EXPAND);

        Widgets.Label(tsLabelRect, "Test Suites");
        if (Widgets.ButtonText(tsCollapseRect, "Collapse"))
        {
            CollapseAllTestSuites();
        }
        if (Widgets.ButtonText(tsExpandRect, "Expand"))
        {
            ExpandAllTestSuites();
        }
        var statusTSLabelRect = rowTS
            .RightPartPixels(rowTS.width - tsExpandRect.xMax - WIDTH_CONTROLS_SECTION_GAP)
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_LABEL);
        var statusTSControllerRect = rowTS.RightPartPixels(rowTS.width - statusTSLabelRect.xMax);

        var statusTSFailRect = statusTSControllerRect.LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusTSWarnRect = statusTSControllerRect
            .RightPartPixels(
                statusTSControllerRect.width - statusTSFailRect.width - WIDTH_CONTROLS_CONTROL_GAP
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusTSSkipRect = statusTSControllerRect
            .RightPartPixels(
                statusTSControllerRect.width
                    - (2 * statusTSWarnRect.width)
                    - (2 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusTSUnknownRect = statusTSControllerRect
            .RightPartPixels(
                statusTSControllerRect.width
                    - (3 * statusTSSkipRect.width)
                    - (3 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusTSPassRect = statusTSControllerRect
            .RightPartPixels(
                statusTSControllerRect.width
                    - (4 * statusTSUnknownRect.width)
                    - (4 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);

        Widgets.CheckboxLabeled(
            statusTSFailRect,
            $"{StatusExplorer.GetTestSuiteStatusCount(TestSuiteStatus.ERROR)} {TestSuiteStatusExtension.StatusSymbol(TestSuiteStatus.ERROR)}",
            ref FilteredExplorer.failEnabledTS
        );

        Widgets.CheckboxLabeled(
            statusTSWarnRect,
            $"{StatusExplorer.GetTestSuiteStatusCount(TestSuiteStatus.WARNING)} {TestSuiteStatusExtension.StatusSymbol(TestSuiteStatus.WARNING)}",
            ref FilteredExplorer.warningEnabledTS
        );

        Widgets.CheckboxLabeled(
            statusTSSkipRect,
            $"{StatusExplorer.GetTestSuiteStatusCount(TestSuiteStatus.SKIP)} {TestSuiteStatusExtension.StatusSymbol(TestSuiteStatus.SKIP)}",
            ref FilteredExplorer.skipEnabledTS
        );

        Widgets.CheckboxLabeled(
            statusTSUnknownRect,
            $"{StatusExplorer.GetTestSuiteStatusCount(TestSuiteStatus.UNKNOWN)} {TestSuiteStatusExtension.StatusSymbol(TestSuiteStatus.UNKNOWN)}",
            ref FilteredExplorer.unknownEnabledTS
        );

        Widgets.CheckboxLabeled(
            statusTSPassRect,
            $"{StatusExplorer.GetTestSuiteStatusCount(TestSuiteStatus.PASS)} {TestSuiteStatusExtension.StatusSymbol(TestSuiteStatus.PASS)}",
            ref FilteredExplorer.passEnabledTS
        );

        // TEST - RUN ALL / LOG ALL - STATUSES

        var tLabelRect = rowT.LeftPartPixels(WIDTH_CONTROLS_TS_LABEL);
        var tRunAllRect = rowT.RightPartPixels(rowT.width - tLabelRect.xMax)
            .LeftPartPixels(WIDTH_CONTROLS_TS_COLLAPSE);
        var tLogAllRect = rowT.RightPartPixels(rowT.width - tRunAllRect.xMax)
            .LeftPartPixels(WIDTH_CONTROLS_TS_EXPAND);
        Widgets.Label(tLabelRect, "Tests");
        if (Widgets.ButtonText(tRunAllRect, "Run all"))
        {
            Runner.RunAllRegisteredTests();
            StatusExplorer.UpdateAllStatusCounts();
            TimeElapsedExplorer.UpdateAllAssembliesTimeElapsed();
        }
        if (Widgets.ButtonText(tLogAllRect, "Log all"))
        {
            Viewer.LogTestsResults();
        }

        var statusTLabelRect = rowT.RightPartPixels(
                rowT.width - tLogAllRect.xMax - WIDTH_CONTROLS_SECTION_GAP
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_LABEL);
        var statusTControllerRect = rowT.RightPartPixels(rowT.width - statusTLabelRect.xMax);

        var statusTFailRect = statusTControllerRect.LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusTSkipRect = statusTControllerRect
            .RightPartPixels(
                statusTControllerRect.width
                    - (2 * statusTFailRect.width)
                    - (2 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusTUnknownRect = statusTControllerRect
            .RightPartPixels(
                statusTControllerRect.width
                    - (3 * statusTSkipRect.width)
                    - (3 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);
        var statusTPassRect = statusTControllerRect
            .RightPartPixels(
                statusTControllerRect.width
                    - (4 * statusTUnknownRect.width)
                    - (4 * WIDTH_CONTROLS_CONTROL_GAP)
            )
            .LeftPartPixels(WIDTH_CONTROLS_VIEW_CONTROL);

        Widgets.CheckboxLabeled(
            statusTFailRect,
            $"{StatusExplorer.GetTestStatusCount(TestStatus.ERROR)} {TestStatusExtension.StatusSymbol(TestStatus.ERROR)}",
            ref FilteredExplorer.failEnabledT
        );

        Widgets.CheckboxLabeled(
            statusTSkipRect,
            $"{StatusExplorer.GetTestStatusCount(TestStatus.SKIP)} {TestStatusExtension.StatusSymbol(TestStatus.SKIP)}",
            ref FilteredExplorer.skipEnabledT
        );

        Widgets.CheckboxLabeled(
            statusTUnknownRect,
            $"{StatusExplorer.GetTestStatusCount(TestStatus.UNKNOWN)} {TestStatusExtension.StatusSymbol(TestStatus.UNKNOWN)}",
            ref FilteredExplorer.unknownEnabledT
        );

        Widgets.CheckboxLabeled(
            statusTPassRect,
            $"{StatusExplorer.GetTestStatusCount(TestStatus.PASS)} {TestStatusExtension.StatusSymbol(TestStatus.PASS)}",
            ref FilteredExplorer.passEnabledT
        );

        //// SEARCH
        var rowSearch = bar.GetRect(HEIGHT_CONTROLS);
        var searchLabelRect = rowSearch.LeftPartPixels(WIDTH_CONTROLS_SEARCH_LABEL);
        var searchControlRect = rowSearch.RightPartPixels(rowSearch.width - searchLabelRect.xMax);

        Widgets.Label(searchLabelRect, "Search :");
        try
        {
            var searchRegexTmp = Widgets.TextField(searchControlRect, searchRegex);
            if (searchRegexTmp != searchRegex)
            {
                searchRegex = searchRegexTmp;
                FilteredExplorer.UpdateFilter(new Regex(searchRegex));
            }
        }
        catch
        {
            FilteredExplorer.UpdateFilter(new Regex(@""));
        }
    }

    private int CalcTotalHeight()
    {
        var count = 0;
        foreach (var asm in FilteredExplorer.GetFilteredAssemblies())
        {
            count += 1;
            if (!GetAssemblyVisibility(asm))
            {
                continue;
            }

            foreach (var ts in FilteredExplorer.GetFilteredTestSuites(asm))
            {
                count += 1;
                if (!GetTestSuiteVisibility(ts))
                {
                    continue;
                }

                count += FilteredExplorer.GetFilteredTests(ts).EnumerableCount();
            }
        }
        return count * HEIGHT_ROW;
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

    private void ToggleAllTestSuites(bool toggle)
    {
        foreach (var asm in FilteredExplorer.GetFilteredAssemblies())
        {
            foreach (var ts in FilteredExplorer.GetFilteredTestSuites(asm))
            {
                testSuiteVisibility[ts] = toggle;
            }
        }
    }

    private void CollapseAllTestSuites() => ToggleAllTestSuites(false);

    private void ExpandAllTestSuites() => ToggleAllTestSuites(true);

    private void DrawTests(Listing_Standard listing)
    {
        foreach (var asm in FilteredExplorer.GetFilteredAssemblies())
        {
            DrawAssemblyLine(listing, asm);
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

    private void DrawAssemblyLine(Listing_Standard listing, Assembly asm)
    {
        var row = listing.GetRect(HEIGHT_ROW);
        var runBtnRect = row.LeftPartPixels(WIDTH_ROW_RUN);
        var timeRect = row.RightPartPixels(row.width - runBtnRect.xMax)
            .LeftPartPixels(WIDTH_TIME_LABEL);
        var errorRect = row.RightPartPixels(row.width - (timeRect.xMax + (WIDTH_LEVEL_INDENT * 0)))
            .LeftPartPixels(WIDTH_ERROR_LEVEL);
        var toggleLabelRect = row.RightPartPixels(row.width - errorRect.xMax)
            .LeftPartPixels(WIDTH_TOGGLE_LABEL);
        var messageRect = row.RightPartPixels(row.width - toggleLabelRect.xMax);
        messageRect = messageRect.LeftPartPixels(messageRect.width - WIDTH_DETAILS_BTN);
        var detailsRect = row.RightPartPixels(WIDTH_DETAILS_BTN);

        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;
        // test run button
        if (Widgets.ButtonText(runBtnRect, "run"))
        //    if (Widgets.ButtonText(new Rect(new Vector2(row.xMin, row.yMin + currentRow * HEIGHT_ROW + HEIGHT_OPTIONS), SIZE_RUN_BTN), "run"))
        {
            Runner.RunAssembly(asm);
            TimeElapsedExplorer.UpdateAssemblyTimeElapsed(asm);
        }

        // Todo time data
        var timeElapsed = TimeElapsedExplorer.GetAssemblyTimeElapsed(asm);
        Widgets.Label(timeRect, $"{(timeElapsed == -1 ? "--" : $"{timeElapsed}")} ms");

        // error level tick
        var color = AssemblyExplorer.GetAssemblyStatus(asm) switch
        {
            AssemblyStatus.ERROR => COLOR_FAIL,
            AssemblyStatus.WARNING => COLOR_WARN,
            AssemblyStatus.PASS => COLOR_PASS,
            AssemblyStatus.UNKNOWN => COLOR_UNKNOWN,
            _ => throw new NotImplementedException(),
        };
        Widgets.DrawBoxSolid(errorRect, color);

        // collapse / expand button
        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleLeft;
        var labelbuilder = new StringBuilder();
        _ = labelbuilder.Append(GetAssemblyVisibility(asm) ? "[-] " : "[+] ");
        _ = labelbuilder.Append(
            $"{AssemblyStatusExtension.StatusSymbol(AssemblyExplorer.GetAssemblyStatus(asm))} "
        );
        _ = labelbuilder.Append(asm.GetName().Name);
        if (Widgets.ButtonText(toggleLabelRect, labelbuilder.ToString(), false))
        {
            ToggleAssemblyVisibility(asm);
        }

        // results
        var text =
            AssemblyExplorer.GetAssemblyError(asm) != null
                ? AssemblyExplorer.GetAssemblyError(asm)?.ToString() ?? ""
                : "";
        DrawErrorZone(messageRect, detailsRect, text);

        // next lines: test suites
        if (GetAssemblyVisibility(asm))
        {
            foreach (var ts in FilteredExplorer.GetFilteredTestSuites(asm))
            {
                DrawTestSuiteLine(listing, ts);
            }
        }
    }

    private bool GetTestSuiteVisibility(Type ts)
    {
        if (!testSuiteVisibility.TryGetValue(ts, out var value))
        {
            value = true;
            testSuiteVisibility[ts] = value;
        }

        return value;
    }

    private void ToggleTestSuiteVisibility(Type ts) =>
        testSuiteVisibility[ts] = !GetTestSuiteVisibility(ts);

    private void DrawTestSuiteLine(Listing_Standard listing, Type ts)
    {
        var row = listing.GetRect(HEIGHT_ROW);
        var runBtnRect = row.LeftPartPixels(WIDTH_ROW_RUN);
        var timeRect = row.RightPartPixels(row.width - runBtnRect.xMax)
            .LeftPartPixels(WIDTH_TIME_LABEL);
        var errorRect = row.RightPartPixels(row.width - (timeRect.xMax + (WIDTH_LEVEL_INDENT * 1)))
            .LeftPartPixels(WIDTH_ERROR_LEVEL);
        var toggleLabelRect = row.RightPartPixels(row.width - errorRect.xMax)
            .LeftPartPixels(WIDTH_TOGGLE_LABEL);
        var messageRect = row.RightPartPixels(row.width - toggleLabelRect.xMax);
        messageRect = messageRect.LeftPartPixels(messageRect.width - WIDTH_DETAILS_BTN);
        var detailsRect = row.RightPartPixels(WIDTH_DETAILS_BTN);

        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;
        // test run button
        if (Widgets.ButtonText(runBtnRect, "run"))
        {
            Runner.RunTestSuite(ts);
            TimeElapsedExplorer.UpdateAssemblyTimeElapsed(ts.Assembly);
        }
        // Todo time data
        var timeElapsed = TimeElapsedExplorer.GetTestSuiteTimeElapsed(ts);
        Widgets.Label(timeRect, $"{(timeElapsed == -1 ? "--" : $"{timeElapsed}")} ms");
        var color = TestSuiteExplorer.GetTestSuiteStatus(ts) switch
        {
            TestSuiteStatus.WARNING or TestSuiteStatus.SKIP => COLOR_WARN,
            TestSuiteStatus.ERROR => COLOR_FAIL,
            TestSuiteStatus.PASS => COLOR_PASS,
            TestSuiteStatus.UNKNOWN => COLOR_UNKNOWN,
            _ => throw new NotImplementedException(),
        };
        Widgets.DrawBoxSolid(errorRect, color);

        // collapse / expand button
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleLeft;
        var labelbuilder = new StringBuilder();
        _ = labelbuilder.Append(GetTestSuiteVisibility(ts) ? "[-] " : "[+] ");
        _ = labelbuilder.Append(
            $"{TestSuiteStatusExtension.StatusSymbol(TestSuiteExplorer.GetTestSuiteStatus(ts))} "
        );
        _ = labelbuilder.Append(ts.Name);
        if (Widgets.ButtonText(toggleLabelRect, labelbuilder.ToString(), false))
        {
            ToggleTestSuiteVisibility(ts);
        }

        // results

        var text =
            TestSuiteExplorer.GetTestSuiteError(ts) != null
                ? TestSuiteExplorer.GetTestSuiteError(ts)?.ToString() ?? ""
                : "";
        DrawErrorZone(messageRect, detailsRect, text);

        // next lines: test suites
        if (GetTestSuiteVisibility(ts))
        {
            foreach (var test in FilteredExplorer.GetFilteredTests(ts))
            {
                DrawTestLine(listing, test);
            }
        }
    }

    private static void DrawTestLine(Listing_Standard listing, MethodInfo test)
    {
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;

        var row = listing.GetRect(HEIGHT_ROW);
        var runBtnRect = row.LeftPartPixels(WIDTH_ROW_RUN);
        var timeRect = row.RightPartPixels(row.width - runBtnRect.xMax)
            .LeftPartPixels(WIDTH_TIME_LABEL);
        var errorRect = row.RightPartPixels(row.width - (timeRect.xMax + (WIDTH_LEVEL_INDENT * 2)))
            .LeftPartPixels(WIDTH_ERROR_LEVEL);
        var toggleLabelRect = row.RightPartPixels(row.width - errorRect.xMax)
            .LeftPartPixels(WIDTH_TOGGLE_LABEL);
        var messageRect = row.RightPartPixels(row.width - toggleLabelRect.xMax);
        messageRect = messageRect.LeftPartPixels(messageRect.width - WIDTH_DETAILS_BTN);
        var detailsRect = row.RightPartPixels(WIDTH_DETAILS_BTN);

        // test run button
        if (Widgets.ButtonText(runBtnRect, "run"))
        {
            Runner.RunTest(test);
            TimeElapsedExplorer.UpdateAssemblyTimeElapsed(test.DeclaringType.Assembly);
        }
        // Todo time data
        var timeElapsed = TimeElapsedExplorer.GetTestTimeElapsed(test);
        Widgets.Label(timeRect, $"{(timeElapsed == -1 ? "--" : $"{timeElapsed}")} ms");
        var color = TestExplorer.GetTestStatus(test) switch
        {
            TestStatus.SKIP => COLOR_WARN,
            TestStatus.ERROR => COLOR_FAIL,
            TestStatus.PASS => COLOR_PASS,
            TestStatus.UNKNOWN => COLOR_UNKNOWN,
            _ => throw new NotImplementedException(),
        };
        Widgets.DrawBoxSolid(errorRect, color);

        // collapse / expand button
        Text.Anchor = TextAnchor.MiddleLeft;
        var labelbuilder = new StringBuilder();
        _ = labelbuilder.Append(
            $"{TestStatusExtension.StatusSymbol(TestExplorer.GetTestStatus(test))} "
        );
        _ = labelbuilder.Append(test.Name);
        Widgets.Label(toggleLabelRect, labelbuilder.ToString());

        // results
        var text =
            TestExplorer.GetTestError(test) != null
                ? TestExplorer.GetTestError(test)?.ToString() ?? ""
                : "";
        DrawErrorZone(messageRect, detailsRect, text);
    }

    private static void DrawErrorZone(Rect messageRect, Rect detailsRect, string text)
    {
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleLeft;
        if (text.Length > 0)
        {
            if (text.Length > ERROR_CHAR_LIMIT)
            {
                if (Widgets.ButtonText(detailsRect, "To debug log"))
                {
                    // TODO
                    RimTestReduxMod.Instance.LogError(text);
                }
                text = text[..(ERROR_CHAR_LIMIT - 5)] + "[...]";
            }
            _ = Widgets.TextArea(
                messageRect,
                text.Replace("\n", " ", StringComparison.Ordinal),
                true
            );
        }
    }
}
