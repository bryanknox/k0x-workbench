#!/usr/bin/env pwsh
<#
.SYNOPSIS
Publishes the K0xWorkbench WPF App and builds the WiX MSI installer in one step.

.DESCRIPTION
This script combines the functionality of PublishWpfApp.ps1 and BuildWixMsi.ps1
to provide a complete build pipeline from source code to MSI installer. It publishes the
WPF application and then builds the MSI installer that includes all published files.

.PARAMETER PackageId
The WiX Package Id. Defaults to "bryanknox.K0xWorkbench.250901".

.PARAMETER Version
The version number in semantic version format (e.g., "1.2.3").

.PARAMETER ProductName
The name of the product to be displayed in the installer. Defaults to "K0xWorkbench".

.PARAMETER Manufacturer
The manufacturer name to be displayed in the installer. Defaults to "Bryan Knox".

.PARAMETER Configuration
The build configuration to use. Defaults to "Release".

.PARAMETER Platform
The target platform for the MSI. Defaults to "x64" (the only platform currently supported).

.PARAMETER SkipPublish
If specified, skips the publishing step and only builds the MSI installer.
Useful when the application has already been published.

.PARAMETER MsiFileName
Optional. The base name for the generated MSI file (without .msi extension). Defaults to "WixMsi".

.PARAMETER MsiOutFolderPath
Optional. Specifies the output folder path for the generated MSI installer files.
Otherwise, the default output path will be used : "WixMsi\bin\$Platform\$Configuration\en-US\"
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
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [Parameter(Mandatory = $false)]
    [ValidateSet("x64")]
    [string]$Platform = "x64",

    [Parameter(Mandatory = $false)]
    [switch]$SkipPublish,

    [Parameter(Mandatory = $false)]
    [string]$MsiFileName = "WixMsi",

    [Parameter(Mandatory = $false)]
    [string]$MsiOutFolderPath
)

# Set error action preference
$ErrorActionPreference = 'Stop'

# Define script paths
$PUBLISH_SCRIPT = Join-Path 'scripts' 'PublishWpfApp.ps1'
$BUILD_MSI_SCRIPT = Join-Path 'scripts' 'BuildWixMsi.ps1'

try {
    Write-Host "🚀 Starting complete build pipeline for $ProductName v$Version" -ForegroundColor Green

    # Validate that we're in the correct directory (check for solution file)
    $solutionFile = 'k0x-workbench.sln'
    if (-not (Test-Path $solutionFile)) {
        throw "Solution file '$solutionFile' not found. Please run this script from the repository root."
    }

    # Validate that required scripts exist
    if (-not (Test-Path $PUBLISH_SCRIPT)) {
        throw "Publish script '$PUBLISH_SCRIPT' not found."
    }

    if (-not (Test-Path $BUILD_MSI_SCRIPT)) {
        throw "Build MSI script '$BUILD_MSI_SCRIPT' not found."
    }

    Write-Host "📋 Pipeline configuration:" -ForegroundColor Cyan
    Write-Host "  - Package Id: $PackageId"
    Write-Host "  - Version: $Version"
    Write-Host "  - Product Name: $ProductName"
    Write-Host "  - Manufacturer: $Manufacturer"
    Write-Host "  - Configuration: $Configuration"
    Write-Host "  - Platform: $Platform"
    Write-Host "  - Skip Publish: $SkipPublish"
    if ($MsiOutFolderPath) {
        Write-Host "  - MSI Out Folder Path: $MsiOutFolderPath"
    }
    if ($MsiFileName) {
        Write-Host "  - MSI File Name: $MsiFileName"
    }

    # Step 1: Publish the WPF application (unless skipped)
    if (-not $SkipPublish) {
        Write-Host ""
        Write-Host "📦 Step 1: Publishing WPF application..." -ForegroundColor Yellow

        & $PUBLISH_SCRIPT -Version $Version

        if ($LASTEXITCODE -ne 0) {
            throw "Application publish failed with exit code $LASTEXITCODE"
        }

        Write-Host "✅ Application published successfully!" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "⏭️  Step 1: Skipping application publish (as requested)" -ForegroundColor Yellow
    }

    # Step 2: Build the MSI installer
    Write-Host ""
    Write-Host "📦 Step 2: Building MSI installer..." -ForegroundColor Yellow

    & $BUILD_MSI_SCRIPT -PackageId $PackageId -Version $Version -ProductName $ProductName -Manufacturer $Manufacturer -Configuration $Configuration -Platform $Platform -MsiFileName $MsiFileName -MsiOutFolderPath $MsiOutFolderPath

    if ($LASTEXITCODE -ne 0) {
        throw "MSI build failed with exit code $LASTEXITCODE"
    }

    Write-Host "✅ MSI installer built successfully!" -ForegroundColor Green

    # Display final summary
    Write-Host ""
    Write-Host "🎉 Complete build pipeline finished successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📂 Output locations:" -ForegroundColor Cyan
    Write-Host "  - Published app: local-published\WpfApp\" -ForegroundColor Gray
    if ($MsiOutFolderPath) {
        Write-Host "  - MSI installer: $MsiOutFolderPath" -ForegroundColor Gray
    } else {
        Write-Host "  - MSI installer: WixMsi\bin\$Platform\$Configuration\en-US\" -ForegroundColor Gray
    }

    # Find and display the MSI file details
    $msiSearchBase = if ($MsiOutFolderPath) { $MsiOutFolderPath } else { "WixMsi\bin\$Platform\$Configuration" }
    $msiPattern = Join-Path $msiSearchBase "en-US\*.msi"
    $msiFiles = Get-ChildItem $msiPattern -ErrorAction SilentlyContinue
    if (-not $msiFiles) {
        $msiPattern = Join-Path $msiSearchBase "*.msi"
        $msiFiles = Get-ChildItem $msiPattern -ErrorAction SilentlyContinue
    }

    if ($msiFiles) {
        $msiFile = $msiFiles[0]
        Write-Host ""
        Write-Host "📄 Final MSI installer: $($msiFile.Name)" -ForegroundColor White
        Write-Host "   Size: $([math]::Round($msiFile.Length / 1MB, 2)) MB" -ForegroundColor Gray
        Write-Host "   Path: $($msiFile.FullName)" -ForegroundColor Gray

        Write-Host ""
        Write-Host "✨ To install the application, run:" -ForegroundColor Yellow
        Write-Host "   msiexec /i `"$($msiFile.FullName)`"" -ForegroundColor White
    }

}
catch {
    Write-Host ""
    Write-Host "❌ Error during build pipeline: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Stack trace: $($_.ScriptStackTrace)" -ForegroundColor Red
    exit 1
}
