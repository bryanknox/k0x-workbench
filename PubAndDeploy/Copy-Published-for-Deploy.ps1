# Copy-Published-for-Deploy.ps1
#
# Copies the current published output to the For Deployment folder.
#--------------------------------------------------------------------

Write-Host
Write-Host "Copy-Published-for-Deploy - K0x-Workbench WPF-Blazor App"
Write-Host

try
{
    # Configuration.
    $srcRelativeFolderPath = "../.k0xPublished"
    $dstRelativeFolderPath = "../.k0xDeployable"

    # Get the folder path where the current script is located.
	$thisScriptFolder = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

    $srcFolderPath = [System.IO.Path]::GetFullPath( (Join-Path $thisScriptFolder $srcRelativeFolderPath) )

    $dstFolderPath = [System.IO.Path]::GetFullPath( (Join-Path $thisScriptFolder $dstRelativeFolderPath) )

    Write-Host "Configuration:"
	Write-Host "  srcFolderPath: $srcFolderPath"
    Write-Host "  dstFolderPath: $dstFolderPath"
    Write-Host

	#----------------------------------------------------------
    # Copy the file to the destination folders.

    if (Test-Path $dstFolderPath) {
        Write-Host "Deleting existing deployment folder."
        Remove-Item -Recurse -Force $dstFolderPath
    }
    if (!(Test-Path -path $dstFolderPath))
    {
        Write-Host "Creating deployment folder."
        New-Item $dstFolderPath -Type Directory | Out-Null
    }

	Write-Host "Copying files to: $dstFolderPath"

    Copy-Item -Path "$srcFolderPath\*" -Destination $dstFolderPath -Recurse -Force

    Write-Host
    Write-Host "Done."
}
#----------------------------------------------------------
# Catch Exceptions
catch
{
    Write-Host
    Write-Host 'Exception Caught.' -foreground "red"
    Write-Host
    Write-Host $_ -foreground "red"
}

Write-Host
# Read-Host 'Press ANY key to EXIT...'
