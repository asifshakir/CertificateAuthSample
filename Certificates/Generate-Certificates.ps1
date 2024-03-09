$certificateName = "DemoBankCertificate"
$certificatePassword = "FajrPassword@123"
$certificateBaseFolder = "C:\Temp"
$publicCertificatePath = Join-Path -Path $certificateBaseFolder -ChildPath "$($certificateName).cer"
$privateCertificatePath = Join-Path -Path $certificateBaseFolder -ChildPath "$($certificateName).pfx"

Write-Host $publicCertificatePath
Write-Host $privateCertificatePath

$cert = New-SelfSignedCertificate -DnsName $certificateName -CertStoreLocation "cert:\LocalMachine\My" -KeyExportPolicy Exportable -KeySpec Signature
$pwd = ConvertTo-SecureString -String $certificatePassword -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath $privateCertificatePath -Password $pwd
Export-Certificate -Cert $cert -FilePath $publicCertificatePath