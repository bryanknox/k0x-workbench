<#
.SYNOPSIS
    Copies (vendors) agent skills from upstream repositories into
    .claude/skills/, and records which upstream version was copied.

.DESCRIPTION
    Reads scripts/vendored-skills.json, which lists one or more sources.
    Each source is an upstream git repository plus the names of the skill
    folders to take from it.

    For each selected source the script shallow-clones the requested ref
    (by default the latest release tag), replaces each listed
    .claude/skills/<name>/ folder with the upstream copy, and writes the
    ref and commit it copied from back into the source's "vendored"
    object.

    Re-running at the same ref re-copies anyway, so the script also undoes
    accidental local edits to a vendored skill.

    The script never commits or pushes; review the changes and commit
    them yourself.

    See docs/vendored-skills.md for the workflow.

.PARAMETER Source
    Names of the sources (the "name" field in vendored-skills.json) to
    process. Defaults to all sources.

.PARAMETER Ref
    An upstream tag, branch, or full 40-character commit SHA to copy from.
    Only allowed when exactly one source is selected.
    When omitted, each source's latest release tag is used: the highest
    "<tagPrefix><major.minor.patch>" tag in its repository.

.PARAMETER Check
    Change nothing. Compare each selected source's vendored skill folders
    against its pinned "vendored.commit", and report whether a newer
    release exists. Exits 1 if any vendored folder differs from upstream.

.EXAMPLE
    .\UpdateVendoredSkills.ps1
    Updates every source to its latest release tag.

.EXAMPLE
    .\UpdateVendoredSkills.ps1 -Source conventional-pr-titles -Ref conventional-pr-titles--v2.0.0
    Copies one source's skills from a specific tag.

.EXAMPLE
    .\UpdateVendoredSkills.ps1 -Check
    Verifies the vendored skills match their pinned upstream commits.

.NOTES
    Requires: Git to be installed and available in PATH
    Requires: Network access to each source repository
#>

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string[]]$Source,

    [string]$Ref,

    [switch]$Check
)

# Set error action preference to stop on errors for robust error handling
$ErrorActionPreference = 'Stop'

# Skill folders owned by this repo, not vendored. A source listing one of
# these names fails validation, so a vendor can never overwrite them.
# Add any new repo-owned skill under .claude/skills/ here.
$repoOwnedSkills = @(
    'normalize-bench-file'
)

$repoRoot = Split-Path -Parent $PSScriptRoot
$configPath = Join-Path $PSScriptRoot 'vendored-skills.json'
$skillsRoot = Join-Path $repoRoot '.claude/skills'

function Invoke-Git {
    # Runs git with the given arguments and throws if it fails.
    param([Parameter(Mandatory = $true)][string[]]$Arguments)

    $output = & git @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "git $($Arguments -join ' ') failed with exit code $LASTEXITCODE"
    }
    return $output
}

function Get-SourceSetting {
    # Returns a source's optional setting, or the default when it is absent.
    param($SourceEntry, [string]$Name, [string]$Default)

    $value = $SourceEntry.PSObject.Properties[$Name]?.Value
    if ([string]::IsNullOrWhiteSpace($value)) { return $Default }
    return $value
}

function Get-LatestReleaseTag {
    # Returns the highest "<prefix><major.minor.patch>" tag in the repository.
    param([string]$Repository, [string]$TagPrefix)

    $lines = Invoke-Git @('ls-remote', '--tags', '--refs', $Repository, "$TagPrefix*")
    $releases = foreach ($line in $lines) {
        $tag = ($line -split "`t")[1] -replace '^refs/tags/', ''
        $versionText = $tag.Substring($TagPrefix.Length)
        if ($versionText -match '^\d+\.\d+\.\d+$') {
            [pscustomobject]@{ Tag = $tag; Version = [version]$versionText }
        }
    }

    if (-not $releases) {
        throw "No '$TagPrefix<major.minor.patch>' release tags found in $Repository"
    }
    return ($releases | Sort-Object Version -Descending | Select-Object -First 1).Tag
}

function Get-UpstreamCheckout {
    # Shallow-clones the repository at the ref into a new temp folder.
    # Returns the folder path and the commit SHA that was checked out.
    param([string]$Repository, [string]$CheckoutRef)

    $cloneDir = Join-Path ([IO.Path]::GetTempPath()) "vendored-skills-$([guid]::NewGuid().ToString('N'))"

    if ($CheckoutRef -match '^[0-9a-fA-F]{40}$') {
        # A commit SHA cannot be passed to clone --branch; fetch it directly.
        Invoke-Git @('init', '--quiet', $cloneDir) | Out-Null
        Invoke-Git @('-C', $cloneDir, 'remote', 'add', 'origin', $Repository) | Out-Null
        Invoke-Git @('-C', $cloneDir, 'fetch', '--quiet', '--depth', '1', 'origin', $CheckoutRef) | Out-Null
        Invoke-Git @('-C', $cloneDir, '-c', 'advice.detachedHead=false', 'checkout', '--quiet', 'FETCH_HEAD') | Out-Null
    }
    else {
        Invoke-Git @('-c', 'advice.detachedHead=false', 'clone', '--quiet', '--depth', '1', '--branch', $CheckoutRef, $Repository, $cloneDir) | Out-Null
    }

    $commit = Invoke-Git @('-C', $cloneDir, 'rev-parse', 'HEAD')
    return [pscustomobject]@{ Path = $cloneDir; Commit = "$commit".Trim() }
}

