# k0x-workbench

A Windows desktop application for quick access to context specific tools and information.

- Create a **workbench** working in a context, like a project.
- Add **tools** to quickly launch apps, run scripts, or open a documents or websites.
- Group tools into **toolkits** organized around scenarios or categories.

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

- .NET 9, C#
- WPF-Blazor hybrid app
- Serilog
- XUnit
- FluentAssertions
- Moq

### Build

- Build `k0x-workbench.sln` in Visual Studio 2022
- or `cd src\WpfBlazor` and run `dotnet build`
