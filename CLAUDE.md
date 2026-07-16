# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

K0x Workbench is a Windows-only WPF-Blazor hybrid desktop app (.NET 10) for quick access to context-specific tools. Users define a "bench" (workbench) JSON file containing "kits" (toolkits, nestable) of "tools" (commands to launch apps, scripts, documents, or websites).

## Commands

All .NET commands run from the repo root against `k0x-workbench.sln` (or from a project folder).

```powershell
dotnet build                          # build the solution
dotnet test                           # run all xUnit tests
dotnet test tests/K0x.Workbench.DataStorage.JsonFiles.Tests   # one test project
dotnet test --filter "FullyQualifiedName~BenchJsonFileLoader" # filter tests
```

CI (`.github/workflows/ci-checks.yml`) enforces formatting — run these before committing to avoid CI failures:

```powershell
dotnet format whitespace --verify-no-changes
dotnet format style --verify-no-changes
dotnet format analyzers --verify-no-changes
```

`Directory.Build.props` sets `WarningsAsErrors`, nullable enabled, net10.0 (app targets `net10.0-windows10.0.26100.0`). NuGet versions are centralized in `Directory.Packages.props`.

PowerShell scripts used by GitHub Actions workflows have Pester 5 tests:

```powershell
cd .github/workflows/pwsh-unit-tests
Invoke-Pester -Path .                          # all tests
Invoke-Pester -Path "Some-Function.Tests.ps1"  # one test file
```

Local publish / installer build (outputs to git-ignored `local-published-ignored/`):

```powershell
.\scripts\PublishAndBuildMsi.ps1 -Version "1.0.0"   # publish app + build WiX MSI
```

Releases are triggered by pushing a v-tag (strict 3-part semver), preferably via `.\scripts\CreateVTagRelease.ps1 -Version 1.2.3`. See `docs/release-management.md`.

## Architecture

### Project layering (src/)

Libraries follow an abstractions/implementation split. Interfaces and models live in `*.Abstractions` projects; implementations depend on them. Each implementation project exposes a `ServicesConfigurationExtensions` class for DI registration.

- `K0x.DataStorage.JsonFiles` — generic JSON file load/save (`IJsonFileLoader<T>` / `IJsonFileSaver<T>`).
- `K0x.Workbench.DataStorage.Abstractions` — domain models (`Bench`, `Kit`, `Tool`) and interfaces (`IBenchFileLoader`, `IBenchFileSaver`, `IBenchProvider`, `IBenchFilePathProvider`).
- `K0x.Workbench.DataStorage.JsonFiles` — bench file persistence built on the generic JSON layer.
- `K0x.Workbench.RecentBenches` (+ `.Abstractions`) — tracks recently opened bench files, stored in the app data folder.
- `K0x.Workbench.Files.Abstractions` — `IDataFolderPathProvider` (app data folder, default `%USERPROFILE%\.k0xworkbench`).
- `WpfBlazor` — the app itself. Test projects in `tests/` mirror the library projects (xUnit + FluentAssertions + Moq).

### WpfBlazor app flow

`Program.Main` → `ProgramConfiguration.Setup()` builds a generic host: config from `appsettings.json`, environment variables, and `K0xWorkbench_`-prefixed environment overrides; Serilog logging; registers all services from the libraries above. Command-line arg (bench file path) is stored in the singleton `IBenchFilePathProvider`. WPF `MainWindow` hosts a `BlazorWebView` (WebView2) that shares the app's `IServiceProvider`, rooted at `BlazorApp.razor`. `Pages/Index.razor` ("/") is a route determiner: navigates to `/bench` (BenchPage) if a bench file path is set, otherwise `/recent` (RecentPage). UI components for the bench tree are in `Components/` (`BenchView` → `KitView` → `ToolView`). See `src/WpfBlazor/README.md` for the detailed startup sequence.

### Bench (K0xBench.json) file format

Rules from `.github/instructions/K0xWorkbench.instructions.md` apply to any `*K0xBench.json` file:

- Root: `Bench` object with `Label` and `Kits`. Kits have `Label`, `DefaultWorkingDirectory`, `Tools`, and nested `Kits`. Tools have `Label`, `Command`, `Arguments`, `WorkingDirectory`.
- A Tool's unset `WorkingDirectory` is inherited from the containing Kit's `DefaultWorkingDirectory`; an unset Kit `DefaultWorkingDirectory` is inherited from the nearest ancestor Kit.
- Use forward slashes in paths, omit null/empty Tool properties, and hoist shared `WorkingDirectory` values up to the Kit's `DefaultWorkingDirectory`.

## PowerShell for GitHub Actions workflows

Guidelines in `docs/guidelines/` apply to `.github/workflows/pwsh` (scripts/modules) and `pwsh-unit-tests` (Pester tests):

- One script per `.ps1`; one function per `.psm1` module. Workflow steps use `shell: pwsh`; complex logic goes in the pwsh folder, not inline in YAML.
- **OrExit pattern**: functions/scripts that must fail the workflow step log a `::error` annotation and `exit 1`; name them with an `OrExit` suffix and provide a `-ThrowOnError` switch (throws instead of exiting) for unit testing. See `docs/guidelines/pwsh-orexit-pattern-guidelines.md`.
- Tests mock all external dependencies, and mock `Write-Host` when the code emits `::error`/`::warning`/`::notice` annotations so tests don't create real annotations in CI.
- Wrap long lines at 80 columns using backtick continuation, command on the first line and each argument on its own line.
