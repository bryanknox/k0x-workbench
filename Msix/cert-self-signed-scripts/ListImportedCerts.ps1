# ListImportedCerts.ps1
#
# # List certificates (with thumbprints) in the Local Machine Trusted People store.
#
# See https://learn.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing#security-considerations

# List certificates (with thumbprints)
Get-ChildItem -Path "Cert:\LocalMachine\TrustedPeople" | Select-Object Thumbprint, Subject

