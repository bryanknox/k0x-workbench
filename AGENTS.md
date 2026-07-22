# AGENTS.md

Canonical orientation for AI coding agents working in this repository.
Harness-neutral: GitHub Copilot reads this file directly, Claude Code
imports it from `CLAUDE.md`. Put project facts here once; do not restate
them in a harness entrypoint.

## What this is

K0x Workbench is a Windows-only WPF-Blazor hybrid desktop app (.NET 10)
for quick access to context-specific tools. Users define a "bench"
(workbench) JSON file containing "kits" (toolkits, nestable) of "tools"
(commands that launch apps, scripts, documents, or websites).

## Commands

All .NET commands run from the repo root against `k0x-workbench.slnx`
(or from a project folder).

```powershell
dotnet build                          # build the solution
dotnet test                           # run all xUnit tests
dotnet test tests/K0x.Workbench.DataStorage.JsonFiles.Tests   # one test project
dotnet test --filter "FullyQualifiedName~BenchJsonFileLoader" # filter tests
```

CI (`.github/workflows/ci-checks.yml`) enforces formatting. Run these
before committing to avoid CI failures:

```powershell
dotnet format whitespace --verify-no-changes
dotnet format style --verify-no-changes
dotnet format analyzers --verify-no-changes
```

NuGet lock files (`packages.lock.json`, committed) pin the full restore
graph; CI restores with `--locked-mode`. After changing any package
version or reference, regenerate the lock files and commit them:

```powershell
dotnet restore --force-evaluate                        # solution projects
dotnet restore WixMsi/WixMsi.wixproj --force-evaluate  # WixMsi is not in the solution
```

PowerShell scripts used by GitHub Actions workflows have Pester 5 tests:

```powershell
cd .github/workflows/pwsh-unit-tests
Invoke-Pester -Path .                          # all tests
Invoke-Pester -Path "Some-Function.Tests.ps1"  # one test file
```

Local publish / installer build (outputs to git-ignored
`local-published-ignored/`):

```powershell
.\scripts\PublishAndBuildMsi.ps1 -Version "1.0.0"   # publish app + build WiX MSI
```

Releases are triggered by pushing a v-tag (strict 3-part semver),
preferably via `.\scripts\CreateVTagRelease.ps1 -Version 1.2.3`. See
[docs/release-management.md](docs/release-management.md).

## Build configuration

`Directory.Build.props` sets `WarningsAsErrors`, nullable enabled, and
net10.0 (the app targets `net10.0-windows10.0.26100.0`). NuGet versions
are centralized in `Directory.Packages.props` (Central Package
Management with transitive pinning; per-project `VersionOverride` is
disabled). `RestoreLockedMode` is on whenever `CI=true`.

Dependency updates are automated — see [docs/dependabot.md](docs/dependabot.md).

## Architecture

### Project layering (src/)

Libraries follow an abstractions/implementation split. Interfaces and
models live in `*.Abstractions` projects; implementations depend on
them. Each implementation project exposes a
`ServicesConfigurationExtensions` class for DI registration.

- `K0x.DataStorage.JsonFiles` — generic JSON file load/save
  (`IJsonFileLoader<T>` / `IJsonFileSaver<T>`).
- `K0x.Workbench.DataStorage.Abstractions` — domain models (`Kit`,
  `Tool`) and interfaces (`IBenchFileLoader`, `IBenchFileSaver`,
  `IBenchFilePathProvider`).
- `K0x.Workbench.DataStorage.JsonFiles` — bench file persistence built
  on the generic JSON layer.
- `K0x.Workbench.RecentBenches` (+ `.Abstractions`) — tracks recently
  opened bench files, stored in the app data folder.
- `K0x.Workbench.Files.Abstractions` — `IDataFolderPathProvider` (app
  data folder, default `%USERPROFILE%\.k0xworkbench`).
- `WpfBlazor` — the app itself. Test projects in `tests/` mirror the
  library projects (xUnit + FluentAssertions + Moq).

