#!/usr/bin/env pwsh
<#
.SYNOPSIS
Builds and publishes the WpfBlazor locally.

.DESCRIPTION
This script builds and publishes the WpfBlazor following the same steps as the
GitHub Actions workflow. It takes a version number as input and publishes the app
to a local output directory.

NOTE: The name of the published app is taken from the AssemblyName configured in the project file.
Not the project name.

.PARAMETER Version
The version number in semantic version format (e.g., "1.2.3").

.EXAMPLE
.\PublishWpfApp.ps1 -Version "1.2.3"

.EXAMPLE
.\PublishWpfApp.ps1 "1.0.0"
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version
)

# Set error action preference
$ErrorActionPreference = 'Stop'

# Define constants similar to the workflow
$CONFIGURATION = 'Release'
$WPF_APP_CSPROJ_PATH = 'src/WpfBlazor/WpfBlazor.csproj'
$TARGET_RUNTIME = 'win-x64'
$OUTPUT_BASE_DIR = 'local-published'
$OUTPUT_DIR = Join-Path $OUTPUT_BASE_DIR 'WpfApp'

try {
    Write-Host "🚀 Starting local publish of WpfBlazor v$Version" -ForegroundColor Green

    # Validate that we're in the correct directory (check for solution file)
    $solutionFile = 'k0x-workbench.sln'
    if (-not (Test-Path $solutionFile)) {
        throw "Solution file '$solutionFile' not found. Please run this script from the repository root."
    }

    # Validate that the project file exists
    if (-not (Test-Path $WPF_APP_CSPROJ_PATH)) {
        throw "Project file '$WPF_APP_CSPROJ_PATH' not found."
    }

    # Step 1: Get WPF app names from project (similar to workflow step)
    Write-Host "🔍 Getting WPF app names from project..." -ForegroundColor Yellow

    # Import the module from the workflow directory
    $moduleFile = '.\.github\workflows\pwsh\Get-AssemblyNameOrExit.psm1'
    if (-not (Test-Path $moduleFile)) {
        throw "Required module file '$moduleFile' not found."
    }

    Import-Module $moduleFile -Force
    $assemblyName = Get-AssemblyNameOrExit -ProjectPath $WPF_APP_CSPROJ_PATH
    Write-Host "✅ Extracted WPF app assembly name: $assemblyName" -ForegroundColor Green

    # Generate build number (using current timestamp for local builds)
    $semVersion = $Version
    $semVersionDotZero = "$semVersion.0"

    Write-Host "📋 Build configuration:" -ForegroundColor Cyan
    Write-Host "  - Assembly Name: $assemblyName"
    Write-Host "  - Version: $semVersion"
    Write-Host "  - Assembly Version: $semVersionDotZero"
    Write-Host "  - File Version: $semVersionDotZero"
    Write-Host "  - Configuration: $CONFIGURATION"
    Write-Host "  - Target Runtime: $TARGET_RUNTIME"
    Write-Host "  - Output Directory: $OUTPUT_DIR"

    # Create output directory if it doesn't exist
    Write-Host "📁 Preparing output directory..." -ForegroundColor Yellow
    if (Test-Path $OUTPUT_DIR) {
        Write-Host "  - Cleaning existing output directory: $OUTPUT_DIR"
        Remove-Item $OUTPUT_DIR -Recurse -Force
    }
    New-Item -ItemType Directory -Path $OUTPUT_DIR -Force | Out-Null
    Write-Host "✅ Output directory ready: $OUTPUT_DIR" -ForegroundColor Green

    # Create .gitignore file in the local-published folder
    $gitignoreFile = Join-Path $OUTPUT_BASE_DIR '.gitignore'
    if (-not (Test-Path $gitignoreFile)) {
        Write-Host "📝 Creating .gitignore file..." -ForegroundColor Yellow
        New-Item -ItemType Directory -Path $OUTPUT_BASE_DIR -Force | Out-Null
        '*' | Out-File -FilePath $gitignoreFile -Encoding UTF8
        Write-Host "✅ Created .gitignore file: $gitignoreFile" -ForegroundColor Green
    }

    # Step 2: dotnet publish WPF app artifacts (similar to workflow step)
    Write-Host "📦 Publishing WPF app artifacts..." -ForegroundColor Yellow

    $publishArgs = @(
        'publish'
        $WPF_APP_CSPROJ_PATH
        '--configuration', $CONFIGURATION
        '--self-contained'
        '--runtime', $TARGET_RUNTIME
        '--output', $OUTPUT_DIR
        '--verbosity', 'minimal'
        "-p:Version=$semVersion"
        "-p:AssemblyVersion=$semVersionDotZero"
        "-p:FileVersion=$semVersionDotZero"
        "-p:IncludeSourceRevisionInInformationalVersion=true"
    )

    Write-Host "  - Running: dotnet $($publishArgs -join ' ')" -ForegroundColor Gray
    & dotnet @publishArgs

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE"
    }

    Write-Host "✅ WPF app published successfully!" -ForegroundColor Green

    # Display summary
    Write-Host ""
    Write-Host "🎉 Local publish completed successfully!" -ForegroundColor Green
    Write-Host "📂 Published files location: $OUTPUT_DIR" -ForegroundColor Cyan
    Write-Host "🚀 To run the application: $OUTPUT_DIR\$assemblyName.exe" -ForegroundColor Cyan

    # List the main executable and key files
    $exePath = Join-Path $OUTPUT_DIR "$assemblyName.exe"
    if (Test-Path $exePath) {
        $fileInfo = Get-Item $exePath
        Write-Host "📄 Executable: $($fileInfo.Name) ($([math]::Round($fileInfo.Length / 1MB, 2)) MB)" -ForegroundColor Gray
    }

    # Count total files
    $totalFiles = (Get-ChildItem $OUTPUT_DIR -Recurse -File).Count
    Write-Host "📊 Total files published: $totalFiles" -ForegroundColor Gray

    Write-Host ""
    Write-Host "✨ You can now test the application by running:" -ForegroundColor Yellow
    Write-Host "   $exePath" -ForegroundColor White

}
catch {
    Write-Host ""
    Write-Host "❌ Error during local publish: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Stack trace: $($_.ScriptStackTrace)" -ForegroundColor Red
    exit 1
}
finally {
    # Clean up imported module
    if (Get-Module -Name Get-AssemblyNameOrExit -ErrorAction SilentlyContinue) {
        Remove-Module Get-AssemblyNameOrExit -Force
    }
}