function Get-FolderFileHashes {
    # Maps each file's path (relative to the folder, forward slashes) to its SHA256 hash.
    param([string]$Folder)

    $hashes = @{}
    if (-not (Test-Path -LiteralPath $Folder -PathType Container)) { return $hashes }

    $folderFullPath = (Resolve-Path -LiteralPath $Folder).Path
    foreach ($file in Get-ChildItem -LiteralPath $folderFullPath -Recurse -File -Force) {
        $relativePath = [IO.Path]::GetRelativePath($folderFullPath, $file.FullName) -replace '\\', '/'
        $hashes[$relativePath] = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
    }
    return $hashes
}

function Compare-SkillFolder {
    # Returns a description of each difference between two skill folders.
    param([string]$Expected, [string]$Actual)

    $expectedHashes = Get-FolderFileHashes $Expected
    $actualHashes = Get-FolderFileHashes $Actual

    $differences = @(foreach ($path in $expectedHashes.Keys | Sort-Object) {
            if (-not $actualHashes.ContainsKey($path)) { "missing: $path" }
            elseif ($actualHashes[$path] -ne $expectedHashes[$path]) { "modified: $path" }
        })
    $differences += @(foreach ($path in $actualHashes.Keys | Sort-Object) {
            if (-not $expectedHashes.ContainsKey($path)) { "extra: $path" }
        })
    return $differences
}

function Assert-ValidConfig {
    # Fails before anything is cloned or deleted if the config is unusable.
    param($Config)

    if (-not $Config.sources) {
        throw "$configPath has no sources"
    }

    $sourceNames = @{}
    $skillOwners = @{}
    foreach ($entry in $Config.sources) {
        foreach ($field in 'name', 'repository') {
            if ([string]::IsNullOrWhiteSpace($entry.PSObject.Properties[$field]?.Value)) {
                throw "A source in $configPath is missing '$field'"
            }
        }
        if ($sourceNames.ContainsKey($entry.name)) {
            throw "Source name '$($entry.name)' appears more than once"
        }
        $sourceNames[$entry.name] = $true

        if (-not $entry.skills) {
            throw "Source '$($entry.name)' lists no skills"
        }
        foreach ($skill in $entry.skills) {
            if ($skill -notmatch '^[a-z0-9][a-z0-9-]*$') {
                throw "Source '$($entry.name)' lists an invalid skill folder name: '$skill'"
            }
            if ($repoOwnedSkills -contains $skill) {
                throw "Source '$($entry.name)' lists '$skill', which is a repo-owned skill"
            }
            if ($skillOwners.ContainsKey($skill)) {
                throw "Skill '$skill' is listed by both '$($skillOwners[$skill])' and '$($entry.name)'"
            }
            $skillOwners[$skill] = $entry.name
        }
    }
}

function Save-Config {
    # Writes the config back with 4-space indentation (per .editorconfig) and LF line endings.
    param($Config)

    $json = $Config | ConvertTo-Json -Depth 10
    # ConvertTo-Json indents with 2 spaces; double the leading whitespace of each line.
    $lines = $json -split "\r?\n" | ForEach-Object {
        if ($_ -match '^( +)(.*)$') { (' ' * ($Matches[1].Length * 2)) + $Matches[2] } else { $_ }
    }
    [IO.File]::WriteAllText($configPath, ($lines -join "`n") + "`n")
}

function Update-Source {
    # Copies a source's skills from upstream and records the ref and commit.
    param($SourceEntry, [string]$RequestedRef)

    $repository = $SourceEntry.repository
    $skillsPath = Get-SourceSetting $SourceEntry 'skillsPath' 'skills'
    $tagPrefix = Get-SourceSetting $SourceEntry 'tagPrefix' 'v'

    $checkoutRef = $RequestedRef
    if (-not $checkoutRef) {
        $checkoutRef = Get-LatestReleaseTag $repository $tagPrefix
        Write-Host "  Latest release tag: $checkoutRef" -ForegroundColor Yellow
    }

    $checkout = Get-UpstreamCheckout $repository $checkoutRef
    try {
        Write-Host "  Checked out $checkoutRef ($($checkout.Commit))" -ForegroundColor Yellow

        # Confirm every listed skill exists upstream before touching the repo.
        foreach ($skill in $SourceEntry.skills) {
            $upstreamSkillMd = Join-Path $checkout.Path "$skillsPath/$skill/SKILL.md"
            if (-not (Test-Path -LiteralPath $upstreamSkillMd -PathType Leaf)) {
                throw "Upstream has no $skillsPath/$skill/SKILL.md at $checkoutRef"
            }
        }

        New-Item -ItemType Directory -Force -Path $skillsRoot | Out-Null
        foreach ($skill in $SourceEntry.skills) {
            # Replace the whole folder so files removed upstream go away too.
            $destination = Join-Path $skillsRoot $skill
            if (Test-Path -LiteralPath $destination) {
                Remove-Item -LiteralPath $destination -Recurse -Force
            }
            Copy-Item -LiteralPath (Join-Path $checkout.Path "$skillsPath/$skill") -Destination $skillsRoot -Recurse -Force
            Write-Host "  Copied $skill" -ForegroundColor White
        }

        $vendored = [pscustomobject]@{ ref = $checkoutRef; commit = $checkout.Commit }
        $SourceEntry | Add-Member -NotePropertyName 'vendored' -NotePropertyValue $vendored -Force
        return $checkoutRef
    }
    finally {
        Remove-Item -LiteralPath $checkout.Path -Recurse -Force -ErrorAction SilentlyContinue
    }
}