### WpfBlazor app flow

`Program.Main` calls `ProgramConfiguration.Setup()`, which builds a
generic host: config from `appsettings.json`, environment variables, and
`K0xWorkbench_`-prefixed environment overrides; Serilog logging; and
registration of all services from the libraries above. The command-line
arg (bench file path) is stored in the singleton
`IBenchFilePathProvider`. WPF `MainWindow` hosts a `BlazorWebView`
(WebView2) that shares the app's `IServiceProvider`, rooted at
`BlazorApp.razor`. `Pages/Index.razor` ("/") is a route determiner: it
navigates to `/bench` (BenchPage) if a bench file path is set, otherwise
to `/recent` (RecentPage). UI components for the bench tree live in
`Components/` (`ToolKitView` → `KitView` → `ToolView`). See
[src/WpfBlazor/README.md](src/WpfBlazor/README.md) for the detailed
startup sequence.

## Conventions

### Naming a bench file's root Kit

The JSON root property stays named `Bench` for backward file
compatibility (`BenchJsonFileModel.Bench`, typed `Kit`). In code, name
identifiers for a bench file's root kit `benchKit` / `BenchKit`; use
plain `kit` for generic `Kit` values. Avoid `rootKit` — it reads as
"rootkit".

### Bench (K0xBench.json) file format

Structure, inheritance, preferred tool patterns, and cleanup rules for
any `*K0xBench.json` file are defined in
[.github/instructions/K0xWorkbench.instructions.md](.github/instructions/K0xWorkbench.instructions.md).
Read that file before creating or editing a bench file. It is the only
copy of those rules — do not restate them elsewhere.

### PowerShell for GitHub Actions workflows

Rules for `.github/workflows/pwsh` (scripts and modules) and
`pwsh-unit-tests` (Pester tests) are defined in:

- [docs/guidelines/pwsh-workflow-steps-guidelines.md](docs/guidelines/pwsh-workflow-steps-guidelines.md)
- [docs/guidelines/pwsh-orexit-pattern-guidelines.md](docs/guidelines/pwsh-orexit-pattern-guidelines.md)

Read the relevant file before writing or reviewing workflow PowerShell.
These are the only copies of those rules.

## How agent guidance is organized

Every rule lives in exactly one canonical file; harness-specific files
are thin entrypoints that point at it. When adding guidance, put the
fact in its canonical file and reference it — never paste specifics into
an entrypoint.

| File | Role |
|---|---|
| `AGENTS.md` | this file — canonical project orientation |
| `CLAUDE.md` | Claude Code entrypoint; imports this file |
| `.github/copilot-instructions.md` | Copilot entrypoint; points at this file |
| `.github/instructions/*.instructions.md` | canonical path-scoped rules (Copilot loads by glob) |
| `.claude/rules/*.md` | Claude Code shims for the same path-scoped rules |
| `.claude/skills/*/SKILL.md` | task procedures — **not Claude-only**; see below |
| `.github/agents/*.agent.md`, `.claude/agents/*.md` | expert lenses, one pair per lens |
| `docs/guidelines/*.md` | canonical harness-neutral rule documents |

### Why skills live under `.claude/`

`.claude/skills/` is **not** a Claude Code-only directory. GitHub Copilot
and Cursor both read it as a project skill root, so a skill placed there
is available in all three — including as a `/slash-command`. It is used
here because Claude Code reads *only* that path, while every other
harness reads several. It is the one location all our targets share, not
a statement about which tool this project prefers.

If your harness is not listed below, check its own docs for which skill
roots it reads before assuming these skills are unavailable to you.

| Harness | Reads `.claude/skills/`? |
|---|---|
| Claude Code | yes (only root it reads) |
| GitHub Copilot | yes |
| Cursor | yes |
| Codex | **no** — reads `.agents/skills/` |

The design behind this layout, including the portability facts it rests
on and what to do when a harness is added or dropped, is in
[docs/agent-primitives-design.md](docs/agent-primitives-design.md).
