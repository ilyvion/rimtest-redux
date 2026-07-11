![RimWorld 1.6](https://img.shields.io/badge/RimWorld-1.6-brightgreen)
![CI](https://github.com/ilyvion/rimtest-redux/actions/workflows/ci.yml/badge.svg)

RimTest Redux is a RimWorld mod that adds a lightweight automated testing framework for other mods. It lets modders write and register test suites that run in-game, with a settings UI to trigger runs and inspect pass/fail/warning/skip status per assembly, test suite, and individual test.

No more manually clicking through the game to check a fix didn't break something else — write a test once, and let RimTest Redux re-run it every time you load a save.

Originally created by LaTrissTitude as RimTest; this is a continuation/fork.

## Features

- **Attribute-based test discovery** — decorate a class with `[TestSuite]` and its methods with `[Test]`; no manual registration required.
- **Setup/teardown hooks** — `[BeforeEach]` and `[AfterEach]` methods run automatically around every test in a suite, with `[AfterEach]` guaranteed to run for cleanup even if the test failed.
- **Exception-expectation tests** — `[ShouldThrow]`, optionally with a specific expected exception type.
- **A fluent assertion API** — `Assert.That(...)`, `Assert.ThatCollection(...)`, and `Assert.ThatFunc(...)`, with readable grammar links (`.Is`, `.Not`, `.Has`, `.Does`, ...).
- **A CI-style test runner window** — per-assembly summary counts, per-suite duration and colored result bar, and a details view listing individual tests with expandable failure/skip reasons.
- **Status filtering and search** — filter by pass/fail/skip/not-run status and free-text (regex) search, at both the suite and individual test level.
- **Dev Mode toolbar shortcut** — jump straight to the test runner from the debug toolbar.
- **Run-at-startup option** — automatically run all registered tests when the game loads, so regressions show up before you even open the runner window.
- **Fully localized** — all UI text goes through RimWorld's translation system.

## Installation

RimTest Redux depends on [Harmony](https://steamcommunity.com/workshop/filedetails/?id=2009463077) and [ilyvion's Laboratory](https://github.com/ilyvion/ilyvion-laboratory/releases/latest). Subscribe to/install both alongside RimTest Redux itself.

As a testing tool, RimTest Redux is meant to be run during development, and most mod authors won't want to force their players to install it too. But if your shipped mod assembly references `RimTestRedux` types (which it will, if it contains `[TestSuite]` classes), that assembly has a hard dependency on `RimTestRedux.dll` being loadable at runtime — without it, .NET fails to load the _entire_ assembly, not just the tests, breaking your mod for anyone who doesn't have RimTest Redux installed. You have two safe options:

- **Declare it as a `modDependencies` entry** in your `About.xml`, same as Harmony, so it's guaranteed to be present. Simple, but means every player has to install a dev tool they'll never use.
- **Keep your tests in a separate assembly** (a second project/csproj producing its own DLL) that only references `RimTestRedux.dll`, and only ship that assembly's `.dll` in your dev/debug builds, not in the copy you publish to players. This is the more common approach, since it keeps the dependency entirely out of the shipped mod.

Either way, don't reference `RimTestRedux` types from your mod's main assembly without accounting for the load-order dependency this creates.

## Writing tests for your mod

1. Add a reference to `RimTestRedux.dll` — ideally from a separate test-only project/assembly, so your shipped mod doesn't take on a hard dependency on RimTest Redux (see [Installation](#installation) above).
2. Write a `static` class decorated with `[TestSuite]`, containing `static void` methods decorated with `[Test]`.
3. Build and load your mod (and test assembly, if separate) as normal. RimTest Redux scans every assembly loaded into the game for `[TestSuite]` classes automatically — there's no registration step, XML def, or interface to implement.

```csharp
using System;
using RimTestRedux;

namespace MyMod.Tests;

[TestSuite]
internal static class InventoryTests
{
    private static Pawn testPawn = null!;

    [BeforeEach]
    public static void SetUp() => testPawn = CreateTestPawn();

    [AfterEach]
    public static void TearDown() => testPawn.Destroy();

    [Test]
    public static void PawnStartsWithNoWeapons() =>
        Assert.ThatCollection(testPawn.equipment.AllEquipmentListForReading).Is.Empty();

    [Test]
    public static void AddingItemIncreasesInventoryCount()
    {
        testPawn.inventory.innerContainer.TryAdd(ThingMaker.MakeThing(ThingDefOf.MealSimple));
        Assert.ThatCollection(testPawn.inventory.innerContainer).Has.Count(1);
    }

    [Test]
    [ShouldThrow(typeof(ArgumentNullException))]
    public static void EquipNullWeaponThrows() => testPawn.equipment.AddEquipment(null!);

    [Test]
    public static void HealthPercentIsWithinExpectedRange() =>
        Assert.That(testPawn.health.summaryHealth.SummaryHealthPercent).Is.BetweenInclusive(0.0, 1.0);

    private static Pawn CreateTestPawn() => /* ... */ null!;
}
```

Rules enforced by the framework: a `[TestSuite]` class must be `static`, and every `[Test]`/`[BeforeEach]`/`[AfterEach]` method must be `static`, return `void`, and take no parameters. A suite may declare at most one `[BeforeEach]` and one `[AfterEach]`. Suites or tests that don't meet these rules are shown with a "skipped" status in the runner rather than crashing the game.

### Assertions

All assertions start with an `Assert.That...` call and end with a check, optionally chained through readable grammar links (`.To`, `.Is`, `.Be`, `.Do`/`.Does`, `.Has`/`.Have`, `.The`, `.Not`) that don't affect behavior except for `.Not`, which negates the following check:

| Entry point                                                  | Checks                                                                                                                                                                                                  |
| ------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Assert.That(IComparable value)`                             | `EqualTo`, `LessThan`, `GreaterThan`, `BetweenInclusive(min, max)`, `BetweenExclusive(min, max)`, `SameValueAs` (value equality), `SameReferenceAs` (reference equality), `Null()`, `True()`, `False()` |
| `Assert.ThatCollection(IEnumerable collection)`              | `Contain(item)`, `Empty()`, `Count(expected)`                                                                                                                                                           |
| `Assert.ThatFunc(Func<dynamic>)` / `Assert.ThatFunc(Action)` | `Throw()`                                                                                                                                                                                               |

```csharp
Assert.That(1 + 1).Is.EqualTo(2);
Assert.That(result).Is.Not.Null();
Assert.ThatCollection(hediffs).Does.Contain(myHediff);
Assert.ThatFunc(() => pawn.Kill(null)).Does.Throw();
```

A failed assertion throws `AssertionException`, which the runner catches and reports as a test failure with the assertion's message.

## Using the test runner

Open the runner from the mod's settings (**Mod Options → RimTest Redux → Open Test Runner**) or, in Dev Mode, via the flask icon on the debug toolbar.

The runner window lists every assembly with `[TestSuite]` classes, showing pass/fail/skipped/not-run counts at a glance. Use the toolbar to run all tests, log results to the game log, collapse/expand assemblies, or search by name (regex supported). Status badges let you filter to just failures, warnings, or skipped tests. Click **Details** on a suite to see its individual tests, each with its own run button, duration, and — for failures or skips — an expandable reason and full exception text.

Two settings in the Mod Options tab control default behavior:

- **Run own tests** — whether RimTest Redux's own self-tests are included in a "Run All".
- **Run at startup** — whether all registered tests run automatically when the game loads.

## Building from source

RimTest Redux is part of a family of mods that share build tooling. To build it, check out these sibling repositories alongside this one (i.e. `../ilyvion.Laboratory` and `../rimworld-utils` relative to this repo):

- [`ilyvion.Laboratory`](https://github.com/ilyvion/ilyvion-laboratory) — shared library dependency.
- [`rimworld-utils`](https://github.com/ilyvion/rimworld-utils) — shared MSBuild props/targets and build scripts.

Then build with the .NET SDK (targets `net481`):

```sh
dotnet build --configuration Release --property RimWorldVersion=1.6 RimTestRedux.sln
```

Or use `.vscode/build.sh`, which builds and copies the mod's files into your RimWorld `Mods` folder (see `.vscode/build_config.sh` for the target path).

## Supported versions

RimWorld 1.6 only. Older RimWorld versions (1.2/1.3, supported by the original RimTest) are no longer supported as of RimTest Redux.

## License

RimTest Redux is licensed under either of

- Apache License, Version 2.0 ([LICENSE.Apache-2.0](LICENSE.Apache-2.0) or <http://www.apache.org/licenses/LICENSE-2.0>)
- MIT license ([LICENSE.MIT](LICENSE.MIT) or <http://opensource.org/licenses/MIT>)

at your option.

### Contribution

Unless you explicitly state otherwise, any contribution intentionally submitted for inclusion in the work by you, as defined in the Apache-2.0 license, shall be dual licensed as above, without any additional terms or conditions.
