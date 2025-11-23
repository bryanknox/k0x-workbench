# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade K0x.Workbench.RecentBenches.Abstractions.csproj
4. Upgrade K0x.Workbench.Files.Abstractions.csproj
5. Upgrade K0x.DataStorage.JsonFiles.csproj
6. Upgrade K0x.Workbench.DataStorage.Abstractions.csproj
7. Upgrade K0x.Workbench.RecentBenches.csproj
8. Upgrade K0x.Workbench.DataStorage.JsonFiles.csproj
9. Upgrade K0x.Workbench.RecentBenches.Tests.csproj
10. Upgrade K0x.Workbench.DataStorage.JsonFiles.Tests.csproj
11. Upgrade K0x.DataStorage.JsonFiles.Tests.csproj
12. Upgrade WpfBlazor.csproj
13. Run unit tests to validate upgrade in the projects listed below:
    - K0x.DataStorage.JsonFiles.Tests.csproj
    - K0x.Workbench.DataStorage.JsonFiles.Tests.csproj
    - K0x.Workbench.RecentBenches.Tests.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                         | Current Version | New Version | Description                                   |
|:-----------------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.AspNetCore.Components.Web                  | 9.0.0           | 10.0.0      | Recommended for .NET 10.0                     |
| Microsoft.AspNetCore.Components.WebView.Wpf          | 9.0.21          |             | No .NET 10.0 compatible version available     |
| Microsoft.Extensions.Configuration                   | 9.0.0           | 10.0.0      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Configuration.Binder            | 9.0.0           | 10.0.0      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Configuration.Json              | 9.0.0           | 10.0.0      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.DependencyInjection.Abstractions| 9.0.0           | 10.0.0      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Hosting                         | 9.0.0           | 10.0.0      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Hosting.Abstractions            | 9.0.0           | 10.0.0      | Recommended for .NET 10.0                     |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### K0x.Workbench.RecentBenches.Abstractions.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### K0x.Workbench.Files.Abstractions.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### K0x.DataStorage.JsonFiles.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### K0x.Workbench.DataStorage.Abstractions.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### K0x.Workbench.RecentBenches.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Configuration should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)

#### K0x.Workbench.DataStorage.JsonFiles.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)

#### K0x.Workbench.RecentBenches.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### K0x.Workbench.DataStorage.JsonFiles.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### K0x.DataStorage.JsonFiles.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### WpfBlazor.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0-windows` to `net10.0-windows`

NuGet packages changes:
  - Microsoft.AspNetCore.Components.Web should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.AspNetCore.Components.WebView.Wpf at version `9.0.21` (*no .NET 10.0 compatible version available - will keep current version*)
  - Microsoft.Extensions.Configuration should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Configuration.Binder should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Configuration.Json should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting.Abstractions should be updated from `9.0.0` to `10.0.0` (*recommended for .NET 10.0*)
