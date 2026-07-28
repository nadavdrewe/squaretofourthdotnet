# Revel Fourth Pipeline Build Plan

Last updated: 2026-06-07

## Current Status

- Status: build complete and locally verified through fake end-to-end Revel pull and Fourth SOAP submit paths.
- Active step: ready for fixture/live dry-run validation with real Revel/Fourth credentials supplied outside source.
- Working directory: `C:\Code\updatedChucs`
- Target solution directory: `C:\Code\updatedChucs\RevelFourthPipeline`

## Build Goals

- Pull nightly operations report JSON from a dynamic Revel tenant URL:
  - `{RevelBaseUrl}/reports/operations/json/`
  - Required headers: `Accept: application/json`, `API-AUTHENTICATION`, `Referer`
  - Required query flags: `show_opened=1`, `show_unpaid=1`, `show_irregular=1`
- Map Revel operations data into Fourth sales transactions.
- Generate the legacy Fourth `FourthHeader` XML shape.
- Push via Fourth SOAP `Login` and `SubmitSales`.
- Keep live submissions guarded behind dry-run/config flags.

## Work Queue

1. Scaffold .NET 10 projects. Done.
2. Port domain models and XML DTOs. Done.
3. Implement Revel pull client. Done.
4. Implement Fourth XML builder and SOAP wrapper. Done.
5. Implement worker orchestration. Done.
6. Add tests and fixtures. Done.
7. Build/test and document remaining risks. Done.
8. Add non-dry-run runner test over fake Revel and fake Fourth SOAP transports. Done.

## Key Source References

- Revel pull: `Grind/automatedreports.grind.railgunit.com/ScheduledTasks/BaseJob.cs`
- Revel auth/client: `Grind/Revel.808nd.com/Classes/RevelFactory.cs`
- Revel ops models: `Grind/Revel.808nd.com/OperationsReport/Models/OtherClasses.cs`
- Fourth SOAP helper: `Grind/Revel.808nd.com/FourthClient/FourthClient.cs`
- Fourth XML model: `Grind/Revel.808nd.com/FourthClient/SalesSubmission.cs`
- Fourth WSDL: `Grind/Revel.808nd.com/Service References/fhAPI/fhapi.wsdl`
- Legacy push orchestration: `BluebirdFourth/web.fourth.revel.com/ScheduledTasks/PushToFourth3amJob.cs`

## Notes For Handoff

- Do not copy legacy secrets or hard-coded production connection strings.
- `shoreditchgrind.revelup.com` is only an example tenant; Revel base URL must be dynamic.
- The biggest open decision is the exact financial mapping from operations report JSON to Fourth XML totals. Build tests around fixtures and keep live pushes disabled until reconciliation is accepted.

## Created Projects

- `RevelFourthPipeline/RevelFourthPipeline.slnx`
- `RevelFourthPipeline/RevelFourthPipeline.Domain/RevelFourthPipeline.Domain.csproj`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/RevelFourthPipeline.Infrastructure.csproj`
- `RevelFourthPipeline/RevelFourthPipeline.Worker/RevelFourthPipeline.Worker.csproj`
- `RevelFourthPipeline/RevelFourthPipeline.Tests/RevelFourthPipeline.Tests.csproj`

## Domain Files Added

- `RevelFourthPipeline/RevelFourthPipeline.Domain/Common/DateTimeExtensions.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Domain/Configuration/RevelFourthPipelineOptions.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Domain/Revel/OperationsReportModels.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Domain/Fourth/FourthSalesXmlModels.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Domain/Fourth/FourthSalesModels.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Domain/Pipeline/PipelineModels.cs`

## Infrastructure Files Added

- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Abstractions/IRevelOperationsReportClient.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Abstractions/IFourthSalesXmlBuilder.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Abstractions/IFourthSoapClient.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Abstractions/IRevelOperationsToFourthMapper.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Abstractions/IRevelFourthPipelineRunner.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Serialization/FlexibleDecimalJsonConverter.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Revel/RevelOperationsReportClient.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Mapping/RevelOperationsToFourthMapper.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Fourth/FourthSalesXmlBuilder.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Fourth/FourthSoapClient.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/Pipeline/RevelFourthPipelineRunner.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Infrastructure/DependencyInjection.cs`

## Worker Files Updated

- `RevelFourthPipeline/RevelFourthPipeline.Worker/Program.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Worker/Worker.cs`
- `RevelFourthPipeline/RevelFourthPipeline.Worker/appsettings.json`
- `RevelFourthPipeline/RevelFourthPipeline.Worker/appsettings.Development.json`

## Verification Log

- `dotnet test RevelFourthPipeline.slnx` first run: failed 1 XML builder test because the XML writer closed the memory stream before `XmlDocument.Load`.
- Fix applied: `FourthSalesXmlBuilder.SerializeToDocument` now uses `XmlWriterSettings.CloseOutput = false`.
- `dotnet test RevelFourthPipeline.slnx` second run: passed 7/7 tests.
- `dotnet build RevelFourthPipeline.slnx`: succeeded with 0 warnings and 0 errors.
- `dotnet run --project RevelFourthPipeline.Worker/RevelFourthPipeline.Worker.csproj`: succeeded with default dry-run config, no stores, and clean shutdown.
- Added dry-run pipeline test using real Revel HTTP client code over fake HTTP, real mapper, real XML builder, and a throwing Fourth client to prove SOAP is skipped during dry-run.
- `dotnet test RevelFourthPipeline.slnx`: passed 8/8 tests.
- Final `dotnet build RevelFourthPipeline.slnx`: succeeded with 0 warnings and 0 errors.
- Final `dotnet run --project RevelFourthPipeline.Worker/RevelFourthPipeline.Worker.csproj`: succeeded with default dry-run config, no stores, and clean shutdown.
- Added non-dry-run pipeline test using real Revel HTTP client code over fake HTTP, real mapper, real XML builder, and real Fourth SOAP wrapper over fake SOAP responses.
- `dotnet test RevelFourthPipeline.slnx`: passed 9/9 tests.
- A parallel verification build collided with the simultaneous test compile and produced a transient `VBCSCompiler` file-lock error. Sequential rerun succeeded.
- `dotnet build RevelFourthPipeline.slnx`: succeeded with 0 warnings and 0 errors.
- `dotnet run --project RevelFourthPipeline.Worker/RevelFourthPipeline.Worker.csproj`: succeeded with default dry-run config, no stores, and clean shutdown.
- Source scan excluding `bin`, `obj`, `.vs`, and `packages`: no unfinished code markers or legacy secret patterns found. Only intentional docs examples of `shoreditchgrind.revelup.com` and placeholder `example.revelup.com` appsettings values were reported.

## Remaining Validation Before Live Submit

- Provide real `RevelFourthPipeline` config through user secrets, environment variables, or deployment secrets. Do not commit credentials.
- Run dry-run with one real store and compare generated XML totals against the Revel operations report.
- Verify the dynamic Revel base URL for each tenant/store resolves to `{RevelBaseUrl}/reports/operations/json/` and accepts the configured `API-AUTHENTICATION` secret.
- Confirm with Fourth whether PLU must be numeric-only; the legacy generated XML declared `nonNegativeInteger`, but the legacy push code can pass SKU strings.
- Confirm the operations-report money formula:
  - current mapper uses `taxable_sales + untaxable_sales` as net sales;
  - if those are zero, it falls back to `price - tax`;
  - gross is `net + tax`.
- Only set `DryRun` to `false` after reconciliation is accepted.