function Test-Source {
    # Compares a source's vendored skills to its pinned commit.
    # Returns $true when every skill folder matches upstream.
    param($SourceEntry)

    $repository = $SourceEntry.repository
    $skillsPath = Get-SourceSetting $SourceEntry 'skillsPath' 'skills'
    $tagPrefix = Get-SourceSetting $SourceEntry 'tagPrefix' 'v'
    $pinnedRef = $SourceEntry.PSObject.Properties['vendored']?.Value?.ref
    $pinnedCommit = $SourceEntry.PSObject.Properties['vendored']?.Value?.commit

    if (-not $pinnedCommit) {
        Write-Host "  Not vendored yet; run without -Check to copy it" -ForegroundColor Red
        return $false
    }

    $latestTag = Get-LatestReleaseTag $repository $tagPrefix
    if ($latestTag -ne $pinnedRef) {
        Write-Host "  Newer release available: $latestTag (vendored: $pinnedRef)" -ForegroundColor Yellow
    }
    else {
        Write-Host "  Vendored $pinnedRef is the latest release" -ForegroundColor White
    }

    $checkout = Get-UpstreamCheckout $repository $pinnedCommit
    try {
        $matchesUpstream = $true
        foreach ($skill in $SourceEntry.skills) {
            $differences = Compare-SkillFolder (Join-Path $checkout.Path "$skillsPath/$skill") (Join-Path $skillsRoot $skill)
            if ($differences) {
                $matchesUpstream = $false
                Write-Host "  $skill differs from $pinnedRef ($pinnedCommit):" -ForegroundColor Red
                $differences | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
            }
            else {
                Write-Host "  $skill matches upstream" -ForegroundColor White
            }
        }
        return $matchesUpstream
    }
    finally {
        Remove-Item -LiteralPath $checkout.Path -Recurse -Force -ErrorAction SilentlyContinue
    }
}

try {
    $config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
    Assert-ValidConfig $config

    $selectedSources = @($config.sources)
    if ($Source) {
        $unknown = $Source | Where-Object { $config.sources.name -notcontains $_ }
        if ($unknown) {
            throw "Unknown source(s): $($unknown -join ', '). Known: $($config.sources.name -join ', ')"
        }
        $selectedSources = @($config.sources | Where-Object { $Source -contains $_.name })
    }

    if ($Ref -and $Check) {
        throw "-Ref cannot be used with -Check; -Check compares against each source's pinned commit"
    }
    if ($Ref -and $selectedSources.Count -ne 1) {
        throw "-Ref needs exactly one source; use -Source to pick one"
    }

    $failures = @()
    $updated = @()
    foreach ($entry in $selectedSources) {
        Write-Host "Source: $($entry.name)" -ForegroundColor Green
        try {
            if ($Check) {
                if (-not (Test-Source $entry)) { $failures += $entry.name }
            }
            else {
                $copiedRef = Update-Source $entry $Ref
                $updated += "$($entry.name)@$copiedRef"
            }
        }
        catch {
            Write-Host "  Failed: $_" -ForegroundColor Red
            $failures += $entry.name
        }
    }

    if (-not $Check) {
        if ($updated) {
            Save-Config $config
            $changes = Invoke-Git @('-C', $repoRoot, 'status', '--short', '--', '.claude/skills', 'scripts/vendored-skills.json')
            if ($changes) {
                Write-Host "Changes:" -ForegroundColor Cyan
                $changes | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
                Write-Host "Suggested commit title:" -ForegroundColor Cyan
                Write-Host "  chore: update vendored skills ($($updated -join ', '))" -ForegroundColor White
            }
            else {
                Write-Host "No changes; vendored skills already match upstream." -ForegroundColor Green
            }
        }
    }

    if ($failures) {
        throw "Failed source(s): $($failures -join ', ')"
    }

    if ($Check) {
        Write-Host "All vendored skills match their pinned upstream commits." -ForegroundColor Green
    }
}
catch {
    # Handle any errors that occur during the process
    Write-Error "Error updating vendored skills: $_"
    exit 1
}
