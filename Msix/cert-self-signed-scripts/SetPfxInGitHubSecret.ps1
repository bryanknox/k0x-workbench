# SetPfxInGitHubSecret.ps1
#
# Set a GitHub secret with the base64 encoded contents of a PFX file.
#
# This script uses the GitHub CLI to set a repository secret containing the
# base64 encoded contents of a PFX certificate file. This is useful for
# storing code signing certificates in GitHub Actions workflows.
#
# Prerequisites:
# - GitHub CLI (gh) must be installed and authenticated
# - The user must have appropriate permissions to manage secrets in the repository
#
# See https://cli.github.com/manual/gh_secret_set

param (
    [Parameter(Mandatory = $true)]
    [string]$pfxFilePath,

    [Parameter(Mandatory = $false)]
    [string]$secretName = "CERT_PFX_CONTENT",

    [Parameter(Mandatory = $false)]
    [string]$repository = $null
)

# Validate that the PFX file exists
if (-not (Test-Path $pfxFilePath)) {
    Write-Error "PFX file not found: $pfxFilePath"
    exit 1
}

# Check if GitHub CLI is installed and authenticated
try {
    $ghAuth = gh auth status 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "GitHub CLI is not authenticated. Please run 'gh auth login' first."
        exit 1
    }
    Write-Host "GitHub CLI authentication verified." -ForegroundColor Green
}
catch {
    Write-Error "GitHub CLI (gh) is not installed or not in PATH. Please install it from https://cli.github.com/"
    exit 1
}

# Read the PFX file and encode it as base64
try {
    Write-Host "Reading PFX file: $pfxFilePath" -ForegroundColor Yellow
    $pfxBytes = [System.IO.File]::ReadAllBytes($pfxFilePath)
    $base64Content = [System.Convert]::ToBase64String($pfxBytes)
    Write-Host "PFX file successfully encoded to base64." -ForegroundColor Green
}
catch {
    Write-Error "Failed to read or encode PFX file: $_"
    exit 1
}

# Set the GitHub secret
try {
    Write-Host "Setting GitHub secret: $secretName" -ForegroundColor Yellow

    if ($repository) {
        # Set secret for specific repository
        $base64Content | gh secret set $secretName --repo $repository
        Write-Host "GitHub secret '$secretName' set successfully in repository '$repository'." -ForegroundColor Green
    } else {
        # Set secret for current repository (based on current directory)
        $base64Content | gh secret set $secretName
        Write-Host "GitHub secret '$secretName' set successfully in current repository." -ForegroundColor Green
    }
}
catch {
    Write-Error "Failed to set GitHub secret: $_"
    exit 1
}

Write-Host ""
Write-Host "Successfully set GitHub secret '$secretName' with base64 encoded PFX content." -ForegroundColor Green
Write-Host "You can now use this secret in your GitHub Actions workflows." -ForegroundColor Cyan
