using System.Reflection.Emit;
using RimTestRedux.Core;

namespace RimTestRedux.Patches;

/// <summary>
/// Adds a button to the debug toolbar (bottom-right, next to the other dev-tool buttons) that
/// opens the RimTestRedux test-runner dashboard.
/// </summary>
[HarmonyPatch(typeof(DebugWindowsOpener), nameof(DebugWindowsOpener.DrawButtons))]
internal static class DebugWindowsOpener_Patch
{
    private static readonly FieldInfo _fieldDebugWindowsOpenerWidgetRow = AccessTools.Field(
        typeof(DebugWindowsOpener),
        "widgetRow"
    );
    private static readonly MethodInfo _methodWidgetRowFinalX_get = AccessTools.PropertyGetter(
        typeof(WidgetRow),
        nameof(WidgetRow.FinalX)
    );
    private static readonly MethodInfo _methodDraw = SymbolExtensions.GetMethodInfo(() =>
        Draw(default!)
    );

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private static IEnumerable<CodeInstruction> Transpiler(
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
        IEnumerable<CodeInstruction> instructions
    )
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Locate the call to WidgetRow.FinalX; we want to place our own button right before it.
        _ = codeMatcher.SearchForward(i =>
            i.opcode == OpCodes.Callvirt
            && i.operand is MethodInfo m
            && m == _methodWidgetRowFinalX_get
        );
        if (!codeMatcher.IsValid)
        {
            Log.Error(
                "Could not patch DebugWindowsOpener.DrawButtons, IL does not match expectations: call to get value of WidgetRow.FinalX was not found."
            );
            return codeMatcher.Instructions();
        }
        _ = codeMatcher.Insert([
            // call patch method (Draw)
            new(OpCodes.Call, _methodDraw),
            // put WidgetRow field back on stack (we "stole" it from the original call to FinalX)
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld, _fieldDebugWindowsOpenerWidgetRow),
        ]);

        return codeMatcher.Instructions();
    }

    private static void Draw(WidgetRow row)
    {
        if (row.ButtonIcon(Icons.Testing, "RimTestRedux.OpenTestRunnerDebugTooltip".Translate()))
        {
            if (!Find.WindowStack.IsOpen<Window_TestRunner>())
            {
                Find.WindowStack.Add(new Window_TestRunner());
            }
        }
    }
}
