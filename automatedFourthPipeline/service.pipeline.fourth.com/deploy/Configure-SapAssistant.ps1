[CmdletBinding(DefaultParameterSetName = 'Enable')]
param(
    [Parameter(ParameterSetName = 'Enable')]
    [string]$Model = 'gpt-5.6-terra',

    [Parameter(ParameterSetName = 'Enable')]
    [ValidateRange(400, 2000)]
    [int]$MaxOutputTokens = 1000,

    [Parameter(ParameterSetName = 'Enable')]
    [ValidateRange(1, 500)]
    [int]$DailyWorkspaceRequestLimit = 40,

    [Parameter(Mandatory, ParameterSetName = 'Disable')]
    [switch]$Disable
)

$ErrorActionPreference = 'Stop'
$live = 'C:\inetpub\wwwroot\SquareToFourth'
$configPath = Join-Path $live 'web.config'

Import-Module WebAdministration
$sitePath = [IO.Path]::GetFullPath((Get-Item 'IIS:\Sites\SquareToFourth').physicalPath).TrimEnd('\')
if ($sitePath -ne $live.TrimEnd('\')) { throw "Unexpected IIS path: $sitePath" }
if (-not (Test-Path -LiteralPath $configPath -PathType Leaf)) { throw "Missing web.config: $configPath" }

$backupDirectory = 'C:\Deploy\SquareToFourth\configuration-backups'
New-Item -ItemType Directory -Path $backupDirectory -Force | Out-Null
$backupPath = Join-Path $backupDirectory ('web.config-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.bak')
Copy-Item -LiteralPath $configPath -Destination $backupPath

[xml]$configuration = Get-Content -LiteralPath $configPath -Raw
$aspNetCore = $configuration.SelectSingleNode('/configuration/system.webServer/aspNetCore')
if ($null -eq $aspNetCore) { throw 'web.config does not contain system.webServer/aspNetCore.' }
$variables = $aspNetCore.SelectSingleNode('environmentVariables')
if ($null -eq $variables) {
    $variables = $configuration.CreateElement('environmentVariables')
    [void]$aspNetCore.AppendChild($variables)
}

function Set-EnvironmentVariable([string]$Name, [string]$Value) {
    $node = $variables.SelectSingleNode("environmentVariable[@name='$Name']")
    if ($null -eq $node) {
        $node = $configuration.CreateElement('environmentVariable')
        $node.SetAttribute('name', $Name)
        [void]$variables.AppendChild($node)
    }
    $node.SetAttribute('value', $Value)
}

function Remove-EnvironmentVariable([string]$Name) {
    $node = $variables.SelectSingleNode("environmentVariable[@name='$Name']")
    if ($null -ne $node) { [void]$variables.RemoveChild($node) }
}

if ($Disable) {
    Set-EnvironmentVariable 'SapAssistant__Enabled' 'false'
    Remove-EnvironmentVariable 'SapAssistant__ApiKey'
} else {
    $secureKey = Read-Host 'OpenAI project API key' -AsSecureString
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureKey)
    try { $apiKey = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
    if ([string]::IsNullOrWhiteSpace($apiKey)) { throw 'An API key is required.' }
    Set-EnvironmentVariable 'SapAssistant__Enabled' 'true'
    Set-EnvironmentVariable 'SapAssistant__ApiKey' $apiKey.Trim()
    Set-EnvironmentVariable 'SapAssistant__Model' $Model.Trim()
    Set-EnvironmentVariable 'SapAssistant__MaxOutputTokens' $MaxOutputTokens.ToString([Globalization.CultureInfo]::InvariantCulture)
    Set-EnvironmentVariable 'SapAssistant__DailyWorkspaceRequestLimit' $DailyWorkspaceRequestLimit.ToString([Globalization.CultureInfo]::InvariantCulture)
    $apiKey = $null
}

$settings = [Xml.XmlWriterSettings]::new()
$settings.Indent = $true
$settings.Encoding = [Text.UTF8Encoding]::new($false)
$writer = [Xml.XmlWriter]::Create($configPath, $settings)
try { $configuration.Save($writer) } finally { $writer.Dispose() }

if ((Get-WebAppPoolState 'SquareToFourth').Value -eq 'Started') { Restart-WebAppPool 'SquareToFourth' }

$state = if ($Disable) { 'disabled' } else { "enabled with model $Model" }
Write-Output "SAP requirements guide $state. Backup: $backupPath"
