#--------------------------------------------------------------------
# Publish-Project.ps1
#
# Publishes the Release configuration of WPF app
# from the current source code in the project's workspace.
#
# - Builds the project in Release configuration and publishes the app
# to the publish folder.
# - Deletes the output folder if it already exists.
#--------------------------------------------------------------------
Write-Host
Write-Host "K0x-Workbench WPF-Blazor Publish Project"
Write-Host

$relativeAppProjectPath = "../src/WpfBlazor/WpfBlazor.csproj"

$relativePublishOutputPath = "../.k0xPublished"

# Get the folder path where the current script is located.
$thisScriptFolder = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

$csprojFilePath = [System.IO.Path]::GetFullPath( (Join-Path $thisScriptFolder $relativeAppProjectPath) )

$publishFolderPath = [System.IO.Path]::GetFullPath( (Join-Path $thisScriptFolder $relativePublishOutputPath) )

Write-Host "csprojFilePath: $csprojFilePath"
Write-Host
Write-Host "publishFolderPath: $publishFolderPath"
Write-Host

# Delete the publish folder if it already exists
if (Test-Path $publishFolderPath) {
    Remove-Item -Recurse -Force $publishFolderPath
    Write-Host "Deleted existing publish folder: $publishFolderPath"
}

dotnet publish $csprojFilePath `
    --output $publishFolderPath `
    --self-contained `
	--configuration:Release `

Write-Host
Write-Host "Done."
Write-Host
