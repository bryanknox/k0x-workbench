function Get-AssemblyNameOrExit {
    <#
    .SYNOPSIS
    Extracts the AssemblyName from a .NET project file or exits with error code 1.

    .DESCRIPTION
    This function reads a .NET project file and extracts the AssemblyName element.
    If no explicit AssemblyName is found, it falls back to the project filename
    without the .csproj extension. On any error, it logs the error using GitHub
    Actions workflow commands and exits with code 1.

    .PARAMETER ProjectPath
    The path to the .NET project file (.csproj) to read.

    .PARAMETER ThrowOnError
    If specified, throws an exception instead of calling exit 1.
    This is useful for unit testing.

    .EXAMPLE
    $assemblyName = Get-AssemblyNameOrExit -ProjectPath "src/MyApp/MyApp.csproj"

    .EXAMPLE
    # For unit testing
    $assemblyName = Get-AssemblyNameOrExit -ProjectPath "test.csproj" -ThrowOnError
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath,

        [Parameter(Mandatory = $false)]
        [switch]$ThrowOnError
    )

    # Helper function to handle errors
    function HandleError {
        param(
            [string]$ErrorTitle,
            [string]$ErrorMessage
        )

        Write-Host "::error title=$ErrorTitle::$ErrorMessage"

        if ($ThrowOnError) {
            throw $ErrorMessage
        }
        exit 1
    }

    # Check if project file exists
    if (-not (Test-Path $ProjectPath)) {
        HandleError `
            -ErrorTitle "Project File Not Found" `
            -ErrorMessage "The project file '$ProjectPath' does not exist."
    }

    # Read the project file content
    try {
        $projectContent = Get-Content $ProjectPath -Raw -ErrorAction Stop
    }
    catch {
        HandleError `
            -ErrorTitle "Failed to Read Project File" `
            -ErrorMessage "Unable to read the project file '$ProjectPath'. Error: $($_.Exception.Message)"
    }

    # Check if file content is empty
    if ([string]::IsNullOrWhiteSpace($projectContent)) {
        HandleError `
            -ErrorTitle "Empty Project File" `
            -ErrorMessage "The project file '$ProjectPath' is empty or contains only whitespace."
    }

    # Try to parse as XML to validate structure
    try {
        [xml]$projectContent | Out-Null
    }
    catch {
        HandleError `
            -ErrorTitle "Invalid XML in Project File" `
            -ErrorMessage "The project file '$ProjectPath' contains invalid XML. Error: $($_.Exception.Message)"
    }

    # Extract AssemblyName using regex
    $assemblyNameMatch = [regex]::Match($projectContent, '<AssemblyName>([^<]*)</AssemblyName>')

    if ($assemblyNameMatch.Success) {
        $assemblyName = $assemblyNameMatch.Groups[1].Value.Trim()

        # Validate that the assembly name is not empty
        if ([string]::IsNullOrWhiteSpace($assemblyName)) {
            HandleError `
                -ErrorTitle "Empty AssemblyName" `
                -ErrorMessage "The AssemblyName element in project file '$ProjectPath' is empty."
        }

        Write-Host "Found explicit AssemblyName: $assemblyName"
        return $assemblyName
    }
    else {
        # Fall back to project filename without extension
        $projectFileInfo = Get-Item $ProjectPath
        $projectFileName = $projectFileInfo.BaseName

        # Validate that we have a valid project name
        if ([string]::IsNullOrWhiteSpace($projectFileName)) {
            HandleError `
                -ErrorTitle "Cannot Determine Assembly Name" `
                -ErrorMessage "No explicit AssemblyName found in project file '$ProjectPath' and unable to determine project name from filename."
        }

        $message = "No explicit AssemblyName found, using project filename: $projectFileName"

        Write-Host "::notice title=Using Project Name as AssemblyName::$message"

        return $projectFileName
    }
}

Export-ModuleMember -Function Get-AssemblyNameOrExit
