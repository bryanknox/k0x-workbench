# k0x-workbench

A Windows desktop application for quick access to context specific tools and information.

- Create a **workbench** for working in a context, like a project.
- Add **tools** to quickly launch apps, run scripts, or open a documents or websites.
- Group tools into **toolkits** organized around tasks, scenarios, or categories.

## Example Workbench

- Workbench: **My Project**
  - Toolkit: **Info & Communication**
    - Tool: **OneNote** *(opens the project notebook)*
    - Tool: **Teams Channel** *(opens MS Teams channel)*
  - Toolkit: **Workspace**
    - Tool: **GH Repo** *(opens the project repository)*
    - Tool: **Visual Studio** *(opens the solution file)*
    - Tool: **VS Code** *(opens the workspace folder)*
    - Tool: **Terminal** *(opens workspace in Windows Terminal)*
    - Tool: **Azure Portal** *(opens the project resources)*
  - Toolkit: **Setup Workspace**
    - Tool: **Set GH User Name & Email** *(runs a script)*

## Features

- Launch defined tools from the workbench
- Workbench file: create, open, and save-a-copy
- Edit workbench file in registered JSON editor.
  - Add, edit, remove tools and toolkits.

## Start from Command Line

Start the app from the command. Optional, provide the path to a workbench JSON file.

```shell
k0x-workbench.exe <path-to-workbench.json>
```

## Development

### Tech Stack

- WPF-Blazor hybrid app
  - .NET 9, C#
  - Serilog
  - XUnit
  - FluentAssertions
  - Moq
- GitHub Actions workflows
  - PowerShell 7
  - Pester 5

### Repo Folder structure

`docs\guidelines` - Guidelines for devs and agents developing in this repo.

`scripts\` - Scripts for development and testing.

`src\SampleWpfApp` - Sample WPF app. Just a raw sample doesn't do anything interesting.

`.github\chatmodes` - GitHub Copilot custom chat modes.

`.github\workflows` - GitHub Actions workflows.

`.github\workflows\pwsh` - PowerShell scripts used by workflows.

`.github\workflows\pwsh-unit-tests` - Pester tests for Powershell scripts used by workflows.

### Building

- Build `k0x-workbench.sln` in Visual Studio 2022
- or `cd src\WpfBlazor` and run `dotnet build`

### Testing

- Tests are run automatically in GitHub Actions workflows.

- Manually run tests for the WPF app:
  - In Visual Studio 2022 Test Explorer
  - or, `dotnet test`

- See: [PowerShell Workflow Steps Guidelines](./docs/guidelines/pwsh-workflow-steps-guidelines.md)
  for info about tests for related PowerShell scripts.

### Releasing

See [Release Management](./docs/release-management.md)
