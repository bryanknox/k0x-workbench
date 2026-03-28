# MSIX Build Workflow Setup for msix-build-approach-2.yml

Related to: `.github\workflows\msix-build-approach-2.yml`

This document explains how to set up and use the GitHub Actions workflow for building MSIX packages for the K0x Workbench application.

## Prerequisites

### 1. Certificate Setup

Before the workflow can run successfully, you need to ensure the self-signed certificate is properly set up:

1. **Local Certificate Creation** (if not already done):
   ```powershell
   # Navigate to certificate scripts
   cd Msix\cert-self-signed-scripts

   # Create the certificate
   .\CreateMyCertSelfSigned.ps1 -subjectDN "CN=Bryan Knox, OU=me, O=Knoxbits, L=Long Beach, S=CA, C=US" -friendlyName "K0x Workbench Code Signing"

   # Export to PFX (use a secure password)
   .\ExportMyCertToPfx.ps1 -friendlyName "K0x Workbench Code Signing" -pfxPath "..\..\_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx" -password "YourSecurePassword"
   ```

2. **Repository Secret Setup**:
   - Go to your GitHub repository settings
   - Navigate to "Secrets and variables" → "Actions"
   - Add a new repository secret named `CERT_PASSWORD`
   - Set the value to the password you used when creating the PFX file

### 2. Workflow File

The workflow file is located at `.github/workflows/msix-build.yml` and follows **Approach 2** from the MSIX build plan (WAP Project Build).

## Workflow Features

### 🚀 Automatic Triggers

- **Manual dispatch**: Allows manual runs with custom options
- **Not implemented**:
  - **Push to main/develop branches**: Builds packages automatically
  - **Pull requests to main**: Builds and validates packages
  - **Git tags (v*)**: Creates GitHub releases with packages

### ⚙️ Configurable Options

When running manually, you can configure:
- **Build Configuration**: Release (default) or Debug
- **Create Bundle**: Enable/disable MSIX bundle creation

### 📦 Build Process

The workflow follows these steps:

1. **Environment Setup**
   - Checkout repository
   - Setup .NET 9.0
   - Setup MSBuild
   - Cache NuGet packages

2. **Validation**
   - Verify certificate exists
   - Validate project structure

3. **Build Process**
   - Restore NuGet packages
   - Build WPF application
   - Build MSIX package using WAP project

4. **Validation & Testing**
   - Verify build outputs
   - Validate MSIX package integrity
   - Test installation process (dry run)

5. **Artifact Management**
   - Upload MSIX packages
   - Upload test certificates
   - Create GitHub releases (for tags)

## Usage

### Manual Workflow Run

1. Go to your repository on GitHub
2. Navigate to "Actions" tab
3. Select "Build MSIX Package" workflow
4. Click "Run workflow"
5. Choose your options:
   - Branch to run on
   - Build configuration (Release/Debug)
   - Whether to create bundle

### Automatic Builds

**On every push to main/develop:**
```bash
git push origin main
```

**Create a release:**
```bash
git tag -a v1.0.2 -m "Release version 1.0.2"
git push origin v1.0.2
```

### Pull Request Testing

When you create a pull request to main, the workflow will:
- Build the MSIX package
- Validate package integrity
- Test installation process
- Upload artifacts for manual testing

## Outputs

### 📁 Artifacts

Each successful build creates these artifacts:

1. **MSIX Packages** (`msix-packages-{configuration}-{platform}`)
   - Contains `.msix` or `.msixbundle` files
   - Signed and ready for distribution
   - Retention: 30 days

2. **Package Info** (`package-info-{configuration}-{platform}`)
   - JSON file with package details
   - Includes file names and sizes
   - Retention: 7 days

3. **Test Certificate** (`test-certificate-{configuration}`)
   - `.cer` file for testing installations
   - Must be installed before testing packages
   - Retention: 7 days

### 🎁 GitHub Releases

For tagged commits, the workflow automatically creates GitHub releases containing:
- MSIX package/bundle files
- Test certificate
- Installation instructions
- Package details

## Installation Instructions

### For End Users

1. **Download from GitHub Release:**
   - Go to the repository's Releases page
   - Download the latest release
   - Download both the `.cer` certificate file and `.msix`/`.msixbundle` package

2. **Install Certificate:**
   - Double-click the `.cer` file
   - Click "Install Certificate"
   - Choose "Local Machine" and "Next"
   - Select "Place all certificates in the following store"
   - Browse and select "Trusted People"
   - Complete the installation

3. **Install Application:**
   - Double-click the `.msix` or `.msixbundle` file
   - Follow the installation prompts

### For Developers/Testers

1. **Download from Workflow Artifacts:**
   - Go to the Actions tab
   - Select a completed workflow run
   - Download the artifact files

2. **Install using PowerShell:**
   ```powershell
   # Install certificate
   Import-Certificate -FilePath "path\to\certificate.cer" -CertStoreLocation "Cert:\LocalMachine\TrustedPeople"

   # Install package
   Add-AppxPackage -Path "path\to\package.msix"
   ```

## Troubleshooting

### Common Issues

1. **"Certificate not found" error:**
   - Ensure the certificate exists at `_CertIgnore/K0xWorkbenchMsixSelfSigning.pfx`
   - Verify the certificate was created with the correct subject DN

2. **"Access denied" during signing:**
   - Check that `CERT_PASSWORD` secret is set correctly
   - Verify the certificate password is correct

3. **Build failures:**
   - Check that all NuGet packages restore successfully
   - Ensure .NET 9.0 SDK compatibility
   - Review build logs for specific error messages

4. **Package validation failures:**
   - Verify the Package.appxmanifest is valid
   - Check that all required files are included
   - Ensure certificate subject matches manifest Publisher

### Debugging Steps

1. **Check workflow logs:**
   - Go to Actions tab → Select failed run → Review step logs

2. **Download artifacts for local testing:**
   - Even failed runs may produce partial artifacts
   - Test locally to isolate issues

3. **Run locally:**
   - Use the commands from the build plan document
   - Test each phase independently

4. **Validate certificate:**
   ```powershell
   # Check certificate details
   Get-PfxCertificate -FilePath "_CertIgnore\K0xWorkbenchMsixSelfSigning.pfx"
   ```

## Security Considerations

- The self-signed certificate is included in the repository for development/testing
- For production releases, consider using a proper code signing certificate
- The certificate password is stored as a GitHub secret (encrypted)
- Certificate files in artifacts are for testing only

## Customization

### Modifying Build Options

Edit `.github/workflows/msix-build.yml` to customize:

- Target platforms (currently x64)
- Build configurations
- Bundle settings
- Output paths
- Artifact retention periods

### Adding Additional Validations

You can extend the workflow with:
- Security scans
- Performance tests
- Compatibility checks
- Automated deployment to test environments

### CI/CD Integration

The workflow is designed to integrate with:
- Automated testing pipelines
- Deployment workflows
- Release management systems
- Package distribution platforms

## Related Documentation

- [MSIX Build Plan](../Msix/docs/command-line-msix-build-plan.md) - Detailed build approach documentation
- [Certificate Scripts](../Msix/cert-self-signed-scripts/) - Certificate management utilities
- [WAP Project](../Msix/WpfBlazor.Packaging/) - Windows Application Packaging project
