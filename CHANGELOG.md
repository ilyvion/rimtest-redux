# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

- Fixed the Harmony field reference used to suppress stacktrace caching, which pointed at a field that no longer exists on the current Harmony version.

## [0.1.0] - 2026-07-11

### Added

- A new icon has been added to the developer debug toolbar (visible in Dev Mode). Clicking it opens the test runner directly, without having to go through the mod's settings.
- Tests can now be marked with a `[ShouldThrow]` attribute to declare that they're expected to throw an exception. A specific exception type can optionally be required, e.g. `[ShouldThrow(typeof(ArgumentNullException))]`.
- Assertions now support checking collections/lists directly, e.g. `Assert.ThatCollection(pawn.health.hediffSet.hediffs).Does.Contain(myHediff)`, `.Is.Empty()`, and `.Has.Count(3)`.
- Test suites can now declare a `[BeforeEach]` method and/or an `[AfterEach]` method, which automatically run immediately before and after every test in that suite. This means shared setup (like building a test `Map` or `Pawn`) no longer has to be copy-pasted into every test, and `[AfterEach]` is guaranteed to run for cleanup even if the test it followed failed.

### Changed

- The mod has been renamed from "RimTest" to "RimTest Redux" to reflect that it's now being continued and maintained under new stewardship. Now supports only RimWorld 1.6. Older RimWorld versions (1.2/1.3) are no longer supported.
- The test runner window has been redesigned to look like a modern CI test report. Each assembly now shows an at-a-glance summary (total/passed/failed/skipped/not-run counts) followed by its test suites, each with a duration and a colored result bar; clicking "Details" on a test suite opens a window listing its individual tests, with an expandable row for each failed or skipped test showing the reason. The old toolbar of bare numbers and checkmarks is gone; running tests, viewing the debug log, and collapsing/expanding assemblies are now icon buttons above the results.
- The test runner now opens in its own window instead of being embedded in the Mod Options tab. The Mod Options tab still has the startup checkboxes and an "Open Test Runner" button for those who prefer to launch it from there.
- All of the mod's text (buttons, tooltips, labels, and test result messages) now goes through RimWorld's translation system instead of being hard-coded in English, so translators can localize it into other languages.
- The `TheSame()` assertion has been renamed to `SameValueAs()` to make clear it checks value equality, and a new `SameReferenceAs()` assertion has been added for checking that two values are literally the same object in memory.

### Fixed

- The test runner's search box is no longer case-sensitive, so searching for a test or suite no longer requires typing its name with exact capitalization.
- An assembly or test suite that hasn't been run yet now correctly shows "--" for its duration instead of "0 ms".
- The test runner window no longer re-scans every assembly, suite, and test's status on every single frame it's open, which was causing needless CPU usage (and stutter with large test suites) the whole time the window was visible. Status counts now only recompute when something that could actually change them happens (running tests, changing the search filter, or toggling a status filter).

[Unreleased]: https://github.com/ilyvion/rimtest-redux/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/ilyvion/rimtest-redux/releases/tag/v0.1.0
