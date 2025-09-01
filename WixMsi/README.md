# WiX MSI Installer for K0xWorkbench

This directory contains a WiX 6 installer project that creates an MSI installer for the WPF Application.

> This WiX installer project is fully parameterized so it can be easily adapted to other .NET applications that need an MSI for installation on Windows machines.

## Overview

The WiX installer project uses the WiX 6 Toolset.

- **File Harvesting**: Uses the `Files` element to automatically harvest all files from the published application output
- **Parameterization**: Supports configurable product name, manufacturer, version, file paths, and MSI filename

## Project File Structure

- `Package.wxs` - Main package definition with parameterized properties
- `AppComponents.wxs` - Component group that harvests files using the WiX Toolset's `Files` element
- `eula.rtf` - The End User License Agreement (EULA) used in the installer.
- `eula.txt` - The raw text of the EULA
- `Folders.wxs` - Directory structure definition
- `Package.en-us.wxl` - Localization strings
- `Shortcut.wxs` - Defines Windows shortcuts to be installed and uninstalled
- `WixMsi.wixproj` - MSBuild project file with parameterization
support

## Key Features

The WiX 6 installer implements the following key features:

- **Per-User Installation**: Installs to the user's local application data folder, no admin privileges required
- **Selectable Install Location**: Users can choose the installation directory via WixUI_InstallDir dialog
- **End User License Agreement (EULA)**: Displays license agreement during installation
- **Automatic File Harvesting**: Uses WiX 6 `Files` element to include all published application files
- **Start Menu Shortcuts**: Creates application shortcuts in the user's Start Menu
- **Major Version Upgrades**: Automatic upgrade/replacement of previous versions
- **Downgrade Protection**: Prevents installation of older versions over newer ones
- **Parameterized Build**: Configurable product name, version, manufacturer, and file paths
- **Localization Support**: Supports multiple languages (currently en-US included)
- **Embedded Installation Files**: All files embedded in MSI for single-file distribution

### Per-User Installation

The installer is configured with `Scope="perUser"` which provides several benefits:

- **No Administrator Rights Required**: Users can install the application without elevated privileges
- **User-Specific Installation**: Each user gets their own copy of the application
- **Clean Uninstall**: Complete removal without affecting other users or system files
- **Installation Location**: By default, files are installed to `%LOCALAPPDATA%\{ProductName}`

### Selectable Install Location

The installer uses the WiX 6 `WixUI_InstallDir` dialog set to provide users with the ability to choose their installation directory:

- **Default Location**: `%LOCALAPPDATA%\{ProductName}` (user's local application data folder)
- **Custom Location**: Users can browse and select any accessible directory
- **Directory Creation**: Installer automatically creates the selected directory if it doesn't exist
- **Path Validation**: Ensures the selected path is valid and accessible

### End User License Agreement (EULA)

The installer includes a license agreement step in the installation wizard:

- **RTF Format**: License is displayed from the `eula.rtf` file included in the project
- **Required Acceptance**: Users must accept the license terms before proceeding with installation
- **Customizable**: Replace `eula.rtf` with your own license agreement
- **Localization**: License text can be localized for different languages

### Automatic File Harvesting

The installer uses the WiX 6 `Files` element to automatically harvest all files from the published application directory:

- **Dynamic File Discovery**: Automatically includes all files from the `PublishedFilesPath` without manual enumeration
- **Recursive Directory Support**: Includes files from subdirectories using the `**` wildcard pattern
- **No Manual Maintenance**: New files are automatically included when the application is updated
- **Preserves Directory Structure**: Maintains the original folder structure in the installation

See [Files Element Documentation](https://docs.firegiant.com/wix/schema/wxs/files/) in the WiX Core docs.

### Start Menu Shortcuts

The installer creates Start Menu shortcuts for easy application access:

- **Program Menu Integration**: Creates shortcuts in the user's Programs folder
- **Product-Specific Folder**: Shortcuts are organized under a folder named after the product
- **Clean Uninstall**: Shortcuts and folders are automatically removed during uninstallation
- **Working Directory**: Shortcuts are configured with the correct working directory

### Major Version Upgrades

The installer is configured with WiX 6's modern upgrade strategy using the `UpgradeStrategy="majorUpgrade"` attribute. This ensures:

- **Automatic Upgrades**: When a newer version is installed, it automatically replaces the previous version
- **Single Entry in Add/Remove Programs**: Only one version appears in Windows "Installed Apps" list
- **Preserved Settings**: User settings and configurations are maintained during upgrades
- **Downgrade Protection**: Installing an older version over a newer one is blocked by default

**How it works:**
- The `PackageId` remains constant across all versions (acts as the upgrade family identifier)
- The `PackageVersion` changes with each release
- The `ProductCode` is automatically generated uniquely for each build
- Windows Installer uses the PackageId to detect and upgrade related products

**Version Numbering:**
- Use strict 3-part version numbers (e.g., "1.2.3")
  - The installer expects a strict 3-part `PackageVersion` version number (e.g., "1.2.3").
    Otherwise, Windows Installer will ignore any fourth field (e.g. 1.2.3.4)
    and may not detect upgrades correctly.
- Increment the version number for each release to enable proper upgrade detection

### Parameterized Build System

The installer project is fully parameterized to support different build scenarios and easy customization:

- **Product Information**: Configurable product name, manufacturer, and version
- **File Paths**: Customizable paths for published files and main executable
- **MSI Output**: Configurable MSI filename and output location
- **Build Integration**: Easy integration with CI/CD pipelines and build scripts
- **Default Values**: Sensible defaults provided for all parameters

### Localization Support

The installer is designed to support multiple languages and regions:

- **String Resources**: All user-visible text is externalized to `.wxl` localization files
- **Current Support**: US English (en-US) localization included
- **Extensible Design**: Additional languages can be added by creating new `.wxl` files
- **Culture-Specific Builds**: Can build MSI packages for specific cultures and regions

### Embedded Installation Files

The installer is configured for single-file distribution:

- **Embedded CAB**: All installation files are embedded directly in the MSI using `EmbedCab="true"`
- **No External Dependencies**: MSI contains everything needed for installation
- **Simplified Distribution**: Single MSI file can be distributed without additional files
- **Reduced File Count**: Eliminates the need for separate CAB files or external media

## Parameterization

The installer project supports the following parameters:

| Parameter | Description | Default Value |
|-----------|-------------|---------------|
| `PackageId` | Unique identifier for the package | "bryanknox.K0xWorkbench.250901" |
| `PackageVersion` | 3-part version number (e.g., "1.2.3") | "1.0.0" |
| `ProductName` | Display name of the product | "K0xWorkbench" |
| `Manufacturer` | Company/manufacturer name | "Bryan Knox" |
| `PublishedFilesPath` | Path to published application files | "../local-published/WpfApp" |
| `MainExecutableFileName` | Name of the main executable file in the PublishedFilesPath | "K0xWorkbench.exe" |
| `MsiFileName` | Base name for the generated MSI file (without .msi extension) | "WixMsi" |

## Dev Usage

### Prerequisites

1. Install WiX 6 Toolset
   - `dotnet tool install --global wix --version 6.0.0`
1. Ensure the application has been published to the directory specified by the `PublishedFilesPath` parameter.

### Building the MSI Installer

#### Option 1: Using a PowerShell Build Script (Recommended)

Use the provided PowerShell scripts for an automated build process:

```powershell
# Build & publish app then build MSI
.\scripts\PublishAndBuildMsi.ps1 -Version "1.2.3"

# Build & publish app then build MSI, with custom product information
.\scripts\PublishAndBuildMsi.ps1 -Version "1.0.0" -ProductName "My WPF App" -Manufacturer "My Company"

# Build & publish app then build MSI with custom MSI filename
.\scripts\PublishAndBuildMsi.ps1 -Version "1.2.3" -MsiFileName "MyApp-Setup"

# Build & publish app then build MSI, placing output in c:\build\msi\
.\scripts\PublishAndBuildMsi.ps1 -Version "1.2.3" -MsiOutFolderPath "c:\build\msi"

# Build only the MSI (if app is already published)
.\scripts\BuildWixMsi.ps1 -Version "1.2.3"

# Build only MSI with custom filename
.\scripts\BuildWixMsi.ps1 -Version "1.2.3" -MsiFileName "MyApp-Setup"

# Build only MSI, placing output in .\artifacts\msi\
.\scripts\BuildWixMsi.ps1 -Version "1.2.3" -MsiOutFolderPath ".\artifacts\msi"
```

#### Option 2: Using dotnet build directly

```powershell
dotnet build WixMsi\WixMsi.wixproj `
  --configuration Release `
  -p:Platform=x64 `
  -p:ProductName="K0xWorkbench" `
  -p:Manufacturer="Bryan Knox" `
  -p:PackageVersion="1.2.3" `
  -p:PublishedFilesPath="C:\full\path\to\published\files" `
  -p:MsiFileName="MyApp-Setup"
```

#### Option 3: Using MSBuild

```powershell
msbuild WixMsi\WixMsi.wixproj `
  /p:Configuration=Release `
  /p:Platform=x64 `
  /p:ProductName="K0xWorkbench" `
  /p:Manufacturer="Bryan Knox" `
  /p:PackageVersion="1.2.3" `
  /p:PublishedFilesPath="C:\full\path\to\published\files" `
  /p:MsiFileName="MyApp-Setup"
```

### Output Location

The location where the MSI installer will be created is specified by the `PublishedFilesPath` parameter, which defaults to:
`local-published\msi\en-US` in the local dev scripts.

The WixMsi project's default location for the created MSI installer is:
```
WixMsi\bin\{Platform}\{Configuration}\en-US\{MsiFileName}.msi
```
For example: `WixMsi\bin\x64\Release\en-US\WixMsi.msi`

## GitHub Actions Integration

The parameterized design makes it easy to integrate with CI/CD pipelines.

Example GitHub Actions usage:
```yaml
- name: Build MSI Installer
  run: |
    dotnet build WixMsi/WixMsi.wixproj `
      --configuration Release `
      -p:Platform=x64 `
      -p:ProductName="K0xWorkbench" `
      -p:Manufacturer="Bryan Knox" `
      -p:PackageVersion="${{ github.event.inputs.version }}" `
      -p:PublishedFilesPath="${{ github.workspace }}/published-output" `
      -p:MsiFileName="K0xWorkbench-${{ github.event.inputs.version }}-Setup"
```

See `.github\workflows\vtag-release.yml`

## Development Notes

### Version Handling

- The installer expects a strict 3-part `PackageVersion` version number (e.g., "1.2.3").

- Version is used for MSI package versioning and upgrade logic

### MSI File Naming

- The `MsiFileName` parameter controls the output MSI filename
- Do not include the `.msi` extension - it will be added automatically
- Useful for including version numbers or build identifiers in the filename

### Localization

- Currently supports US English (en-US) localization
- Localization strings are defined in `Package.en-us.wxl`
- Additional languages can be added by creating additional `.wxl` files

## Troubleshooting

### Common Issues

1. **"Published files not found"**
   - Ensure the application has been published first using `PublishWpfApp.ps1`
   - Verify the `PublishedFilesPath` parameter points to the correct location

2. **"WiX toolset not found"**
   - Install WiX 6 Toolset: `dotnet tool install --global wix --version 6.0.0`
   - Ensure WiX is available in your PATH

3. **"Bind path not resolved"**
   - Check that the `PublishedFilesPath` is an absolute path
   - Verify that the path contains the published application files

4. **"Multiple versions appearing in Add/Remove Programs"**
   - Ensure all installers use the same `PackageId` value
   - Verify that `UpgradeStrategy="majorUpgrade"` is set in Package.wxs
   - Check that version numbers are incrementing properly (newer versions should have higher numbers)

5. **"Upgrade not working"**
   - Verify that `PackageVersion` is strictly 3-parts (`1.2.3`).
   - Verify the `PackageVersion` is equal to or higher than the currently installed version.
   - Ensure the `PackageId` matches between versions
   - Check Windows Event Viewer for MSI installation logs if needed

### Debugging

To enable verbose output during build:

```powershell
dotnet build WixMsi\WixMsi.wixproj --verbosity detailed
```

## Additional Resources

- [WiX Toolset Documentation](https://docs.firegiant.com/wix/)

## WiX Toolset and Open Source Maintenance Fee

Building this project uses the WiX Toolset (c) .NET Foundation and contributors, licensed under MS-RL.

If you use the WixToolset in your projects and your project or organizations makes money, then WiX Toolset requires an [Open Source Maintenance Fee](https://opensourcemaintenancefee.org/).

For details see: https://github.com/sponsors/wixtoolset
