# Release Management

## Create a Release

Pushing a v-tag to the GitHub repo will trigger the `.github\workflows\vtag-release.yml` GitHub Actions workflow.
That workflow will build the app and MSI installer, and the publish a GitHub release assocated with the v-tag.

A v-tag is a git tag like: `v1.2.3`. It must be a strict 3-part sematic version.

You can use git commands like the following create and push the v-tag:
```PowerShell
git tag -a v1.2.3 -m "Release version 1.2.3"

git push origin v1.2.3
```

Or, better yet, use the PowerShell script:
```PowerShell
.\scripts\CreateVTagRelease.ps1 -Version 1.2.3
```

## Delete a Test Release

Delete a specified release of the app in GitHub. This script will delete the release and v-tag in the remote GitHub repo, and delete the v-tag from your local.

```PowerShell
.\scripts\Delete-GitHubRelease.ps1 -TagName "v1.0.0"
```

## Local Dev Build and Publish the WpfBlazor app and WiX MSI Installer

For testing on a local dev machine, you can build the app and the installer using the scripts in the repo's `scripts/` folder.

`scripts\PublishWpfApp.ps1` - Builds and publishes the WpfBlazor app locally.

`scripts\BuildWixMsi.ps1` - Builds the WiX MSI installer for published WpfBlazor app.

`scripts\PublishAndBuildMsi.ps1` - Builds and publishes the WpfBlazor app and builds the WiX MSI installer.

Example
```PowerShell
.\scripts\PublishAndBuildMsi.ps1 -Version "1.0.0"
```

### `local-published-ignored` folder

The `local-published-ignored` folder is created automatically by the `PublishWpfApp.ps1` and/or `PublishAndBuildMsi.ps1` scripts and a `.gitignore` file is added to that folder so that any content will be NOT be added to source control.

By default, folders for the published WPF app and MSI installers are created within the `local-published-ignored` folder by scripts in the repo's `scripts` folder.


