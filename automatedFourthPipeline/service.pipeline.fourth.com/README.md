# Square to Fourth Pipeline

This repository contains the .NET 10 Square-to-Fourth integration. It reads Square sales and labor data, maps it into Fourth hospitality sales CSV and timesheet XML payloads, optionally uploads those payloads to Fourth, and stores every worker outcome in SQL for audit and replay.

The current production-facing path is:

1. `web.pipeline.fourth.com` is the admin web app.
2. `squareservice.pipeline.fourth.com` is the scheduled Square-to-Fourth worker.
3. `domain.pipeline.fourth.com`, `square.pipeline.fourth.com`, `com.fourth.pipeline.pos`, `data.pipeline.fourth.com`, and `shared.pipeline.fouth.com` are shared libraries.
4. `service.pipeline.fourth.com` is the older Topshelf service. Do not deploy it for the current Square worker unless there is a separate legacy requirement.

## Current Status

The solution is upgraded to .NET 10 and the Square-to-Fourth sales and hospitality/labor paths have reproducible sandbox coverage.

Latest sandbox readiness verification was completed on 2026-07-07:

- `dotnet build service.pipeline.fourth.com.sln --no-restore` passed with `14` existing warnings and `0` errors.
- `dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"` passed for the default suite.
- `dotnet list service.pipeline.fourth.com.sln package --vulnerable --include-transitive` reported no vulnerable packages.
- Expanded Square sandbox seed test passed for payment/refund run `20260706235456`.
- Read-only Square sandbox replay test passed for payment/refund run `20260706235456`.
- Filtered readiness sales CSV contains `144` rows for the successful run orders: `10` positive tenders, `3` negative refund tenders, `20` product PLUs, `20` modifier descriptions, `18` discount rows, and `14` service-charge rows.
- Tender coverage includes Square `CARD`, `CASH`, and `EXTERNAL` payments plus linked `CARD_REFUND`, `CASH_REFUND`, and `EXTERNAL_REFUND` rows.
- Readiness timesheet XML contains `215` records across `15` Fourth employees and `15` clock-in dates, with `210` closed clock-in/out rows and `5` open clock-in rows.
- The real worker path has run in generate-only mode against the remote SQL sandbox integration for the payment/refund windows, producing CSV/XML files and persisted `PipelineRunRecords` rows without live Fourth credentials.
- Structured DB event logging is enabled through `PipelineEventLogs`; the latest generate-only verification wrote `22` event rows covering job, brand, Square read, transform, file, and run-record stages with `0` failed event rows.
- The remote SQL sandbox integration has `15` active Square-to-Fourth employee mappings for the readiness team members.

Live Fourth upload is implemented but still needs real Fourth-provided credentials and endpoints before it should be enabled.

## Solution Layout

- `web.pipeline.fourth.com`: ASP.NET Core admin app with Identity auth, configuration CRUD, Square OAuth callback, credential testing, employee mapping, and run-record screens.
- `squareservice.pipeline.fourth.com`: Quartz scheduled worker hosted as a .NET Worker Service. This is the current Square-to-Fourth runtime.
- `domain.pipeline.fourth.com`: EF context, migrations, Square-to-Fourth mapping/generation logic, timesheet XML generation, and sales row factories.
- `square.pipeline.fourth.com`: Square API service wrappers for locations, catalog, orders, payments, refunds, employees, and labor.
- `com.fourth.pipeline.pos`: Fourth sales/timesheet HTTP client and CSV contract model.
- `data.pipeline.fourth.com`: persistence models for brands, stores, integrations, credentials, configs, employee mappings, and run records.
- `shared.pipeline.fouth.com`: shared enums and small cross-project types.
- `tests.domain.pipelines.fourth.com`: mapping, replay, sandbox, Fourth OAuth, and contract tests.
- `tests.square.pipeline.fourth.com` and `tests.pos.pipeline.fourth.com`: legacy/targeted tests. Several live tests are explicit and skipped by default.
- `outputs`: generated local audit artifacts.

## Runtime Data Flow

The scheduled worker runs `PushNightlyDataToSquareUSTimeZoneJob`.

