# RemoveImportedCert.ps1
#
# Remove certificate from the Local Machine Trusted People store.
#
# Requires admin privileges.

param (
    [Parameter(Mandatory = $true)]
    [string]$certificateThumbprint
)

# Remove certificate from the Local Machine Trusted People store.
# Requires admin privileges.
Remove-Item -Path "Cert:\LocalMachine\TrustedPeople\$certificateThumbprint"

