$ErrorActionPreference = 'Stop'
Push-Location C:\nginx
try {
    & .\nginx.exe -t
    if ($LASTEXITCODE -ne 0) { throw 'nginx configuration validation failed after certificate renewal.' }
    & .\nginx.exe -s reload
    if ($LASTEXITCODE -ne 0) { throw 'nginx certificate reload failed.' }
} finally { Pop-Location }
