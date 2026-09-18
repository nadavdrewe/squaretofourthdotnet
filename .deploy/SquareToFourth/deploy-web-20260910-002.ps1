$ErrorActionPreference = 'Stop'

Import-Module WebAdministration

$siteName = 'SquareToFourth'
$appPoolName = 'SquareToFourth'
$livePath = [IO.Path]::GetFullPath('C:\inetpub\wwwroot\SquareToFourth')
$archivePath = [IO.Path]::GetFullPath('C:\Deploy\SquareToFourth\incoming\web-20260911-002.zip')
$stagingPath = [IO.Path]::GetFullPath('C:\inetpub\wwwroot\SquareToFourth-stage-20260911-002')
$backupPath = [IO.Path]::GetFullPath(('C:\Deploy\SquareToFourth\backups\SquareToFourth-' + (Get-Date -Format 'yyyyMMdd-HHmmss')))

$configuredPath = [IO.Path]::GetFullPath((Get-Item "IIS:\Sites\$siteName").physicalPath)
if ($configuredPath -ne $livePath) {
    throw "Unexpected IIS physical path: $configuredPath"
}
if (-not $livePath.StartsWith('C:\inetpub\wwwroot\SquareToFourth', [StringComparison]::OrdinalIgnoreCase)) {
    throw "Unsafe live path: $livePath"
}
if (-not $stagingPath.StartsWith('C:\inetpub\wwwroot\SquareToFourth-stage-', [StringComparison]::OrdinalIgnoreCase)) {
    throw "Unsafe staging path: $stagingPath"
}
if (-not $backupPath.StartsWith('C:\Deploy\SquareToFourth\backups\SquareToFourth-', [StringComparison]::OrdinalIgnoreCase)) {
    throw "Unsafe backup path: $backupPath"
}
if (-not (Test-Path -LiteralPath $archivePath -PathType Leaf)) {
    throw "Release archive not found: $archivePath"
}
if (Test-Path -LiteralPath $stagingPath) {
    throw "Staging path already exists: $stagingPath"
}

New-Item -ItemType Directory -Path $stagingPath | Out-Null
Expand-Archive -LiteralPath $archivePath -DestinationPath $stagingPath

$requiredFiles = @(
    'web.pipeline.fourth.com.dll',
    'web.config',
    'appsettings.json',
    'wwwroot\images\brand\square-black.png',
    'wwwroot\images\brand\fourth-blue.png'
)
foreach ($relativePath in $requiredFiles) {
    $candidate = Join-Path $stagingPath $relativePath
    if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) {
        throw "Staging release is missing required file: $relativePath"
    }
}

# Preserve the production-only database, OAuth, admin, and data-protection configuration.
Get-ChildItem -LiteralPath $livePath -Filter 'appsettings*.json' -File |
    Copy-Item -Destination $stagingPath -Force
Copy-Item -LiteralPath (Join-Path $livePath 'web.config') -Destination (Join-Path $stagingPath 'web.config') -Force

foreach ($configurationFile in @('appsettings.json', 'web.config')) {
    $liveHash = (Get-FileHash -LiteralPath (Join-Path $livePath $configurationFile) -Algorithm SHA256).Hash
    $stagingHash = (Get-FileHash -LiteralPath (Join-Path $stagingPath $configurationFile) -Algorithm SHA256).Hash
    if ($liveHash -ne $stagingHash) {
        throw "Production configuration was not preserved in staging: $configurationFile"
    }
}

function Stop-SquareToFourth {
    if ((Get-WebsiteState -Name $siteName).Value -ne 'Stopped') {
        Stop-Website -Name $siteName
    }
    if ((Get-WebAppPoolState -Name $appPoolName).Value -ne 'Stopped') {
        Stop-WebAppPool -Name $appPoolName
    }

    for ($attempt = 1; $attempt -le 15; $attempt++) {
        $workers = Get-CimInstance Win32_Process -Filter "Name='w3wp.exe'" |
            Where-Object { $_.CommandLine -match '-ap\s+"SquareToFourth"' }
        if (-not $workers) {
            return
        }
        Start-Sleep -Seconds 2
    }

    $workers | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }
    Start-Sleep -Seconds 2
}

function Mirror-Directory([string]$source, [string]$destination) {
    & robocopy $source $destination /MIR /COPY:DAT /DCOPY:DAT /R:2 /W:2 /XJ /NFL /NDL /NP | Out-Null
    if ($LASTEXITCODE -ge 8) {
        throw "Robocopy failed with exit code $LASTEXITCODE while mirroring $source to $destination"
    }
}

$backupCreated = $false
try {
    Stop-SquareToFourth
    Mirror-Directory $livePath $backupPath
    $backupCreated = $true
    Mirror-Directory $stagingPath $livePath

    foreach ($relativePath in $requiredFiles) {
        if (-not (Test-Path -LiteralPath (Join-Path $livePath $relativePath) -PathType Leaf)) {
            throw "Live release is missing required file after mirror: $relativePath"
        }
    }

    Start-WebAppPool -Name $appPoolName
    Start-Website -Name $siteName

    $healthy = $false
    for ($attempt = 1; $attempt -le 20; $attempt++) {
        try {
            $liveContent = (& curl.exe -k -sS -f 'https://localhost:15041/health/live') -join "`n"
            if ($LASTEXITCODE -ne 0) { throw 'Local live check failed.' }
            $readyContent = (& curl.exe -k -sS -f 'https://localhost:15041/health/ready') -join "`n"
            if ($LASTEXITCODE -ne 0) { throw 'Local ready check failed.' }
            $homeContent = (& curl.exe -k -sS -f 'https://localhost:15041/') -join "`n"
            if ($LASTEXITCODE -ne 0) { throw 'Local home check failed.' }
            & curl.exe -k -sS -f -o NUL 'https://localhost:15041/images/brand/square-black.png'
            if ($LASTEXITCODE -ne 0) { throw 'Square logo check failed.' }
            & curl.exe -k -sS -f -o NUL 'https://localhost:15041/images/brand/fourth-blue.png'
            if ($LASTEXITCODE -ne 0) { throw 'Fourth logo check failed.' }

            if ($liveContent.Contains('"status":"live"') -and
                $readyContent.Contains('"status":"ready"') -and
                $homeContent.Contains('The complete Square to Fourth suite.') -and
                $homeContent.Contains('Workforce Management') -and
                $homeContent.Contains('Inventory Management')) {
                $healthy = $true
                break
            }
        }
        catch {
            # Allow IIS and the SQL connection pool to warm up.
        }
        Start-Sleep -Seconds 3
    }

    if (-not $healthy) {
        throw 'The deployed site did not pass live, ready, and landing-page checks.'
    }

    Write-Output "DEPLOYED=$livePath"
    Write-Output "BACKUP=$backupPath"
    Write-Output 'HEALTH=live:200 ready:200 home:200 logos:200'
}
catch {
    $deploymentError = $_
    try { Stop-SquareToFourth } catch { }
    if ($backupCreated -and (Test-Path -LiteralPath $backupPath)) {
        Mirror-Directory $backupPath $livePath
    }
    try {
        if ((Get-WebAppPoolState -Name $appPoolName).Value -ne 'Started') {
            Start-WebAppPool -Name $appPoolName
        }
    }
    catch { }
    try {
        if ((Get-WebsiteState -Name $siteName).Value -ne 'Started') {
            Start-Website -Name $siteName
        }
    }
    catch { }
    throw $deploymentError
}
