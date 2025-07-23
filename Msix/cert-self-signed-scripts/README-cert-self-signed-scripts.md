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

3. (Optional) Set the PFX file as a GitHub secret for use in CI/CD workflows.

    This step is only necessary if the PFX content needs to be stored as a GitHub Secret for use by GitHub workflows.

    ```
    SetPfxInGitHubSecret.ps1
    ```

    > This script uses the GitHub CLI to set a repository secret containing the base64 encoded contents of the PFX file. This is useful for storing code signing certificates in GitHub Actions workflows.

4. (Optional) Import the PFX file into the Local Machine Trusted People store.

   This step is only necessary if creating the MSIX app package via
   Visual Studio.

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

   > SECURITY: Remove the imported certificate from the
   *Local Machine Trusted People store* after building the MSIX
   with Visual Studio, and add it back again before the next build?
   See the *Security Considerations* section below.


## Limitations of Self-signed certificates

> <sup>From: https://learn.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing#create-a-self-signed-certificate</sup><br>
**Note:** When you create and use a self-signed certificate only users who install and trust your certificate can run your application. This is easy to implement for testing but it may prevent additional users from installing your application. When you are ready to publish your application we recommend that you use a certificate issued by a trusted source. This system of centralized trust helps to ensure that the application ecosystem has levels of verification to protect users from malicious actors.

## Security Considerations

> <sup>From: https://learn.microsoft.com/en-us/windows/msix/package/create-certificate-package-signing#security-considerations</sup><br>
> By adding a certificate to local machine certificate stores, you affect the certificate trust of all users on the computer. It is recommended that you remove those certificates when they are no longer necessary to prevent them from being used to compromise system trust.

When you add a certificate to the local machine certificate store, it becomes trusted by all users on that computer. This means that any application or process running on the machine can use that certificate for authentication, encryption, or other security-related functions.
