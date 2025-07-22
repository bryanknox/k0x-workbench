# Release Management

## Create a Draft Release

A draft release of the WPF application can be created by pushing a v-tag to the GitHub repo.

```PowerShell
git tag -a v1.2.3 -m "Release version 1.2.3"

git push origin v1.2.3
```

Doing that will trigger the `.github\workflows\vtag-draft-release-zip.yml` GitHub Actions workflow.

The workflow does the following:
1. Builds and publishes the WPF app, using the version number specified in the vTag.
   - The artifact built is a zip file containing the application's executable file set.
1. Generates release notes.
1. Create a draft release with the zip file artifact.

See `.github\workflows\vtag-draft-release-zip.yml` for details.