For each active brand with a `SquareToFourthPosSales` integration:

1. Load the active Square API credential from `CredentialsPool`.
2. Use Square OAuth refresh token when present, otherwise use `LatestAccessToken`.
3. Resolve Square API base URL from `SquareApi:BaseUrl`, `SquareSandbox:BaseUrl`, or the credential `BaseEndpoint`.
4. Load active Square-to-Fourth store integrations for the brand.
5. Read Square locations, catalog, employees/team, orders, payments, refunds, and labor timecards.
6. Generate Fourth timesheet XML for Square labor data.
7. Generate Fourth hospitality sales CSV rows for completed paid Square orders.
8. Write generated files to configured output directories.
9. Insert a `PipelineRunRecords` row for each data type and outcome.
10. Insert `PipelineEventLogs` rows for job/brand/store stages, Square reads, transforms, generated files, uploads, and failures.
11. If Fourth upload flags are enabled, log in to Fourth with OAuth and upload the generated payloads.
12. On failures, write failed run records, event logs, Serilog logs, and send email alerts when alerting is configured.

The worker writes records for generated, uploaded, skipped, and failed outcomes. The payload text is stored in `PipelineRunRecords.Payload` so the exact CSV/XML that was generated can be inspected later.

## Database Model

The main SQL database is `FourthSalesPipelineContext`. The web app also uses an Identity database configured by `DefaultConnection`.

`FourthPipelineContext` respects DI-supplied connection strings. Its built-in fallback connection is only used when the context is created without configured options, such as older direct construction paths.

Important tables:

- `Brands`: customer/brand records.
- `BrandIntegrations`: brand-level integration type switches.
- `Stores`: stores/sites under a brand.
- `StoreIntegrations`: active store-level integrations.
- `SquareStoreConfigs`: Square location ID per store integration.
- `FourthSalesApiStoreConfig`: Fourth unit/site location config per store integration.
- `CredentialsPool`: Square and Fourth credentials.
- `SquareEmployeeMappings`: Square team member ID to Fourth employee number mappings.
- `PipelineRunRecords`: durable audit records for generated/uploaded/skipped/failed worker outcomes, including generated payload text.
- `PipelineEventLogs`: structured stage/API/transform/upload event records for each worker run.
- `Log`: Serilog SQL sink table for worker logs.

Current migrations of note:

- `20260704211421_squareEmployeeMappings`
- `20260704230556_pipelineRunRecords`
- `20260707010423_pipelineEventLogs`

Apply migrations to the target SQL database before running the worker or web app against a new environment.

```powershell
dotnet ef database update --project domain.pipeline.fourth.com\domain.pipeline.fourth.com.csproj --startup-project web.pipeline.fourth.com\web.pipeline.fourth.com.csproj
```

If EF tooling is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

## Configuration

Do not commit production secrets. Use deployment-time `appsettings.Production.json`, environment variables, user secrets for local development, or the hosting platform secret store.

### Connection Strings

The web app needs:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=FourthSalesPipelineContextAuth;User ID=...;Password=...;TrustServerCertificate=True",
    "FourthSalesPipelineContext": "Server=...;Database=FourthSalesPipelineContext;User ID=...;Password=...;TrustServerCertificate=True"
  }
}
```

The worker needs:

```json
{
  "ConnectionStrings": {
    "FourthSalesPipelineContext": "Server=...;Database=FourthSalesPipelineContext;User ID=...;Password=...;TrustServerCertificate=True"
  }
}
```

Environment variable equivalent:

```powershell
$env:ConnectionStrings__FourthSalesPipelineContext = "Server=...;Database=...;User ID=...;Password=...;TrustServerCertificate=True"
```

### Square OAuth

Configure named Square OAuth applications in the protected admin area:

1. Open **API Credentials > Square OAuth Applications**.
2. Add the application name, Sandbox or Production environment, Square application ID, application secret, and registered redirect URL.
3. Open **Client Setup**, choose **Connect Square**, and select the named Square application to use.

The application secret is stored in the `SquareOAuthApplications` table and is never rendered back into the admin UI. Restrict database and admin access accordingly. Existing `SquareOAuth` host configuration remains available as a compatibility fallback for credentials created before named applications were introduced.

Square must have the exact redirect URI registered for the matching application environment. The production callback is `https://squaretofourth.store/oauthredirect/accept`.

