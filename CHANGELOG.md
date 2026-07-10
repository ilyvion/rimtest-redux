# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- A new icon has been added to the developer debug toolbar (visible in Dev Mode). Clicking it opens the test runner directly, without having to go through the mod's settings.
- Tests can now be marked with a `[ShouldThrow]` attribute to declare that they're expected to throw an exception. A specific exception type can optionally be required, e.g. `[ShouldThrow(typeof(ArgumentNullException))]`.

### Changed

- The mod has been renamed from "RimTest" to "RimTest Redux" to reflect that it's now being continued and maintained under new stewardship. Now supports only RimWorld 1.6. Older RimWorld versions (1.2/1.3) are no longer supported.
- The test runner window has been redesigned to look like a modern CI test report. Each assembly now shows an at-a-glance summary (total/passed/failed/skipped/not-run counts) followed by its test suites, each with a duration and a colored result bar; clicking "Details" on a test suite opens a window listing its individual tests, with an expandable row for each failed or skipped test showing the reason. The old toolbar of bare numbers and checkmarks is gone; running tests, viewing the debug log, and collapsing/expanding assemblies are now icon buttons above the results.
- The test runner now opens in its own window instead of being embedded in the Mod Options tab. The Mod Options tab still has the startup checkboxes and an "Open Test Runner" button for those who prefer to launch it from there.

### Fixed

- The test runner's search box is no longer case-sensitive, so searching for a test or suite no longer requires typing its name with exact capitalization.

[Unreleased]: https://github.com/ilyvion/rimtest-redux/compare/v0.1.0...HEAD
