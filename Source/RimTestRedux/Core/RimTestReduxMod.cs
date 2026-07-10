using RimTestRedux.Core;
using RimTestRedux.Testing;

namespace RimTestRedux;

/// <summary>
/// This is the mod entry point run when every assemblies of loaded mods are now loaded and available.
/// </summary>
/// <remarks>We can run test discovery at this point.</remarks>
public class RimTestReduxMod : IlyvionMod
{
#pragma warning disable CS8618 // Set by constructor
    /// <summary>
    /// Gets the singleton instance of the <see cref="RimTestReduxMod"/> class.
    /// </summary>
    public static RimTestReduxMod Instance { get; private set; }

#pragma warning restore CS8618

    /// <summary>
    /// Gets the package ID of the RimTest Redux mod.
    /// </summary>
    public static string PackageId => Instance!.Content.PackageId;

    /// <inheritdoc/>
    protected override bool HasSettings => true;

    /// <summary>
    /// Gets the settings for the RimTest Redux mod.
    /// </summary>
    public static Settings Settings => Instance.GetSettings<Settings>();

    /// <summary>
    /// Said entry point
    /// </summary>
    public RimTestReduxMod(ModContentPack content)
        : base(content) //our constructor
    {
        // This is kind of stupid, but also kind of correct. Correct wins.
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        Instance = this;

        var harmony = new Harmony(content.PackageId);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Explorer.ExploreAndRegisterAssemblies();
        StatusExplorer.UpdateAllStatusCounts();

        if (Settings.RunAtStartup)
        {
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                Runner.RunAllRegisteredTests();
                StatusExplorer.UpdateAllStatusCounts();
                Viewer.LogTestsResults();
            });
        }
    }

    ///<summary>
    /// Settings UI drawing logic
    ///</summary>
    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Settings.DoWindowContents(inRect);
    }
}

/// <summary>
/// Indicates that a class or struct supports hot swapping at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class HotSwappableAttribute : Attribute { }
