# PowerShell `OrExit` Pattern Guidelines

Guidelines for PowerShell functions and scripts that implement the *OrExit* pattern.

## `OrExit` Pattern for Functions and Scripts

Use the `OrExit` pattern in PowerShell scripts and functions that should immediately exit (including exiting any calling PowerShell in nested contexts) when the goal cannot be achieved or an error is encountered.

Functions that return a value on success can use this technique when a value cannot be obtained for return.

When used in GitHub workflow steps, PowerShell with an exit code of `1` will cause the step to fail.

- When the goal of the function or script cannot be achieved,
log an error and then `exit 1`. Skipping any subsequent logic.

- Catch expections, log them as errors and then `exit 1`, so that exception catch logic is not required in workflow steps.

- Add an optional `-ThrowOnError` switch parameter. When specified, the function should throw an exception instead of calling `exit 1`. This will be useful for unit testing, and when calling from scripts that need to handle exceptions directly.

- The function or script name should include an `OrExit` suffix.<br>
  To help indicate that it has behavior.

- If GitHub Action workflow annotations should be created for the error, then use the GitHub workflow commands (`::error`, `::warning`, or `::notice`) to log the errors as annotations.

Example PowerShell snippet:
```PowerShell
. . .
if (someErrorWasDetected) {
    $errorTitle = "The Error Title"
    $errorMessage = "The error message."
    Write-Host "::error title=$errorTitle::$errorMessage"
    . . .
    Write-Host "Additonal info about the error condition."
    if ($ThrowOnError) {
        throw $errorMessage
    }
    exit 1
}
. . .
# Do more stuff.
. . .
return $value
```

## Unit Testing `OrExit` Functions with `-ThrowOnError`

For unit testing error handling logic with an `OrExit` function or script, use the optional `-ThrowOnError` switch parameter, and assert on an exception thrown with the expected message.

  Example:
  ```PowerShell
  {
       New-PullRequestOrExit `
          -SourceBranch $MockSourceBranch `
          -TargetBranch $MockTargetBranch `
          -OrgUrl $MockOrgUrl `
          -Project $MockProject `
          -Repository $MockRepository `
          -Token $MockToken `
          -ThrowOnError
    } | Should -Throw "expected error message."
  ```

### Unit Testing `OrExit` Functions Exit Codes

For unit testing exit codes of an `OrExit` function or script.
Implement a wrapper function that calls the `OrExit` function and captures the exit code.

The `exit` keyword in PowerShell cannot be mocked, so unit testing exit codes gets tricky.

The exit behavior of functions and scripts can be tested using a wrapper script approach. A wrapper script is implemented and is used to call the `OrExit` function or script in a separate PowerShell process. Then unit tests that need to verify exit codes can call that wrapper script. And then check the exit code using the `$LASTEXITCODE` variable.

Example wrapper script snippet:
```PowerShell
param(
    [string]$OrderId,
    [string]$Token
)

Import-Module "$PSScriptRoot\..\Add-OrderOrExit.psm1" -Force

Add-WorkItemBuildLinkOrExit -OderId $OderId -Token $Token
```

Example unit test snippet:
```PowerShell
It "Returns exit code 1 when API call fails" {
    $scriptPath = "$PSScriptRoot\Add-OrderOrExit-Wrapper.ps1"
    powershell -NoProfile -Command "& '$scriptPath' -OderId '12345' -Token 'fake-token'; exit `$LASTEXITCODE"
    $LASTEXITCODE | Should -Be 1
}
```
