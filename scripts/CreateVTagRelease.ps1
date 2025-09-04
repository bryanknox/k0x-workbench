<#
.SYNOPSIS
    Creates a new v-tag (version tag) in the GitHub repo, which will trigger
    a GitHub Actions workflow to publish a new release of the app and its MSI installer.

.DESCRIPTION
    This script creates an annotated git tag with a specified version number,
    pushes it to the origin repository,
    and then verifies the tag was created successfully by fetching and displaying
    the tag information from the remote.

    The script enforces strict 3-part semantic versioning (e.g., "1.2.3")
    and automatically prefixes the tag with "v".

    Pushing a v-tag to the GitHub repo triggers a GitHub Actions workflow
    that will build and publish a new release of the app and its MSI installer.

.PARAMETER Version
    The version number for the release tag.
    Must be in strict 3-part format (major.minor.patch) using only digits.
    Examples: "1.0.0", "2.15.3", "10.234.5678"

    The script will automatically prefix this with "v" to create the tag name
    (e.g., "1.2.3" becomes "v1.2.3").

.EXAMPLE
    .\CreateVTagRelease.ps1 -Version "1.2.3"
    Creates and pushes tag "v1.2.3" with message "Release version 1.2.3"

.EXAMPLE
    .\CreateVTagRelease.ps1 "2.0.1"
    Creates and pushes tag "v2.0.1" with message "Release version 2.0.1"

.NOTES
    Requires: Git to be installed and available in PATH
    Requires: Current directory to be a git repository
    Requires: Remote origin to be configured

.LINK
    https://git-scm.com/docs/git-tag
    https://semver.org/
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateScript({
        if ($_ -notmatch '^([0-9]+)\.([0-9]+)\.([0-9]+)$') {
            throw "Version must be in format major.minor.patch (e.g., '1.2.3')"
        }
        $major = [int]$Matches[1]
        $minor = [int]$Matches[2]
        $patch = [int]$Matches[3]

        # These are limiations of the Windows Installer and MSI.

        if ($major -lt 0 -or $major -gt 255) {
            throw "Major version must be in range 0-255, got: $major"
        }
        if ($minor -lt 0 -or $minor -gt 255) {
            throw "Minor version must be in range 0-255, got: $minor"
        }
        if ($patch -lt 0 -or $patch -gt 65535) {
            throw "Patch version must be in range 0-65535, got: $patch"
        }
        return $true
    })]
    [string]$Version
)

# Set error action preference to stop on errors for robust error handling
$ErrorActionPreference = 'Stop'

try {
    Write-Host "Creating git tag for version $Version..." -ForegroundColor Green

    # Construct tag name and message
    # Tag name follows the convention: v{major}.{minor}.{patch}
    $tagName = "v$Version"
    $message = "Release version $Version"

    # Create annotated tag locally
    # Using -a flag to create an annotated tag (recommended for releases)
    # Annotated tags store metadata like tagger, date, and message
    Write-Host "Creating tag: $tagName" -ForegroundColor Yellow
    git tag -a $tagName -m $message

    # Check if tag creation was successful
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create git tag"
    }

    # Push the tag to the remote repository
    # This makes the tag available to other developers and for release workflows
    Write-Host "Pushing tag to origin..." -ForegroundColor Yellow
    git push origin $tagName

    # Check if push was successful
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to push git tag to origin"
    }

    Write-Host "Successfully created and pushed tag: $tagName" -ForegroundColor Green

    # Verification: Fetch tags from remote to ensure we have the latest state
    Write-Host "Fetching tags from remote..." -ForegroundColor Yellow
    git fetch --tags

    if ($LASTEXITCODE -ne 0) {
        # Non-critical error - tag was still created successfully
        Write-Warning "Failed to fetch tags from remote, but tag was created successfully"
    }
    else {
        # Display verification information about the created tag
        Write-Host "Verifying tag from remote repository:" -ForegroundColor Cyan
        Write-Host "Tag: $tagName" -ForegroundColor White

        # Retrieve the full annotation message from the tag
        # Using -n99 to get all lines and --format to get just the content
        $tagMessage = git tag -n99 --format="%(contents)" $tagName 2>$null
        if ($LASTEXITCODE -eq 0 -and $tagMessage) {
            Write-Host "Message: $tagMessage" -ForegroundColor White
        }
        else {
            # Fallback to the original message if git command fails
            Write-Host "Message: $message" -ForegroundColor White
        }

        # Get the commit SHA that the tag points to
        # This helps verify the tag is pointing to the expected commit
        $commitHash = git rev-list -n 1 $tagName 2>$null
        if ($LASTEXITCODE -eq 0 -and $commitHash) {
            Write-Host "Commit: $commitHash" -ForegroundColor White
        }
    }
}
catch {
    # Handle any errors that occur during the process
    Write-Error "Error creating tag release: $_"
    exit 1
}
