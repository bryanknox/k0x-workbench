# ListMyCerts.ps1
#
# List certificates (with their thumbprints) in the Current User Personal store
# ('Cert:\CurrentUser\My\').

Get-ChildItem Cert:\CurrentUser\My | Format-Table Subject, FriendlyName, Thumbprint
