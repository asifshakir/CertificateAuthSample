$clientName = "DemoBank"
$rootCertificateName = "$($clientName)RootCA"
$rootCertificatePassword = "FajrPassword@123"
$clientCertificateDuration = 5
$certificateBaseFolder = "C:\Temp"
$publicCertificateName = "$($clientName)Certificate"
$generateRootCertificate = $true

function New-RootCertificate {
    param (
        [Parameter(Mandatory = $true)]
        [string]$rootCertificateName,
        [Parameter(Mandatory = $true)]
        [string]$rootCertificatePassword,
        [Parameter(Mandatory = $true)]
        [string]$certificateBaseFolder
    )

    $rootCert = New-SelfSignedCertificate -DnsName $rootCertificateName -CertStoreLocation "cert:\LocalMachine\My" -KeyUsageProperty Sign -KeyUsage CertSign, CRLSign, DigitalSignature -NotAfter (Get-Date).AddYears(10)
    $pwd = ConvertTo-SecureString -String $rootCertificatePassword -Force -AsPlainText

    $rootPrivateCertificatePath = Join-Path -Path $certificateBaseFolder -ChildPath "$rootCertificateName.pfx"
    $rootPublicCertificatePath = Join-Path -Path $certificateBaseFolder -ChildPath "$rootCertificateName.cer"

    $path = Export-PfxCertificate -cert "cert:\LocalMachine\My\$($rootCert.Thumbprint)" -FilePath $rootPrivateCertificatePath -Password $pwd
    $path = Export-Certificate -Cert $rootCert -FilePath $rootPublicCertificatePath

    Write-Host "Root certificate (PFX) path: $rootPrivateCertificatePath" -ForegroundColor Green
    Write-Host "Root certificate password: $rootCertificatePassword" -ForegroundColor Green
}

function New-ChildCertificate {
    param (
        [Parameter(Mandatory = $true)]
        [string]$rootCertificateName,
        [Parameter(Mandatory = $true)]
        [string]$rootCertificatePassword,
        [Parameter(Mandatory = $true)]
        [string]$clientCertificateDuration,
        [Parameter(Mandatory = $true)]
        [string]$certificateBaseFolder,
        [Parameter(Mandatory = $true)]
        [string]$publicCertificateName
    )

    $rootCertPath = Join-Path -Path $certificateBaseFolder -ChildPath "$rootCertificateName.pfx"
    $pwd = ConvertTo-SecureString -String $rootCertificatePassword -Force -AsPlainText
    $rootCert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($rootCertPath, $pwd)

    $publicCertificatePath = Join-Path -Path $certificateBaseFolder -ChildPath "$publicCertificateName.cer"

    $clientCert = (New-SelfSignedCertificate -Signer $rootCert -DnsName $publicCertificateName -CertStoreLocation "cert:\LocalMachine\My" -NotAfter (Get-Date).AddYears($clientCertificateDuration))
    
    $path = Export-Certificate -Cert $clientCert -FilePath $publicCertificatePath

    Write-Host "Child certificate path: $publicCertificatePath" -ForegroundColor Green
}

# Usage
if($true -eq $generateRootCertificate) {
    New-RootCertificate -rootCertificateName $rootCertificateName -rootCertificatePassword $rootCertificatePassword -certificateBaseFolder $certificateBaseFolder
}
New-ChildCertificate -rootCertificateName $rootCertificateName -rootCertificatePassword $rootCertificatePassword -clientCertificateDuration $clientCertificateDuration -certificateBaseFolder $certificateBaseFolder -publicCertificateName $publicCertificateName