namespace RimTestRedux.Core;

/// <summary>
/// RimTestRedux mod settings
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

    private const int HEIGHT_OPEN_TEST_RUNNER_BTN = 32;

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
    /// Draws the RimTestRedux mod options: startup behavior checkboxes and a button opening the
    /// test-runner dashboard (also reachable via the debug toolbar button in Dev Mode).
    /// </summary>
    /// <param name="canvas"></param>
    public void DoWindowContents(Rect canvas)
    {
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

        var buttonRect = options.GetRect(HEIGHT_OPEN_TEST_RUNNER_BTN).LeftPartPixels(200);
        if (Widgets.ButtonText(buttonRect, "Open Test Runner"))
        {
            if (!Find.WindowStack.IsOpen<Window_TestRunner>())
            {
                Find.WindowStack.Add(new Window_TestRunner());
            }
        }
        options.End();
    }
}
