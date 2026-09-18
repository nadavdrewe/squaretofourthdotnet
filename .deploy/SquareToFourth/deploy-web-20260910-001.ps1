$ErrorActionPreference = 'Stop'

Import-Module WebAdministration

$siteName = 'SquareToFourth'
$appPoolName = 'SquareToFourth'
$expectedLivePath = [IO.Path]::GetFullPath('C:\inetpub\wwwroot\SquareToFourth')
$archivePath = [IO.Path]::GetFullPath('C:\Deploy\SquareToFourth\incoming\web-20260910-001.zip')
$stagingPath = [IO.Path]::GetFullPath('C:\inetpub\wwwroot\SquareToFourth-stage-20260910-001')
$backupPath = [IO.Path]::GetFullPath(('C:\Deploy\SquareToFourth\backups\SquareToFourth-' + (Get-Date -Format 'yyyyMMdd-HHmmss')))
$failedPath = [IO.Path]::GetFullPath(('C:\Deploy\SquareToFourth\backups\SquareToFourth-failed-' + (Get-Date -Format 'yyyyMMdd-HHmmss')))

$site = Get-Item "IIS:\Sites\$siteName"
$livePath = [IO.Path]::GetFullPath($site.physicalPath)
if ($livePath -ne $expectedLivePath) {
    throw "Unexpected IIS physical path: $livePath"
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
if (Test-Path -LiteralPath $backupPath) {
    throw "Backup path already exists: $backupPath"
}

New-Item -ItemType Directory -Path $stagingPath | Out-Null
Expand-Archive -LiteralPath $archivePath -DestinationPath $stagingPath

$requiredFiles = @(
    'web.pipeline.fourth.com.dll',
    'web.config',
    'wwwroot\images\brand\square-black.png',
    'wwwroot\images\brand\fourth-blue.png'
)
foreach ($relativePath in $requiredFiles) {
    $candidate = Join-Path $stagingPath $relativePath
    if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) {
        throw "Release is missing required file: $relativePath"
    }
}

# Production secrets and persistent data-protection settings stay with the live host.
Get-ChildItem -LiteralPath $livePath -Filter 'appsettings*.json' -File |
    Copy-Item -Destination $stagingPath -Force
Copy-Item -LiteralPath (Join-Path $livePath 'web.config') -Destination (Join-Path $stagingPath 'web.config') -Force

$oldMoved = $false
$newMoved = $false
try {
    Stop-Website -Name $siteName
    Stop-WebAppPool -Name $appPoolName
    Start-Sleep -Seconds 2

    Move-Item -LiteralPath $livePath -Destination $backupPath
    $oldMoved = $true
    Move-Item -LiteralPath $stagingPath -Destination $livePath
    $newMoved = $true

    Start-WebAppPool -Name $appPoolName
    Start-Website -Name $siteName

    $healthy = $false
    for ($attempt = 1; $attempt -le 20; $attempt++) {
        try {
            $live = Invoke-WebRequest -UseBasicParsing -Uri 'http://localhost:15040/health/live' -TimeoutSec 10
            $ready = Invoke-WebRequest -UseBasicParsing -Uri 'http://localhost:15040/health/ready' -TimeoutSec 10
            $home = Invoke-WebRequest -UseBasicParsing -Uri 'http://localhost:15040/' -TimeoutSec 10
            if ($live.StatusCode -eq 200 -and
                $ready.StatusCode -eq 200 -and
                $home.StatusCode -eq 200 -and
                $home.Content.Contains('The complete Square to Fourth suite.')) {
                $healthy = $true
                break
            }
        }
        catch {
            # IIS and SQL can take a few seconds to become ready after an in-process restart.
        }
        Start-Sleep -Seconds 3
    }

    if (-not $healthy) {
        throw 'The deployed site did not pass live, ready, and landing-page checks.'
    }

    Write-Output "DEPLOYED=$livePath"
    Write-Output "BACKUP=$backupPath"
    Write-Output 'HEALTH=live:200 ready:200 home:200'
}
catch {
    $deploymentError = $_
    try { Stop-Website -Name $siteName -ErrorAction SilentlyContinue } catch { }
    try { Stop-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue } catch { }

    if ($newMoved -and (Test-Path -LiteralPath $livePath)) {
        Move-Item -LiteralPath $livePath -Destination $failedPath
    }
    if ($oldMoved -and (Test-Path -LiteralPath $backupPath)) {
        Move-Item -LiteralPath $backupPath -Destination $livePath
    }

    try { Start-WebAppPool -Name $appPoolName } catch { }
    try { Start-Website -Name $siteName } catch { }
    throw $deploymentError
}