The admin app creates or updates `CredentialsPool` rows with `CredentialType=SquareApi`.

Square credential fields:

- `CredentialType`: `SquareApi`.
- `BrandId`: owning brand.
- `RefreshToken`: long-lived OAuth refresh token when available.
- `LatestAccessToken`: latest Square access token, or a static sandbox token for sandbox-only setups.
- `BaseEndpoint`: optional Square API base URL. Use `https://connect.squareupsandbox.com` for sandbox.

OAuth access and refresh tokens are stored per brand in `CredentialsPool`. Their metadata records the selected `SquareOAuthApplication` so web and worker refreshes continue using the same Square application.

For local sandbox tests, `squareservice.pipeline.fourth.com/appsettings.Local.json` can contain:

```json
{
  "SquareSandbox": {
    "ApplicationId": "sandbox-app-id",
    "AccessToken": "sandbox-access-token",
    "BaseUrl": "https://connect.squareupsandbox.com"
  }
}
```

### Fourth OAuth

Fourth OAuth 2.0 `client_credentials` login is supported. The worker uses a `CredentialType=FourthBaseCredential` row for each brand when Fourth upload is enabled.

Fourth credential fields:

- `CredentialType`: `FourthBaseCredential`.
- `BrandId`: owning brand.
- `BaseEndpoint`: Fourth API base URL, for example the ePOS gateway API base.
- `ClientId`: Fourth OAuth client ID.
- `ClientSecret`: Fourth OAuth client secret.
- `SupplimentalData1`: optional OAuth token endpoint override.
- `SupplimentalData2`: OAuth scope.
- `Username` and `Password`: only needed for older password-grant credentials.
- `LatestAccessToken`: populated after successful login/test.
- `RefreshToken`: populated if Fourth returns a refresh token.

If `SupplimentalData1` is empty, the client derives `[ROOT]/oauth/connect/token` from the API base URL and strips `/api/...` where present.

Example:

```json
{
  "BaseEndpoint": "https://api-dev.fourth.com/prelive/api/eposgateway/",
  "ClientId": "fourth-client-id",
  "ClientSecret": "fourth-client-secret",
  "SupplimentalData1": "https://api-dev.fourth.com/prelive/oauth/connect/token",
  "SupplimentalData2": "FourthScopeProvidedByFourth"
}
```

### Worker Settings

Worker settings live under `squareservice.pipeline.fourth.com/appsettings.json` or environment variables.

```json
{
  "SquareToFourthSales": {
    "CronExpression": "0 0 11 * * ?",
    "CsvOutputDirectory": "C:\\FourthPipeline\\SquareToFourthSales",
    "UploadToFourth": false,
    "RunOnStartup": false,
    "OverrideStartUtc": "",
    "OverrideEndUtc": ""
  },
  "SquareToFourthTimesheets": {
    "XmlOutputDirectory": "C:\\FourthPipeline\\SquareToFourthTimesheets",
    "UploadToFourth": false,
    "XmlEndpoint": "",
    "OverrideStartUtc": "",
    "OverrideEndUtc": "",
    "EmployeeNumberMappings": {},
    "LocationEmployeeNumberMappings": {}
  }
}
```

Key behavior:

- `CronExpression`: Quartz cron expression for the worker schedule.
- `CsvOutputDirectory`: where sales CSV files are written.
- `SquareToFourthSales:UploadToFourth`: when `false`, generate CSV and write `Generated` run records only. When `true`, require Fourth credentials and upload to Fourth.
- `SquareToFourthSales:RunOnStartup`: triggers the Quartz job immediately when the worker starts. Use for manual backfills/tests, not normal production scheduling.
- `OverrideStartUtc` / `OverrideEndUtc`: optional replay/backfill window overrides. Leave empty for normal previous-day processing.
- `XmlOutputDirectory`: where timesheet XML files are written.
- `SquareToFourthTimesheets:UploadToFourth`: when `false`, generate XML and write `Generated` run records only. When `true`, upload XML to Fourth.
- `SquareToFourthTimesheets:XmlEndpoint`: Fourth XML endpoint path or URL used for timesheet upload.
- `EmployeeNumberMappings`: global Square team member to Fourth employee mappings.
- `LocationEmployeeNumberMappings`: per-Square-location overrides.

