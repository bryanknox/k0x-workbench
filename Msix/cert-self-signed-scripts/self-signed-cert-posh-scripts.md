# PowerShell scripts for self-signed certificates

The following steps can be taken to create a self-signed certificate
suitable for development and testing MSIX application packages.

> See comments in the individual script files for details.<br>
> Also see [Create a certificate for package signing - MSIX | Microsoft Learn](https://learn.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing)

1. Create a self-signed certificate and add it to the Current User Personal store at `Cert:\CurrentUser\My\`.

    ```
    CreateMyCertSelfSigned.ps1
    ```

    > Note: The certificate created must have a `Subject` (Distinguished Name) as matches what is specified `Package.appxmanifest` file's `Identity.Publisher` property.

    You can list existing certs in there using:
    ```
    ListMyCerts.ps1
    ```

    You can remove the cert from there using:
    ```
    RemoveMyCert.ps1
    ```

2. Export the certificate from the Current User Personal store to a PFX file.

    ```
    ExportMyCertToPfx.ps1
    ```

3. Import the PFX file into the Local Machine Trusted People store.

    ```
    ImportPfxFileToLocalMachineTrustedPeopleStore.ps1
    ```
    You can list existing certs in there using:
    ```
    ListImportedCerts.ps1
    ```

    You can remove the cert from there using:
    ```
    RemoveImportedCert.ps1
    ```
