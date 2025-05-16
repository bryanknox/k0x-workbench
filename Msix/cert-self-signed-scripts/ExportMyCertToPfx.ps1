# ExportMyCertToPfx.ps1
#
# Export the certificate from 'Cert:\CurrentUser\My\' to a PFX file.
#
# See https://learn.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing#export-the-certificate-to-a-pfx-file

param (
    [Parameter(Mandatory = $true)]
    [string]$certificateThumbprint,

    [Parameter(Mandatory = $true)]
    [string]$pfxFilePath,

    [Parameter(Mandatory = $true)]
    [SecureString]$passwordSecureString
)

# Export the certificate to a PFX file.
Export-PfxCertificate `
    -cert "Cert:\CurrentUser\My\$certificateThumbprint" `
    -FilePath $pfxFilePath `
    -Password $passwordSecureString