Database-backed `SquareEmployeeMappings` override config mappings. Unmapped Square team members fall back to the Square team member ID so the XML remains traceable.

### Alerts

Failure email alerting is wired through `PipelineAlertService`.

```json
{
  "PipelineAlerts": {
    "Enabled": false,
    "ToAddress": "nadavdrewe@gmail.com",
    "FromAddress": "pipeline@example.com",
    "Smtp": {
      "Host": "smtp.example.com",
      "Port": 587,
      "EnableSsl": true,
      "Username": "smtp-user",
      "Password": "smtp-password",
      "UseDefaultCredentials": false
    }
  }
}
```

Alerts fire for:

- whole job failure;
- brand-level setup/auth/login failure;
- store-level sales CSV failure;
- store-level timesheet XML failure;
- non-2xx Fourth upload responses.

Alert failures are logged but do not hide the original pipeline failure.

## Admin Setup

Use the web admin app for normal setup.

1. Create or select a brand.
2. Add a Square credential under `/BaseCredentials/Create` or run the Square OAuth flow.
3. Add a Fourth credential under `/BaseCredentials/Create`.
4. Create the Square-to-Fourth integration from `/Brands/CreateNewSquareToFourthSalesIntegration`, or create the records manually:
   - active `BrandIntegration` with `SquareToFourthPosSales`;
   - active `Store`;
   - active `StoreIntegration` with `SquareToFourthPosSales`;
   - active `SquareStoreConfig` with Square `LocationId`;
   - active `FourthSalesApiStoreConfig` with `UnitId` and optional `SiteLocationCode`.
5. Add employee mappings under `/SquareEmployeeMappings`.
6. Use `/BaseCredentials/Index` -> `Test Creds` to validate Square and Fourth credentials.
7. Use `/PipelineRunRecords/Index` to inspect worker results after a run.

Useful admin routes:

- `/BaseCredentials/Index`
- `/Brands/Index`
- `/Brands/CreateNewSquareToFourthSalesIntegration`
- `/StoreIntegrations/Index`
- `/SquareStoreConfigs/Index`
- `/FourthSalesApiStoreConfigs/Index`
- `/SquareEmployeeMappings/Index`
- `/PipelineRunRecords/Index`
- `/PipelineEventLogs/Index`

## Deployment

### Prerequisites

- Windows host for the worker service.
- .NET 10 runtime for the worker.
- ASP.NET Core Hosting Bundle / .NET 10 runtime for IIS-hosted web app.
- SQL Server database reachable from web and worker hosts.
- Network access from worker to Square and Fourth APIs.
- Output directories created and writable by the worker service account.
- SMTP credentials if alerts are enabled.

### Build and Verify

Run from the repository root:

```powershell
dotnet restore service.pipeline.fourth.com.sln
dotnet build service.pipeline.fourth.com.sln --no-restore
dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"
dotnet list service.pipeline.fourth.com.sln package --vulnerable --include-transitive
```

Run the explicit payment/refund readiness Square replay when sandbox credentials are available:

```powershell
dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxReplayTests.ReplayPaymentRefundSandboxSeed_ThenVerifyTenderAndRefundCoverage" --logger "console;verbosity=detailed" --no-build
```

### Publish Web App

```powershell
dotnet publish web.pipeline.fourth.com\web.pipeline.fourth.com.csproj -c Release -o C:\Deploy\FourthPipeline\web
```

Deploy the published folder to IIS or the selected ASP.NET Core host.

Before starting the site:

- set production connection strings;
- set both `SquareOAuth` environment settings required for the deployment;
- run Identity and pipeline DB migrations if the target DB is new;
- ensure HTTPS is configured because Square OAuth redirect URIs must match exactly.

