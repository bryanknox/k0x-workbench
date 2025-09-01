#!/usr/bin/env pwsh
<#
.SYNOPSIS
Builds the WiX MSI installer for the K0K0xWorkbench WPF-Blazor hybrid app.

.DESCRIPTION
This script builds the WiX MSI installer using the parameterized project. It accepts
version information and other properties that can be passed to the WiX build process.
The script assumes that the application has already been published and are located in
the directory indicated by the PublishedFilesPath parameter.

.PARAMETER PackageId
The WiX Package Id. Defaults to "bryanknox.K0xWorkbench.250901".

.PARAMETER Version
The version number in semantic version format (e.g., "1.2.3"). This will be converted
to a 4-part version number for the MSI package (e.g., "1.2.3.0").

.PARAMETER ProductName
The name of the product to be displayed in the installer. Defaults to "K0xWorkbench".

.PARAMETER Manufacturer
The manufacturer name to be displayed in the installer. Defaults to "Bryan Knox".

.PARAMETER PublishedFilesPath
The path to the published application files. Defaults to "../local-published/WpfApp".

.PARAMETER Configuration
The build configuration to use. Defaults to "Release".

.PARAMETER Platform
The target platform. Defaults to "x64" (the only platform currently supported).

.PARAMETER MsiFileName
The base name for the generated MSI file (without .msi extension). Defaults to "WixMsi".

.PARAMETER MsiOutFolderPath
Specifies the output folder path for the generated MSI installer files.
Defaults to "local-published\msi"
#>

