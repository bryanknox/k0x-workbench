<#
.SYNOPSIS
    Deletes a GitHub release and its associated tag from both remote and
    local repositories.

.DESCRIPTION
    This script performs a complete cleanup of a GitHub release by:
    1. Detecting the GitHub repository from local git remote (if -Repo not
       specified)
    2. Deleting the release from GitHub using the GitHub CLI
    3. Deleting the remote tag from the origin repository
    4. Deleting the local tag from the local repository

    This script requires the GitHub CLI (gh) to be installed and
    authenticated, and requires git to be available in the PATH.

.PARAMETER Repo
    The GitHub repository in the format 'username/repo-name' or
    'organization/repo-name'. This must be the full repository identifier as
    used by GitHub. If not specified, the repository will be automatically
    detected from the local git remote 'origin'.

.PARAMETER TagName
    The name of the tag/release to delete. Should include the version prefix
    if used (e.g., 'v1.0.0', '1.2.3-beta', etc.).

.EXAMPLE
    .\Delete-GitHubRelease.ps1 -Repo "bryanknox/wpf-release-play" `
                               -TagName "v1.0.0"

    Deletes the v1.0.0 release and tag from the bryanknox/wpf-release-play
    repository.

.EXAMPLE
    .\Delete-GitHubRelease.ps1 -TagName "v1.0.0"

    Deletes the v1.0.0 release and tag from the repository detected from the
    local git remote 'origin'.

.EXAMPLE
    .\Delete-GitHubRelease.ps1 -Repo "myorg/myproject" `
                               -TagName "2.1.0-preview"

    Deletes the 2.1.0-preview release and tag from the myorg/myproject
    repository.

.NOTES
    Prerequisites:
    - GitHub CLI (gh) must be installed and authenticated
    - Git must be available in PATH
    - User must have appropriate permissions to delete releases and push to
      the repository
    - For auto-detection of repository, must be run from within a git
      repository with an 'origin' remote

    This script will fail if:
    - Auto-detection is used but no 'origin' remote is found or the remote
      URL is not a GitHub repository
    - The user doesn't have permission to delete the release

   This script will attempt to delete each of the following, and output
   warnings for any failures:
    - release in the GitHub repo
    - remote tag
    - local tag
#>

param (
    [Parameter(Mandatory = $false,
               HelpMessage = "GitHub repository in format " +
                           "'username/repo-name'. If not specified, will be " +
                           "detected from local git remote.")]
    [string]$Repo,

    [Parameter(Mandatory = $true,
               HelpMessage = "Tag name to delete (e.g., 'v1.0.0')")]
    [string]$TagName
)

$ErrorActionPreference = "Stop"

# Detect repository from local git remote if not specified
if (-not $Repo) {
    Write-Host "🔍 Repository not specified, detecting from local git remote..."
    try {
        $remoteUrl = git remote get-url origin 2>$null
        if ($LASTEXITCODE -ne 0 -or -not $remoteUrl) {
            Write-Host "Repository Detection Failed."
            Write-Host "Could not get remote URL from " `
                       "'git remote get-url origin'."
            Write-Host "Please specify the -Repo parameter or ensure " `
                       "you're in a git repository with an 'origin' remote."
            exit 1
        }

        # Parse GitHub repository from remote URL
        # Handle both HTTPS and SSH formats:
        # HTTPS: https://github.com/username/repo-name.git
        # SSH: git@github.com:username/repo-name.git
        if ($remoteUrl -match 'github\.com[:/]([^/]+)/([^/]+?)(\.git)?$') {
            $Repo = "$($Matches[1])/$($Matches[2])"
            Write-Host "✅ Detected repository: '$Repo'"
        } else {
            Write-Host "Repository Detection Failed"
            Write-Host "Could not parse GitHub repository from remote URL: " `
                       "'$remoteUrl'."
            Write-Host "Please specify the -Repo parameter."
            exit 1
        }
    }
    catch {
        Write-Host "Repository Detection Failed"
        Write-Host "Error detecting repository from git remote: " `
                   "$($_.Exception.Message)"
        exit 1
    }
}

# Validate parameters
Write-Host "🔄 Starting deletion process for release '$TagName' in " `
           "repository '$Repo'..."
Write-Host ""

# Step 1: Delete the release from GitHub
# This removes the release entry from GitHub's releases page
Write-Host "🗑️  Deleting release '$TagName' from '$Repo'..."
gh release delete $TagName --repo $Repo --yes
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Warning: Failed to delete release '$TagName' from '$Repo'."
    Write-Host "   GitHub CLI command failed with exit code $LASTEXITCODE."
}
else {
    Write-Host "✅ Release '$TagName' deleted successfully from '$Repo'."
    $isReleaseDeleted = $true
}

# Step 2: Delete the remote tag from origin
# This removes the tag from the remote repository
Write-Host "🏷️  Deleting remote tag '$TagName'..."
git push origin --delete $TagName
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️ Warning: Failed to delete remote tag '$TagName'."
    Write-Host "   Git command failed with exit code $LASTEXITCODE."
}

# Step 3: Delete the local tag
# This removes the tag from the local repository
Write-Host "📍 Deleting local tag '$TagName'..."
git tag -d $TagName
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Warning: Failed to delete local tag '$TagName'."
    Write-Host "   Git command failed with exit code $LASTEXITCODE."
}
Write-Host ""