### Publish Worker

```powershell
dotnet publish squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj -c Release -o C:\Deploy\FourthPipeline\square-worker
```

Create output directories:

```powershell
New-Item -ItemType Directory -Force C:\FourthPipeline\SquareToFourthSales
New-Item -ItemType Directory -Force C:\FourthPipeline\SquareToFourthTimesheets
```

Install as a Windows service:

```powershell
sc.exe create FourthSquarePipelineWorker binPath= "C:\Deploy\FourthPipeline\square-worker\squareservice.pipeline.fourth.com.exe" start= auto
sc.exe description FourthSquarePipelineWorker "Square to Fourth sales and timesheet worker"
sc.exe start FourthSquarePipelineWorker
```

To update the worker:

```powershell
sc.exe stop FourthSquarePipelineWorker
dotnet publish squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj -c Release -o C:\Deploy\FourthPipeline\square-worker
sc.exe start FourthSquarePipelineWorker
```

### One-Off Generate-Only Backfill

For controlled replay/backfill without uploading to Fourth:

```powershell
$env:SquareToFourthSales__RunOnStartup = "true"
$env:SquareToFourthSales__UploadToFourth = "false"
$env:SquareToFourthSales__OverrideStartUtc = "2026-07-06T23:52:56Z"
$env:SquareToFourthSales__OverrideEndUtc = "2026-07-07T00:11:50Z"
$env:SquareToFourthTimesheets__UploadToFourth = "false"
$env:SquareToFourthTimesheets__OverrideStartUtc = "2026-06-22T07:00:00Z"
$env:SquareToFourthTimesheets__OverrideEndUtc = "2026-07-07T00:10:28Z"
dotnet run --project squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj --no-launch-profile
```

Clear override environment variables after the run. Overrides should not be left enabled on a production scheduled service.

### Enable Live Fourth Upload

Only enable upload after the Fourth credential test succeeds and Fourth has confirmed the endpoint/scope.

1. Confirm `CredentialType=FourthBaseCredential` exists for the brand.
2. Confirm `ClientId`, `ClientSecret`, `BaseEndpoint`, and scope are populated.
3. Use `/BaseCredentials/Index` -> `Test Creds`.
4. Set:

```powershell
$env:SquareToFourthSales__UploadToFourth = "true"
$env:SquareToFourthTimesheets__UploadToFourth = "true"
$env:SquareToFourthTimesheets__XmlEndpoint = "Fourth-provided-timesheet-endpoint"
```

5. Start with a narrow backfill window and inspect `PipelineRunRecords` before enabling the normal schedule.

## Verification Artifacts

Payment/refund readiness sandbox replay artifacts:

- Run ID: `20260706235456`.
- Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706235456_PaymentRefundReplayFourthCsv.csv`
- Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706235456_PaymentRefundReplayFourthTimesheets.xml`
- Sales CSV rows: `144`.
- Sales transaction counts: `TAB_OPEN=10`, `SALES_ITEM=43`, `MODIFIER_ITEM=36`, `DISC_ITEM=18`, `SERVICE_CHARGE=14`, `TENDER=13`, `TAB_CLOSE=10`.
- Positive tenders: `10` across `CARD`, `CASH`, and `EXTERNAL`.
- Negative refund tenders: `3` across `CARD_REFUND`, `CASH_REFUND`, and `EXTERNAL_REFUND`.
- Positive tender total: `823.69`.
- Negative refund total: `-14.50`.
- Unique Fourth product PLUs: `20`.
- Unique modifier descriptions: `20`.
- Timesheet XML records: `215`.
- Distinct Fourth employees: `15`.
- Distinct clock-in dates: `15`.
- Closed/open timecards: `210` closed, `5` open.

Earlier readiness and expanded sandbox replay artifacts:

- Readiness sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706122223_ReadinessReplayFourthCsv.csv`
- Readiness timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706122223_ReadinessReplayFourthTimesheets.xml`
- Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260704232039_ExpandedReplayFourthCsv.csv`
- Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260704232039_ExpandedReplayFourthTimesheets.xml`
- Audit workbook: `C:\Code\updatedChucs\automatedFourthPipeline\service.pipeline.fourth.com\outputs\square-replay-audit\20260704232039_SquareReplayAudit.xlsx`

Worker generate-only artifacts:

- Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthHospitalitySales_2026_07_07_001232.csv`
- Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthTimesheets_2026_07_07_001229.xml`

Worker generate-only counts:

- Sales CSV rows: `153`.
- Sales transaction counts: `TAB_OPEN=13`, `SALES_ITEM=43`, `MODIFIER_ITEM=36`, `DISC_ITEM=18`, `SERVICE_CHARGE=14`, `TENDER=16`, `TAB_CLOSE=13`.
- Positive tenders: `10` across `CARD`, `CASH`, and `EXTERNAL`.
- Negative refund tenders: `6` across `CARD_REFUND`, `CASH_REFUND`, and `EXTERNAL_REFUND`.
- Unique Fourth product PLUs: `20`.
- Unique modifier descriptions: `20`.
- Timesheet XML records: `1296`.
- Intended mapped Fourth employee numbers present: `15`.
- SQL run records: latest DB logging check wrote `SalesCsv=Generated` row `10`, `TimesheetXml=Generated` row `9`, with payload text stored in `PipelineRunRecords`.
- SQL event logs: latest DB logging check wrote `22` `PipelineEventLogs` rows with `0` failures, including Square read, transform, file, and run-record events.

The worker sales window includes nearby failed sandbox attempts, so it contains more refund tender rows than the filtered replay baseline. The worker timesheet count includes all Square timecards in the configured location window. The explicit payment/refund replay test filters the intended successful run orders and verifies `144` sales rows and `215` readiness timesheet rows.

## Operations

### Routine Checks

- Check `/PipelineRunRecords/Index` after each scheduled run.
- Confirm successful rows have `Status=Generated` or `Status=Uploaded`.
- Check `RowCount`, `PeriodStartUtc`, `PeriodEndUtc`, and `OutputFullPath`.
- Review `FourthStatusCode` and `FourthResponseBody` for upload failures.
- Check the SQL `Log` table for worker runtime logs.
- Confirm output directory disk space.

### Common Failure Modes

- Square `401`: expired/invalid Square token, wrong Square base URL, or sandbox token sent to production host.
- No Square orders: wrong date window, wrong Square location ID, or no paid/completed orders in the window.
- No timesheet rows: labor permissions missing, wrong date window, or no Square timecards.
- Fourth login failure: wrong client ID/secret/scope/token endpoint.
- Fourth upload failure: wrong API endpoint, missing Fourth unit/site code, malformed payload, or Fourth-side provisioning issue.
- Unmapped employees: add rows to `SquareEmployeeMappings`; fallback XML will contain Square team member IDs.

### Rollback

- Stop the Windows service.
- Restore the previous published worker/web folder.
- Start the service/site.
- If a migration was applied, use a database backup or a reviewed EF down migration. Do not manually delete pipeline audit rows unless explicitly required.

## Security Notes

- Do not commit production Square, Fourth, SQL, or SMTP secrets.
- Prefer environment variables or platform secret stores for production.
- Credential details pages intentionally show stored tokens/secrets as `Stored`, not plaintext.
- The current source tree may contain local development settings. Treat those as environment-specific and replace them for deployment.
- Restrict web admin access because it can create credentials and trigger integration setup.

## Remaining Operational Work

- Obtain production Fourth `ClientId`, `ClientSecret`, OAuth scope, API base URL, and timesheet XML endpoint.
- Enable and test live Fourth upload with a narrow backfill window.
- Supply SMTP settings and enable `PipelineAlerts`.
- Triage the `14` existing build warnings when time allows; they did not block the latest build or tests.
- Decide whether the legacy `service.pipeline.fourth.com` Topshelf service is still needed; otherwise keep it out of the Square deployment.

## Detailed Project Log

See `AGENTS.md` for the full implementation log, sandbox seed windows, replay IDs, migration notes, and generated artifact paths.
