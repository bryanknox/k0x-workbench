# ImportPfxFileToLocalMachineTrustedPeopleStore.ps1
#
# Import the PFX file into the Local Machine Trusted People store.
#
# Requires admin privileges.
#
# See https://learn.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing#import-the-certificate-to-the-local-machine-trusted-people-store

param (
    [Parameter(Mandatory = $true)]
    [string]$certificateThumbprint,

    [Parameter(Mandatory = $true)]
    [string]$pfxFilePath,

    [Parameter(Mandatory = $true)]
    [SecureString]$passwordSecureString
)

# Import certificate into the Local Machine Trusted People store.
# Requires admin privileges.

Import-PfxCertificate `
    -CertStoreLocation "Cert:\LocalMachine\TrustedPeople" `
    -Password $passwordSecureString `
    -FilePath $pfxFilePath