[CmdletBinding()]
param(

    [Parameter(Mandatory = $false, Position = 0)]
    [string]$PackageId = "bryanknox.K0xWorkbench.250901",

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version,

    [Parameter(Mandatory = $false)]
    [string]$ProductName = "K0xWorkbench",

    [Parameter(Mandatory = $false)]
    [string]$Manufacturer = "Bryan Knox",

    [Parameter(Mandatory = $false)]
    [string]$PublishedFilesPath = "local-published\WpfApp",

    [Parameter(Mandatory = $false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [Parameter(Mandatory = $false)]
    [ValidateSet("x64")]
    [string]$Platform = "x64",

    [Parameter(Mandatory = $false)]
    [string]$MsiFileName = "WixMsi",

    [Parameter(Mandatory = $false)]
    [string]$MsiOutFolderPath = "local-published\msi"
)

# Set error action preference
$ErrorActionPreference = 'Stop'

# Define constants
$WIX_PROJECT_PATH = 'WixMsi\WixMsi.wixproj'

try {
    Write-Host "🚀 Starting WiX MSI build for $ProductName v$Version" -ForegroundColor Green

    # Validate that we're in the correct directory (check for solution file)
    $solutionFile = 'k0x-workbench.sln'
    if (-not (Test-Path $solutionFile)) {
        throw "Solution file '$solutionFile' not found. Please run this script from the repository root."
    }

    # Validate that the WiX project file exists
    if (-not (Test-Path $WIX_PROJECT_PATH)) {
        throw "WiX project file '$WIX_PROJECT_PATH' not found."
    }

    # Resolve the published files path to an absolute path
    if ([System.IO.Path]::IsPathRooted($PublishedFilesPath)) {
        $absolutePublishedFilesPath = $PublishedFilesPath
    } else {
        $absolutePublishedFilesPath = Resolve-Path $PublishedFilesPath -ErrorAction SilentlyContinue
    }

    if (-not $absolutePublishedFilesPath -or -not (Test-Path $absolutePublishedFilesPath)) {
        throw "PublishedFilesPath '$PublishedFilesPath' not found. Please run PublishWpfApp.ps1 first."
    }

    # Resolve MSI Out folder (if provided) to an absolute path and ensure it exists with trailing separator
    $absoluteMsiOutFolderPath = $null
    if ([System.IO.Path]::IsPathRooted($MsiOutFolderPath)) {
        $absoluteMsiOutFolderPath = $MsiOutFolderPath
    } else {
        $resolved = Resolve-Path -Path $MsiOutFolderPath -ErrorAction SilentlyContinue
        $absoluteMsiOutFolderPath = if ($resolved) { $resolved.Path } else { Join-Path (Get-Location) $MsiOutFolderPath }
    }

    # Normalize to full path before checking/creating/clearing
    $absoluteMsiOutFolderPath = [System.IO.Path]::GetFullPath($absoluteMsiOutFolderPath)

    if (Test-Path -Path $absoluteMsiOutFolderPath -PathType Container) {
        # Clear existing content in the MSI output folder (safety: never clear a root drive)
        $root = [System.IO.Path]::GetPathRoot($absoluteMsiOutFolderPath)
        if ($absoluteMsiOutFolderPath -eq $root) {
            throw "Refusing to clear root drive pointed to by MsiOutFolderPath '$absoluteMsiOutFolderPath'."
        }
        Write-Host "🧹 Clearing existing MsiOutFolderPath contents: $absoluteMsiOutFolderPath" -ForegroundColor DarkGray
        Get-ChildItem -Path $absoluteMsiOutFolderPath -Force -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    }
    elseif (Test-Path -Path $absoluteMsiOutFolderPath -PathType Leaf) {
        throw "MsiOutFolderPath '$absoluteMsiOutFolderPath' points to a file, not a directory."
    }
    else {
        New-Item -ItemType Directory -Path $absoluteMsiOutFolderPath -Force | Out-Null
    }

    if ($absoluteMsiOutFolderPath -notmatch '[\\/]$') {
        $absoluteMsiOutFolderPath += [System.IO.Path]::DirectorySeparatorChar
    }


    Write-Host "📋 Build configuration:" -ForegroundColor Cyan
    Write-Host "  - Package Id: $PackageId"
    Write-Host "  - Package Version: $Version"
    Write-Host "  - Product Name: $ProductName"
    Write-Host "  - Manufacturer: $Manufacturer"
    Write-Host "  - Version: $Version"
    Write-Host "  - Configuration: $Configuration"
    Write-Host "  - Platform: $Platform"
    Write-Host "  - Published Files Path: $absolutePublishedFilesPath"
    Write-Host "  - MSI File Name: $MsiFileName"
    Write-Host "  - MSI Out Folder Path: $absoluteMsiOutFolderPath"

    # Validate that published files exist
    if (-not (Test-Path (Join-Path $absolutePublishedFilesPath "*.exe"))) {
        throw "No executable files found in '$absolutePublishedFilesPath'. Please run PublishWpfApp.ps1 first."
    }

    # Build the MSI using dotnet build with MSBuild properties
    Write-Host "📦 Building WiX MSI installer..." -ForegroundColor Yellow

    $buildArgs = @(
        'build'
        $WIX_PROJECT_PATH
        '--configuration', $Configuration
        '--verbosity', 'minimal'
        "-p:PackageId=$PackageId"
        "-p:PackageVersion=$Version"
        "-p:Platform=$Platform"
        "-p:ProductName=$ProductName"
        "-p:Manufacturer=$Manufacturer"
        "-p:PublishedFilesPath=$absolutePublishedFilesPath"
        "-p:MsiFileName=$MsiFileName"
        "-p:OutputPath=$absoluteMsiOutFolderPath"
    )

    Write-Host "  - Running: dotnet $($buildArgs -join ' ')" -ForegroundColor Gray
    & dotnet @buildArgs

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE"
    }

    Write-Host "✅ WiX MSI installer built successfully!" -ForegroundColor Green

    # Find and display the output MSI file
    $msiPattern = Join-Path $absoluteMsiOutFolderPath "en-US\*.msi"
    $msiFiles = Get-ChildItem $msiPattern -ErrorAction SilentlyContinue
    if (-not $msiFiles) {
        $msiPattern = Join-Path $absoluteMsiOutFolderPath "*.msi"
        $msiFiles = Get-ChildItem $msiPattern -ErrorAction SilentlyContinue
    }

    if ($msiFiles) {
        $msiFile = $msiFiles[0]
        Write-Host ""
        Write-Host "🎉 MSI installer created successfully!" -ForegroundColor Green
        Write-Host "📂 MSI file location: $($msiFile.FullName)" -ForegroundColor Cyan

        $fileInfo = Get-Item $msiFile.FullName
        Write-Host "📄 MSI file: $($fileInfo.Name) ($([math]::Round($fileInfo.Length / 1MB, 2)) MB)" -ForegroundColor Gray

        Write-Host ""
        Write-Host "✨ You can now install the application using:" -ForegroundColor Yellow
        Write-Host "   msiexec /i `"$($msiFile.FullName)`"" -ForegroundColor White
    } else {
        Write-Warning "MSI file not found in expected location: $msiPattern"
    }

}
catch {
    Write-Host ""
    Write-Host "❌ Error during WiX MSI build: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Stack trace: $($_.ScriptStackTrace)" -ForegroundColor Red
    exit 1
}
