param([switch]$EnableHttps)
$ErrorActionPreference = 'Stop'
Import-Module WebAdministration
$configPath = 'C:\nginx\conf\nginx.conf'
$sapPath = 'C:\nginx\conf\squaresap.conf'
$original = [IO.File]::ReadAllText($configPath)
$oldSap = if (Test-Path -LiteralPath $sapPath) { [IO.File]::ReadAllText($sapPath) } else { $null }
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
Copy-Item -LiteralPath $configPath -Destination "C:\nginx\conf\backups\nginx-before-sap-$stamp.conf"
if ($oldSap) { Copy-Item -LiteralPath $sapPath -Destination "C:\nginx\conf\backups\squaresap-$stamp.conf" }
$text = [IO.File]::ReadAllText((Join-Path $PSScriptRoot 'squaresap-http.conf'))
if ($EnableHttps) {
    if (-not (Test-Path -LiteralPath 'C:\wacs\certs\squaresap.store\squaresap.store-chain.pem')) { throw 'Issue the certificate first.' }
    $text += "`r`n" + [IO.File]::ReadAllText((Join-Path $PSScriptRoot 'squaresap-https.conf'))
}
try {
    [IO.File]::WriteAllText($sapPath, $text)
    if (-not $original.Contains('include C:/nginx/conf/squaresap.conf;')) {
        $updated = $original.Replace('http {', "http {`r`n    include C:/nginx/conf/squaresap.conf;")
        if ($updated -eq $original) { throw 'Cannot find nginx http context.' }
        [IO.File]::WriteAllText($configPath, $updated)
    }
    Push-Location C:\nginx
    try {
        & .\nginx.exe -t
        if ($LASTEXITCODE -ne 0) { throw 'nginx configuration validation failed.' }
        & .\nginx.exe -s reload
        if ($LASTEXITCODE -ne 0) { throw 'nginx reload failed.' }
    } finally { Pop-Location }
    if (-not (Get-WebBinding -Name SquareToFourth -Protocol http | Where-Object bindingInformation -eq '*:80:squaresap.store')) {
        New-WebBinding -Name SquareToFourth -Protocol http -Port 80 -HostHeader squaresap.store
    }
    Write-Output "DOMAIN_READY HTTPS=$EnableHttps"
} catch {
    [IO.File]::WriteAllText($configPath, $original)
    if ($null -ne $oldSap) { [IO.File]::WriteAllText($sapPath, $oldSap) }
    Push-Location C:\nginx
    try { & .\nginx.exe -s reload } finally { Pop-Location }
    throw
}
