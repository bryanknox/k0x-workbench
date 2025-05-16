# RemoveMyCert.ps1
#
# Remove certificate from the Current User Personal store.
# ('Cert:\CurrentUser\My\').

param (
    [Parameter(Mandatory = $true)]
    [string]$certificateThumbprint
)

# Remove certificate from the Current User Personal store.
# ('Cert:\CurrentUser\My\').
Remove-Item -Path "Cert:\CurrentUser\My\$certificateThumbprint"
