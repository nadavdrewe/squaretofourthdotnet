# Production deployment runbook

This release consists of two processes connected to the same SQL Server database:

- `web.pipeline.fourth.com`: the HTTPS admin site and Square OAuth callback.
- `squareservice.pipeline.fourth.com`: the scheduled Square-to-Fourth worker.

## 1. Prepare secrets

Create environment variables from `web.production.env.example` and `worker.production.env.example` in the host secret store. Do not create or commit `appsettings.Production.json` with live values.

Generate the admin password hash with PowerShell:

```powershell
$password = Read-Host "Admin password" -AsSecureString
$plainText = [System.Net.NetworkCredential]::new('', $password).Password
([System.Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($plainText)) | ForEach-Object ToString x2) -join ''
```

Set the resulting value as `StaticAdmin__PasswordHash`. The web process will refuse to start without a username, valid hash, and persistent `DataProtection__KeysDirectory` outside development.

## 2. Migrate SQL

From the solution root, with `ConnectionStrings__FourthSalesPipelineContext` and `ConnectionStrings__DefaultConnection` set for the target database:

```powershell
dotnet ef database update --project domain.pipeline.fourth.com\domain.pipeline.fourth.com.csproj --startup-project web.pipeline.fourth.com\web.pipeline.fourth.com.csproj
```

This must include migration `20260726173106_AddSquareOAuthApplications` before OAuth application configuration is available in the portal.

## 3. Publish the web application

### IIS / Windows

```powershell
dotnet publish web.pipeline.fourth.com\web.pipeline.fourth.com.csproj -c Release -o C:\Deploy\SquareToFourth\web
```

Configure an IIS HTTPS binding for `squaretofourth.store`, set the web environment variables on the application pool, and grant the application-pool identity read/write access to the `DataProtection__KeysDirectory`.

### Linux container

Build from the solution root:

```bash
docker build -f web.pipeline.fourth.com/Dockerfile -t square-to-fourth-web:release .
```

Mount a durable directory at `/var/lib/square-to-fourth/keys`, provide the web environment variables through the platform secret store, and route HTTPS traffic to container port `8080`. Enable `ForwardedHeaders__Enabled` only when requests arrive through the trusted reverse proxy.

## 4. Publish the worker

### Windows service

```powershell
dotnet publish squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj -c Release -o C:\Deploy\SquareToFourth\worker
New-Item -ItemType Directory -Force C:\SquareToFourth\sales, C:\SquareToFourth\timesheets
sc.exe create SquareToFourthWorker binPath= "C:\Deploy\SquareToFourth\worker\squareservice.pipeline.fourth.com.exe" start= auto
sc.exe failure SquareToFourthWorker reset= 86400 actions= restart/60000/restart/60000/restart/60000
```

Set the worker environment variables on the service account, grant it write access to its output directories, then start the service.

### Linux container

Build from the solution root:

```bash
docker build -f squareservice.pipeline.fourth.com/Dockerfile -t square-to-fourth-worker:release .
```

Mount durable sales and timesheet output directories and configure the worker with its environment variables.

## 5. Configure Square and validate

1. In the Square Developer Dashboard, configure `https://squaretofourth.store/oauthredirect/accept` on the production application.
2. Open the portal, add the production Square OAuth application, and create the client brand.
3. Use **Connect Square** and have the client authorise with their own Square seller account.
4. Add the client’s Fourth credential and map each Square location to Fourth.
5. Keep both Fourth upload flags `false` for the first run. Confirm generated data, run records, event logs, and token refresh behaviour.
6. Enable Fourth uploads only after Fourth has confirmed the endpoint, scope, and an accepted generated payload.

## 6. Production checks

- `GET /health/live` returns `200` when the web process is alive.
- `GET /health/ready` returns `200` only when the pipeline SQL database is reachable.
- Verify the worker service/container is running and its first scheduled run writes `PipelineRunRecords` and `PipelineEventLogs`.
- Confirm the Square OAuth application and client connection are visible in **Client Setup**.
