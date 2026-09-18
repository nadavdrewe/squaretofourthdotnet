param([string]$Release = 'sap-20260917-01')
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
if ($Release -notmatch '^sap-[0-9]{8}-[0-9]{2}$') { throw 'Invalid release name.' }
Import-Module WebAdministration
Add-Type -AssemblyName System.IO.Compression.FileSystem
$live = 'C:\inetpub\wwwroot\SquareToFourth'
$stage = "C:\Deploy\SquareToFourth\incoming\$Release"
$archive = "$stage.zip"
$backup = 'C:\Deploy\SquareToFourth\backups\SquareToFourth-' + (Get-Date -Format 'yyyyMMdd-HHmmss')
if ([IO.Path]::GetFullPath((Get-Item IIS:\Sites\SquareToFourth).physicalPath) -ne $live) { throw 'Unexpected IIS path.' }
if (Test-Path -LiteralPath $stage) { throw 'Staging directory already exists.' }
[IO.Compression.ZipFile]::ExtractToDirectory($archive, $stage)
foreach ($file in @('web.pipeline.fourth.com.dll','wwwroot\css\sap.css','wwwroot\css\sap-assistant.css','wwwroot\css\sap-form-v3.css','wwwroot\js\sap.js')) {
    if (-not (Test-Path -LiteralPath (Join-Path $stage $file))) { throw "Missing artifact $file" }
}
Get-ChildItem -LiteralPath $live -Filter 'appsettings*.json' -File | Copy-Item -Destination $stage
Copy-Item -LiteralPath "$live\web.config" -Destination "$stage\web.config" -Force

# Schema additions are isolated from the existing pipeline and OAuth tables.
$settings = Get-Content -LiteralPath "$live\appsettings.json" -Raw | ConvertFrom-Json
$sql = New-Object System.Data.SqlClient.SqlConnection($settings.ConnectionStrings.FourthSalesPipelineContext)
try {
    $sql.Open()
    $command = $sql.CreateCommand()
    $command.CommandText = [IO.File]::ReadAllText((Join-Path $PSScriptRoot 'sap-discovery.sql'))
    $command.ExecuteNonQuery() | Out-Null
} finally { $sql.Dispose() }
function Stop-App {
    if ((Get-WebsiteState SquareToFourth).Value -ne 'Stopped') { Stop-Website SquareToFourth }
    if ((Get-WebAppPoolState SquareToFourth).Value -ne 'Stopped') { Stop-WebAppPool SquareToFourth }
    for ($i=0; $i -lt 30; $i++) {
        $workers = Get-CimInstance Win32_Process -Filter "Name='w3wp.exe'" | Where-Object { $_.CommandLine -match '-ap\s+"SquareToFourth"' }
        if (-not $workers) { return }
        Start-Sleep -Seconds 1
    }
    throw 'The application pool did not release its worker.'
}
function Start-App {
    if ((Get-WebAppPoolState SquareToFourth).Value -ne 'Started') { Start-WebAppPool SquareToFourth }
    if ((Get-WebsiteState SquareToFourth).Value -ne 'Started') { Start-Website SquareToFourth }
}
function Mirror([string]$source,[string]$destination) {
    foreach ($path in @($source,$destination)) {
        $resolved = [IO.Path]::GetFullPath($path)
        if ($resolved -ne $live -and -not $resolved.StartsWith('C:\Deploy\SquareToFourth\', [StringComparison]::OrdinalIgnoreCase)) { throw "Unsafe mirror path: $resolved" }
    }
    & robocopy $source $destination /MIR /COPY:DAT /DCOPY:DAT /R:2 /W:2 /XJ /NFL /NDL /NP | Out-Null
    if ($LASTEXITCODE -ge 8) { throw "Mirror failed: $LASTEXITCODE" }
}
$backedUp = $false
try {
    Stop-App
    Mirror $live $backup
    $backedUp = $true
    Mirror $stage $live
    Start-App
    $verified = $false
    for ($i=0; $i -lt 15; $i++) {
        # Loopback certificate uses the public name; independent external TLS checks follow deployment.
        $ready = (& curl.exe -k -sS --max-time 10 https://localhost:15041/health/ready) -join "`n"
        $landing = (& curl.exe -k -sS --max-time 10 https://localhost:15041/sap) -join "`n"
        $form = (& curl.exe -k -sS --max-time 10 https://localhost:15041/sap/requirements) -join "`n"
        if ($ready.Contains('"status":"ready"') -and $landing.Contains('Six decisions. One shared plan.') -and $form.Contains('Create requirements workspace')) { $verified=$true; break }
        Start-Sleep -Seconds 2
    }
    if (-not $verified) { throw 'Deployment verification failed.' }
    $deploymentDocs = Join-Path $live 'deploy'
    New-Item -ItemType Directory -Path $deploymentDocs -Force | Out-Null
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Configure-SapAssistant.ps1') -Destination $deploymentDocs -Force
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'SAP_ASSISTANT.md') -Destination $deploymentDocs -Force
    Write-Output "DEPLOYED=$Release BACKUP=$backup SCHEMA=ready"
} catch {
    try { Stop-App; if ($backedUp) { Mirror $backup $live } } finally { Start-App }
    throw
}
