# k0x-workbench

A Windows desktop application for quick access to context specific tools and information.

- Create a **workbench** for working in a context, like a project.
- Add **tools** to quickly launch apps, run scripts, or open a documents or websites.
- Group tools into **toolkits** organized around tasks, scenarios, or categories.

## Releases and Downloads

For releases, download, and installation information see this repo's [Releases](https://github.com/bryanknox/k0x-workbench/releases) page.

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

## Configuration

The app can be configured via its `appsettings.json` or equivalent environment variables.

Values set in environment variables will override any corresponding value set in the `appsettings.json` file.

### `DataFolderPath` (`K0xWorkbench_DataFolderPath`)

Specifies the absolute folder path where the app's internal data files should be stored.

The default location is the `%USERPROFILE%\.k0xworkbench` folder.

Example: `C:\Users\<YourUsername>\.k0xworkbench`

### `LogsFolderPath` (`K0xWorkbench_LogsFolderPath`)

Specifies the absolute folder path where the app's log files should be stored.

The default location is the `%LOCALAPPDATA%\K0xWorkbench\logs` folder.

Example: `C:\Users\<YourUsername>\AppData\Local\K0xWorkbench\logs`

### `Serilog.MinimumLevel` (`K0xWorkbench_Serilog__MinimumLevel`)

Specifies the minimum logging level for messages that should be written to log files.


## Development

### Tech Stack

- WPF-Blazor hybrid app
  - .NET 10, C#
  - Serilog
  - XUnit
  - FluentAssertions
  - Moq
  - WiX 6 Toolkit - https://docs.firegiant.com/wix/
- GitHub Actions workflows
  - PowerShell 7
  - Pester 5 test framework for PowerShell - https://pester.dev/

### Repo Folder structure

`.github\agents` - GitHub Copilot custom agents.

`.github\workflows` - GitHub Actions workflows.

`.github\workflows\pwsh` - PowerShell scripts used by workflows.

`.github\workflows\pwsh-unit-tests` - Pester tests for Powershell scripts used by workflows.

`docs\` - Docs for developing in this repo.

`docs\guidelines` - Guidelines for devs and agents developing in this repo.

`scripts\` - Scripts development and local dev testing.

`src\` - Source code for the K0xWorkbench app and associated libraries.

`src\WpfBlazor` - The K0xWorkbench WPF-Blazor hybrid app.

`tests` - Test projects.

`WixMsi\` - WiX project for the K0xWorkbench WPF-Blazor hybrid app's installer (MSI).

## Development docs in this repo

- [PowerShell in GitHub Actions Workflows](./docs/pwsh-in-workflows.md)

- [WiX MSI Installer project documentation](./WixMsi/README.md)

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
