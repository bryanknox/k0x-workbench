# CreateMyCertSelfSigned.ps1
#
# Create a self-signed certificate and add it to the Current User Personal store
# ('Cert:\CurrentUser\My\').
#
# See https://learn.microsoft.com/en-us/powershell/module/pki/new-selfsignedcertificate?view=windowsserver2025-ps
#
# Notes for New-SelfSignedCertificate command:
#
# -KeyUsage DigitalSignature
#   Specifies a self-signing certificate.
#
# -CertStoreLocation "Cert:\CurrentUser\My"
# 	From https://learn.microsoft.com/en-us/powershell/module/pki/new-selfsignedcertificate?view=windowsserver2025-ps#-certstorelocation
#	Specifies the certificate store in which to store the new certificate.
#
# -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
#   See https://learn.microsoft.com/en-us/powershell/module/pki/new-selfsignedcertificate?view=windowsserver2025-ps#-textextension
#   From https://learn.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing#use-new-selfsignedcertificate-to-create-a-certificate
#   • Extended Key Usage (EKU): "2.5.29.37={text}1.3.6.1.5.5.7.3.3"
#     ○ This extension indicates additional purposes for which
#       the certified public key may be used.
#       For a self-signing certificate, this parameter should include
#       the extension string "2.5.29.37={text}1.3.6.1.5.5.7.3.3",
#       which indicates that the certificate is to be used for code signing.
#   • Basic Constraints: "2.5.29.19={text}"
#     ○ This extension indicates whether or not the certificate is
#       a Certificate Authority (CA). For a self-signing certificate,
#       this parameter should include the extension string
#       "2.5.29.19={text}", which indicates that the certificate is
#       an end entity (not a CA).
#
# -Subject $subjectDN
#    See https://learn.microsoft.com/en-us/powershell/module/pki/new-selfsignedcertificate?view=windowsserver2025-ps#-subject
#    The subject of the new certificate. The subject is a distinguished name (DN)
#    that identifies the entity associated with the public key in the certificate.
#    The subject DN is a string like: "CN=Contoso Software, O=Contoso Corporation, C=US"

param (
    [Parameter(Mandatory = $true)]
    [string]$subjectDN,

    [Parameter(Mandatory = $true)]
    [string]$friendlyName
)

# Create a self-signed certificate and add it to the Current User Personal store
# ('Cert:\CurrentUser\My\').
New-SelfSignedCertificate `
    -Type Custom `
    -KeyUsage DigitalSignature `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}") `
    -Subject $subjectDN `
    -FriendlyName $friendlyName
