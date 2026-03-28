# MSIX Installer Build Plan - Command Line Tools

This document outlines a detailed plan for building MSIX installers for the K0x Workbench WPF desktop application using command line tools instead of Visual Studio.

## Table of Contents

1. [Project Overview](#project-overview)
2. [Prerequisites](#prerequisites)
3. [Build Approach Options](#build-approach-options)
4. [Phase 1: Certificate Management](#phase-1-certificate-management)
5. [Phase 2: Build the Application](#phase-2-build-the-application)
6. [Phase 3 - Approach 1: Manual Package Creation](#phase-3---approach-1-manual-package-creation)
7. [Phase 3 - Approach 2: Automated Build Using WAP Project](#phase-3---approach-2-automated-build-using-wap-project)
8. [Phase 3 - Approach 3: Complete Automation Script](#phase-3---approach-3-complete-automation-script)
9. [Phase 4: Testing and Deployment](#phase-4-testing-and-deployment)
10. [Phase 5: CI/CD Integration](#phase-5-ci-cd-integration)
11. [Troubleshooting](#troubleshooting)
12. [Best Practices](#best-practices)
13. [References](#references)

## Project Overview

- **Main Project**: `src\WpfBlazor\WpfBlazor.csproj` (Full-trust WPF desktop app)
- **Target Framework**: .NET 9.0 Windows
- **Packaging Project**: `Msix\WpfBlazor.Packaging\WpfBlazor.Packaging.wapproj`
- **Certificate**: Self-signed certificate for code signing
- **Output**: MSIX packages ready for distribution

## Build Approach Options

This document presents **three distinct approaches** for building MSIX packages. Choose the approach that best fits your needs:

### 🔄 Decision Point: Choose Your Build Strategy

**When to decide**: Before starting Phase 3, you must choose between the three approaches below.

| Approach | Complexity | Control Level | Automation | Best For |
|----------|------------|---------------|------------|----------|
| **Manual Package Creation** (Phase 3 - Approach 1) | High | Maximum | Low | Learning, debugging, custom scenarios |
| **WAP Project Build** (Phase 3 - Approach 2) | Low | Medium | Medium | Standard workflows, existing VS setup |
| **Complete Automation** (Phase 3 - Approach 3) | Medium | High | Maximum | CI/CD, production builds |

### 📋 Approach 1: Manual Package Creation (Phase 3)

**Process**: Manually create package layout → Update manifest → Use MakeAppx.exe → Sign with SignTool.exe

**✅ Advantages:**
- **Maximum control** over every step of the packaging process
- **Perfect for learning** how MSIX packaging works under the hood
- **Excellent for debugging** packaging issues
- **Customizable** - can modify any aspect of the process
- **No dependency** on Visual Studio tools or WAP projects

**❌ Disadvantages:**
- **Time-consuming** and error-prone for regular builds
- **Manual token replacement** required in manifest
- **More complex** setup and maintenance
- **Requires deep understanding** of MSIX internals
- **Not suitable** for automated CI/CD pipelines

**Use this approach when:**
- Learning MSIX packaging concepts
- Debugging packaging issues
- Need custom packaging logic
- Working without Visual Studio ecosystem

### 📋 Approach 2: WAP Project Build (Phase 3)

**Process**: Use MSBuild with existing WAP project → Automatic packaging → Built-in signing

**✅ Advantages:**
- **Leverages existing setup** (WAP project already configured)
- **Automatic token replacement** in manifest
- **Integrated signing** process
- **Battle-tested** by Microsoft tooling
- **Good balance** of control and automation
- **Familiar** to Visual Studio users

**❌ Disadvantages:**
- **Less control** over individual packaging steps
- **Dependency** on WAP project configuration
- **Limited customization** without modifying project files
- **Requires understanding** of MSBuild properties
- **May inherit issues** from WAP project setup

**Use this approach when:**
- You have an existing WAP project (✅ you do!)
- Want reliable, standard packaging
- Need moderate automation
- Working within Visual Studio ecosystem

### 📋 Approach 3: Complete Automation (Phase 3)

**Process**: PowerShell script → Build app → Create package → Sign → Validate

**✅ Advantages:**
- **Fully automated** end-to-end process
- **Perfect for CI/CD** pipelines
- **Consistent builds** every time
- **Parameterized** for different configurations
- **Error handling** and validation built-in
- **Production-ready** with logging and status reporting

**❌ Disadvantages:**
- **Initial setup complexity** for the automation script
- **Requires PowerShell knowledge** for modifications
- **Less flexibility** for one-off customizations
- **Debugging** requires script knowledge
- **Dependency** on script maintenance

**Use this approach when:**
- Setting up CI/CD pipelines
- Need consistent, repeatable builds
- Want minimal manual intervention
- Building for production environments

### 🎯 Recommended Decision Matrix

| Your Scenario | Recommended Approach | Reason |
|---------------|---------------------|--------|
| **First-time MSIX packaging** | Manual (Phase 3 - Approach 1) | Learn the fundamentals |
| **Existing VS/WAP workflow** | WAP Project (Phase 3 - Approach 2) | Leverage existing setup |
| **Production CI/CD** | Automation (Phase 3 - Approach 3) | Reliability and consistency |
| **Debugging packaging issues** | Manual (Phase 3 - Approach 1) | Maximum visibility |
| **Regular development builds** | WAP Project (Phase 3 - Approach 2) | Good balance |
| **Enterprise deployment** | Automation (Phase 3 - Approach 3) | Scalability and control |

> **💡 Pro Tip**: You can implement multiple approaches! Start with Manual for learning, use WAP for development, and Automation for production.

## Prerequisites

### Required Tools
1. **Windows SDK** (Latest version)
   - Includes `MakeAppx.exe`, `SignTool.exe`, and other packaging tools
   - Download from: https://developer.microsoft.com/windows/downloads/windows-sdk/

2. **.NET SDK** (9.0 or later)
   - Already present in the project

3. **MSBuild** (Included with Visual Studio Build Tools or .NET SDK)

4. **PowerShell** (pwsh.exe) - For automation scripts

### Verify Tool Availability
```powershell
# Check .NET SDK
dotnet --version

# Check MSBuild
msbuild -version

# Check Windows SDK tools (adjust path as needed)
$sdkPath = "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22621.0\x64"
Test-Path "$sdkPath\makeappx.exe"
Test-Path "$sdkPath\signtool.exe"
```

## Phase 1: Certificate Management

The project already has a self-signed certificate (`_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx`) and helper scripts in `Msix\cert-self-signed-scripts\`.

### 1.1 Verify Certificate (if needed)
```powershell
# Navigate to cert scripts directory
Set-Location "c:\_BkGit\bryanknox\k0x-workbench\Msix\cert-self-signed-scripts"

# List existing certificates
.\ListMyCerts.ps1
```

### 1.2 Create New Certificate (if needed)
```powershell
# Create certificate matching the Publisher DN in Package.appxmanifest
.\CreateMyCertSelfSigned.ps1 -subjectDN "CN=Bryan Knox, OU=me, O=Knoxbits, L=Long Beach, S=CA, C=US" -friendlyName "K0x Workbench Code Signing"

# Export to PFX
.\ExportMyCertToPfx.ps1 -friendlyName "K0x Workbench Code Signing" -pfxPath "c:\_BkGit\bryanknox\k0x-workbench\_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx" -password "YourSecurePassword"
```

### 1.3 Install Certificate for Testing
```powershell
# Install certificate to Trusted People store for local testing
.\ImportPfxFileToLocalMachineTrustedPeopleStore.ps1 -pfxPath "c:\_BkGit\bryanknox\k0x-workbench\_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx" -password "YourSecurePassword"
```

## Phase 2: Build the Application

### 2.1 Clean and Restore
```powershell
# Navigate to solution root
Set-Location "c:\_BkGit\bryanknox\k0x-workbench"

# Clean solution
dotnet clean k0x-workbench.sln --configuration Release

# Restore packages
dotnet restore k0x-workbench.sln
```

### 2.2 Build WPF Application
```powershell
# Build the main WPF project in Release mode
dotnet build "src\WpfBlazor\WpfBlazor.csproj" --configuration Release --no-restore

# Alternative: Use MSBuild for more control
msbuild "src\WpfBlazor\WpfBlazor.csproj" /p:Configuration=Release /p:Platform=x64 /p:OutputPath="bin\Release\x64\"
```

### 2.3 Publish Application (Self-Contained)
```powershell
# Publish as self-contained for x64
dotnet publish "src\WpfBlazor\WpfBlazor.csproj" `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    --output "publish\x64" `
    /p:PublishSingleFile=false `
    /p:PublishReadyToRun=true
```

## Phase 3 - Approach 1: Manual Package Creation

> **📌 Build Approach**: This is **Approach 1** - Manual Package Creation
>
> **Best for**: Learning MSIX internals, debugging, maximum control
>
> **Complexity**: High | **Control**: Maximum | **Automation**: Low
>
> **Alternative**: Skip to [Phase 3 - Approach 2](#phase-3---approach-2-automated-build-using-wap-project) for WAP project approach or [Phase 3 - Approach 3](#phase-3---approach-3-complete-automation-script) for full automation

### 3.1 Create Package Layout Directory
```powershell
# Create temporary package layout directory
$packageLayout = "c:\_BkGit\bryanknox\k0x-workbench\temp-package-layout"
New-Item -ItemType Directory -Path $packageLayout -Force

# Copy published application files
Copy-Item -Path "publish\x64\*" -Destination $packageLayout -Recurse -Force

# Copy Package.appxmanifest
Copy-Item -Path "Msix\WpfBlazor.Packaging\Package.appxmanifest" -Destination $packageLayout

# Copy Images folder
Copy-Item -Path "Msix\WpfBlazor.Packaging\Images" -Destination $packageLayout -Recurse -Force
```

### 3.2 Update Manifest Tokens
```powershell
# Update Package.appxmanifest to replace tokens
$manifestPath = "$packageLayout\Package.appxmanifest"
$manifestContent = Get-Content $manifestPath -Raw

# Replace tokens with actual values
$manifestContent = $manifestContent -replace '\$targetnametoken\$', 'K0xWorkbench'
$manifestContent = $manifestContent -replace '\$targetentrypoint\$', 'Windows.FullTrustApplication'

# Save updated manifest
Set-Content -Path $manifestPath -Value $manifestContent
```

### 3.3 Create MSIX Package
```powershell
# Use MakeAppx to create the package
$sdkPath = "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22621.0\x64"
$makeAppxPath = "$sdkPath\makeappx.exe"
$outputMsix = "c:\_BkGit\bryanknox\k0x-workbench\_InstallerIgnore\K0xWorkbench_unsigned.msix"

& $makeAppxPath pack /d $packageLayout /p $outputMsix /nv
```

### 3.4 Sign MSIX Package
```powershell
# Use SignTool to sign the package
$signToolPath = "$sdkPath\signtool.exe"
$pfxPath = "c:\_BkGit\bryanknox\k0x-workbench\_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx"
$signedMsix = "c:\_BkGit\bryanknox\k0x-workbench\_InstallerIgnore\K0xWorkbench_1.0.1.0_x64.msix"

& $signToolPath sign /fd SHA256 /a /f $pfxPath /p "YourSecurePassword" /tr http://timestamp.digicert.com $outputMsix

# Rename to final name
Move-Item $outputMsix $signedMsix
```

## Phase 3 - Approach 2: Automated Build Using WAP Project

> **📌 Build Approach**: This is **Approach 2** - WAP Project Build
>
> **Best for**: Standard workflows, leveraging existing Visual Studio setup
>
> **Complexity**: Low | **Control**: Medium | **Automation**: Medium
>
> **Recommended**: This approach leverages your existing `WpfBlazor.Packaging.wapproj` configuration

### 3.2 Build WAP Project Directly
```powershell
# Navigate to WAP project directory
Set-Location "c:\_BkGit\bryanknox\k0x-workbench\Msix\WpfBlazor.Packaging"

# Build the packaging project (handles everything automatically)
msbuild WpfBlazor.Packaging.wapproj `
    /p:Configuration=Release `
    /p:Platform=x64 `
    /p:AppxBundle=Never `
    /p:UapAppxPackageBuildMode=StoreUpload `
    /p:PackageCertificateKeyFile="c:\_BkGit\bryanknox\k0x-workbench\_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx" `
    /p:PackageCertificatePassword="YourSecurePassword"
```

### 3.3 Build with Bundle Creation
```powershell
# Create MSIX bundle with multiple architectures
msbuild WpfBlazor.Packaging.wapproj `
    /p:Configuration=Release `
    /p:Platform=x64 `
    /p:AppxBundle=Always `
    /p:AppxBundlePlatforms=x64 `
    /p:PackageCertificateKeyFile="c:\_BkGit\bryanknox\k0x-workbench\_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx" `
    /p:PackageCertificatePassword="YourSecurePassword"
```

## Phase 3 - Approach 3: Complete Automation Script

> **📌 Build Approach**: This is **Approach 3** - Complete Automation
>
> **Best for**: CI/CD pipelines, production builds, consistent automation
>
> **Complexity**: Medium | **Control**: High | **Automation**: Maximum
>
> **Production Ready**: Includes error handling, logging, and validation

### 3.4 Comprehensive Build Script
Create `Msix\BuildMsixInstaller.ps1`:

```powershell
#!/usr/bin/env pwsh
# BuildMsixInstaller.ps1 - Complete MSIX build automation

param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64",
    [string]$CertPassword = "",
    [string]$Version = "1.0.1.0",
    [switch]$CreateBundle = $false,
    [switch]$SkipBuild = $false
)

$ErrorActionPreference = "Stop"
$solutionRoot = "c:\_BkGit\bryanknox\k0x-workbench"
Set-Location $solutionRoot

Write-Host "=== K0x Workbench MSIX Builder ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Platform: $Platform" -ForegroundColor Yellow
Write-Host "Version: $Version" -ForegroundColor Yellow

# Validate certificate password
if ([string]::IsNullOrWhiteSpace($CertPassword)) {
    $CertPassword = Read-Host -Prompt "Enter certificate password" -AsSecureString
    $CertPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($CertPassword))
}

# Step 1: Build application (if not skipped)
if (-not $SkipBuild) {
    Write-Host "`n--- Step 1: Building Application ---" -ForegroundColor Green

    # Clean and restore
    dotnet clean k0x-workbench.sln --configuration $Configuration --verbosity minimal
    dotnet restore k0x-workbench.sln --verbosity minimal

    # Build main project
    dotnet build "src\WpfBlazor\WpfBlazor.csproj" --configuration $Configuration --no-restore --verbosity minimal

    Write-Host "Application built successfully." -ForegroundColor Green
} else {
    Write-Host "`n--- Step 1: Skipping Application Build ---" -ForegroundColor Yellow
}

# Step 2: Build MSIX using WAP project
Write-Host "`n--- Step 2: Building MSIX Package ---" -ForegroundColor Green

$wapProject = "Msix\WpfBlazor.Packaging\WpfBlazor.Packaging.wapproj"
$certPath = "_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx"

$msbuildArgs = @(
    $wapProject
    "/p:Configuration=$Configuration"
    "/p:Platform=$Platform"
    "/p:PackageCertificateKeyFile=$certPath"
    "/p:PackageCertificatePassword=$CertPassword"
    "/p:AppxPackageSigningEnabled=true"
    "/verbosity:minimal"
)

if ($CreateBundle) {
    $msbuildArgs += "/p:AppxBundle=Always"
    $msbuildArgs += "/p:AppxBundlePlatforms=$Platform"
    Write-Host "Creating MSIX bundle..." -ForegroundColor Yellow
} else {
    $msbuildArgs += "/p:AppxBundle=Never"
    Write-Host "Creating single MSIX package..." -ForegroundColor Yellow
}

# Execute MSBuild
& msbuild @msbuildArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n=== Build Completed Successfully ===" -ForegroundColor Green

    # Show output location
    $outputDir = "Msix\WpfBlazor.Packaging\AppPackages"
    Write-Host "Output location: $outputDir" -ForegroundColor Cyan

    # List created packages
    $packages = Get-ChildItem -Path $outputDir -Recurse -Include "*.msix", "*.msixbundle" |
                Where-Object { $_.LastWriteTime -gt (Get-Date).AddMinutes(-5) }

    if ($packages) {
        Write-Host "`nCreated packages:" -ForegroundColor Cyan
        $packages | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }
    }
} else {
    Write-Host "`n=== Build Failed ===" -ForegroundColor Red
    exit $LASTEXITCODE
}
```

### 3.5 Quick Build Script
Create `Msix\QuickBuild.ps1`:

```powershell
#!/usr/bin/env pwsh
# QuickBuild.ps1 - Quick MSIX build for testing

param(
    [string]$CertPassword = ""
)

if ([string]::IsNullOrWhiteSpace($CertPassword)) {
    $CertPassword = Read-Host -Prompt "Certificate password" -AsSecureString
    $CertPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($CertPassword))
}

& "c:\_BkGit\bryanknox\k0x-workbench\Msix\BuildMsixInstaller.ps1" -Configuration Debug -CertPassword $CertPassword
```

### 🎯 Build Approach Summary

At this point, you should have chosen and implemented one of the three build approaches:

| ✅ Completed Approach | What You Should Have | Next Steps |
|----------------------|---------------------|------------|
| **Manual Package Creation** | Signed MSIX file in `_InstallerIgnore\` | Proceed to [Phase 4](#phase-4-testing-and-deployment) |
| **WAP Project Build** | MSIX/Bundle in `Msix\WpfBlazor.Packaging\AppPackages\` | Proceed to [Phase 4](#phase-4-testing-and-deployment) |
| **Complete Automation** | `BuildMsixInstaller.ps1` and `QuickBuild.ps1` scripts | Test scripts, then [Phase 5](#phase-5-ci-cd-integration) |

> **💡 Remember**: You can implement multiple approaches for different scenarios (development vs. production)

## Phase 4: Testing and Deployment

### 4.1 Install Package for Testing
```powershell
# Install the package locally for testing
$packagePath = "c:\_BkGit\bryanknox\k0x-workbench\_InstallerIgnore\K0xWorkbench_1.0.1.0_x64.msix"
Add-AppxPackage -Path $packagePath
```

### 4.2 Uninstall Package
```powershell
# Remove package if needed
Get-AppxPackage "*k0xworkbench*" | Remove-AppxPackage
```

### 4.3 Verify Package Contents
```powershell
# Extract and inspect package contents (for debugging)
$extractPath = "temp-package-extract"
& "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22621.0\x64\makeappx.exe" unpack /p $packagePath /d $extractPath /nv
```

## Phase 5: CI-CD Integration

### 5.1 GitHub Actions Example

> **📌 CI/CD Approach**: This example uses **Approach 3** (Complete Automation)
>
> For other approaches, modify the "Build MSIX" step accordingly

```yaml
name: Build MSIX

on:
  push:
    branches: [ main ]
    tags: [ 'v*' ]

jobs:
  build:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Build MSIX (Automation Approach)
      run: |
        $securePassword = ConvertTo-SecureString "${{ secrets.CERT_PASSWORD }}" -AsPlainText -Force
        $password = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))
        .\Msix\BuildMsixInstaller.ps1 -CertPassword $password
      shell: pwsh

    # Alternative: WAP Project Approach
    # - name: Build MSIX (WAP Project Approach)
    #   run: |
    #     $securePassword = ConvertTo-SecureString "${{ secrets.CERT_PASSWORD }}" -AsPlainText -Force
    #     $password = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))
    #     msbuild "Msix\WpfBlazor.Packaging\WpfBlazor.Packaging.wapproj" /p:Configuration=Release /p:Platform=x64 /p:PackageCertificatePassword=$password
    #   shell: pwsh

    - name: Upload Artifacts
      uses: actions/upload-artifact@v4
      with:
        name: msix-packages
        path: Msix/WpfBlazor.Packaging/AppPackages/**/*
```

## Troubleshooting

### Common Issues and Solutions

1. **Certificate Errors**
   - Ensure certificate DN matches Package.appxmanifest Publisher
   - Verify certificate is properly installed
   - Check certificate expiration

2. **Build Errors**
   - Clear bin/obj folders if build fails
   - Ensure all NuGet packages are restored
   - Check .NET SDK version compatibility

3. **Signing Errors**
   - Verify certificate password
   - Ensure timestamp server is accessible
   - Check certificate validity period

4. **Installation Errors**
   - Install certificate to Trusted People store
   - Enable Developer Mode in Windows Settings
   - Check Windows version compatibility

### Useful Commands

```powershell
# Check installed packages
Get-AppxPackage "*k0x*"

# View package information
Get-AppxPackage "*k0x*" | Format-List

# Check certificate store
Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -like "*Bryan Knox*" }

# Validate package
& "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22621.0\x64\makeappx.exe" verify /p "path\to\package.msix"
```

## Best Practices

1. **Version Management**: Update version numbers consistently across all files
2. **Certificate Security**: Store certificates and passwords securely
3. **Testing**: Always test packages on clean machines
4. **Automation**: Use scripts for consistent builds
5. **Documentation**: Keep build logs for troubleshooting

## References

- [MSIX Packaging Documentation](https://docs.microsoft.com/en-us/windows/msix/)
- [Windows Application Packaging Project](https://docs.microsoft.com/en-us/windows/msix/desktop/desktop-to-uwp-packaging-dot-net)
- [Code Signing Best Practices](https://docs.microsoft.com/en-us/windows/msix/package/sign-app-package-using-signtool)
- [MakeAppx.exe Documentation](https://docs.microsoft.com/en-us/windows/msix/package/create-app-package-with-makeappx-tool)
