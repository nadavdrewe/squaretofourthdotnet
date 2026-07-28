# AGENTS.md

## Mission

Build a new .NET 10 pipeline that pulls accurate nightly sales data from Revel/Fourth via the Grind operations report endpoint, then pushes sales to Fourth using the SOAP webservice approach from the legacy Bluebird/Fourth project.

This is a migration/recomposition task, not a straight project reference task. The legacy sources are mostly .NET Framework projects; do not try to make the new .NET 10 project depend directly on them. Port the required models and behavior into clean .NET 10 projects.

## Repository Map

- `Grind/automatedreports.grind.railgunit.com`
  - Authoritative source for the Revel operations-report pull used by Grind automated reports.
  - Key file: `ScheduledTasks/BaseJob.cs`
  - Operations report URL shape:
    - `{RevelBaseUrl}/reports/operations/json/?employee=&online_app=&online_app_type=&online_app_platform=&show_opened=1&show_unpaid=1&show_irregular=1&range_from={0}&range_to={1}&establishment={2}`
    - `https://shoreditchgrind.revelup.com/` is only the legacy Grind example. The new build must treat the Revel host/base URL as dynamic per brand/store/integration.
    - Dates are formatted by `ToRevelDate()` as `yyyy-MM-ddTHH:mm:ss`.

- `Grind/Revel.808nd.com`
  - Shared legacy models and helpers used by both Grind and Bluebird/Fourth.
  - Revel date helper: `Classes/Utility/Extensions.cs`
  - Ops report models: `OperationsReport/Models/OtherClasses.cs`
  - Ops wrapper factory: `OperationsReport/Factory/OpsReportHourlyWrapper.cs`
  - Fourth SOAP helper: `FourthClient/FourthClient.cs`
  - Fourth XML sales model: `FourthClient/SalesSubmission.cs`
  - Fourth mapping input model: `Classes/FourthModelMapping/RevelSummedOrderItems.cs`
  - Fourth config entities: `Classes/Brand.cs`, `Classes/Establishment.cs`

- `BluebirdFourth/web.fourth.revel.com`
  - Authoritative source for the legacy push-to-Fourth orchestration.
  - Key file: `ScheduledTasks/PushToFourth3amJob.cs`
  - This currently pulls Revel orders/order-items, filters/sums them, logs XML, logs in to Fourth, and calls `SubmitSales`.
  - The new project should reuse the Fourth SOAP behavior and XML contract, but the sales source should be the Grind operations JSON path above.

- `BluebirdFourth/client.fourth.com` and `Grind/Revel.808nd.com/Service References/fhAPI`
  - Legacy generated WCF SOAP client and WSDL for Fourth.
  - Use these as contract references. For .NET 10, regenerate or port a supported SOAP client rather than copying fragile .NET Framework service reference code blindly.

- `automatedFourthPipeline/service.pipeline.fourth.com`
  - Existing newer pipeline area with several `net10.0` projects and config models.
  - It appears primarily oriented around Fourth CSV/API sales flows, not the SOAP `SubmitSales` path requested here.
  - Reuse patterns or configuration models only if they help; do not treat this as the authoritative Fourth SOAP implementation.

Ignore generated or bulky artifacts unless specifically needed: `bin/`, `obj/`, `packages/`, `_NCrunch_*`, and `wwwroot/template/`.

## Target Architecture

Recommended shape:

- `RevelFourthPipeline.Domain` (`net10.0`)
  - Port the operations-report DTOs: `RootObject`, `ProductMixData`, `SalesData`, `TaxData`, `DiscountsData`, `VoidsData`, `OpsReportHourlyWrapper` if still needed.
  - Port the Fourth XML sales DTOs from `SalesSubmission.cs`.
  - Add a new internal normalized sales model, for example `FourthSalesTransactionDraft`, so Revel parsing is not coupled directly to the SOAP DTO.

- `RevelFourthPipeline.Infrastructure` (`net10.0`)
  - `IRevelOperationsReportClient`: builds the operations JSON URL, authenticates like the old `RevelFactory.CreateShoreditchGrindHttpClient`, fetches JSON, validates status, deserializes into domain DTOs.
  - `IFourthSoapClient`: wraps Login/GetSessionID/SubmitSales behavior against the Fourth SOAP service.
  - `IFourthSalesXmlBuilder`: creates Fourth XML using the old `FourthClient.GenerateFourthHeaderForSales` and `ConvertToXMLDoc` behavior as the baseline.
  - Config/options for Revel, Fourth, establishment mappings, start hour, dry-run mode, retry policy, and logging destinations.

- `RevelFourthPipeline.Worker` (`net10.0`)
  - A Worker Service hosted on the .NET generic host.
  - Quartz or `BackgroundService` scheduling for the nightly run.
  - Command-line/backfill mode for one date range and one establishment/brand.

- `RevelFourthPipeline.Tests` (`net10.0`)
  - Unit tests for URL/date generation, Revel JSON deserialization, mapping/reconciliation, XML output, and SOAP client behavior via mocks.
  - Golden-file XML tests from known sample operations JSON.

If the implementation is added under `automatedFourthPipeline/service.pipeline.fourth.com`, keep the new SOAP pipeline names explicit so it is not confused with the existing Fourth Sales API/CSV code.

## Build-Critical Pull Details

The Revel pull side is driven by a dynamic base URL and an API key/secret.

- Required config per integration:
  - `RevelBaseUrl` or `RevelupUrl`: for example `https://{tenant}.revelup.com/`; this is the key host used to build the operations-report URL.
  - `RevelApiKeySecret`: the value sent in the `API-AUTHENTICATION` header.
  - `RevelEstablishmentId`: the Revel establishment id passed as `establishment={id}`.
  - Optional local ids such as store id, brand id, or DB primary keys must not be substituted for `RevelEstablishmentId`.
- Required request headers, based on `RevelFactory.CreateShoreditchGrindHttpClient`:
  - `Accept: application/json`
  - `API-AUTHENTICATION: {RevelApiKeySecret}`
  - `Referer: {RevelBaseUrl}`
- Required URL construction:
  - Trim trailing slash from `RevelBaseUrl`, then append `/reports/operations/json/`.
  - Keep these query flags: `show_opened=1`, `show_unpaid=1`, `show_irregular=1`.
  - Set `range_from={start.ToRevelDate()}`, `range_to={end.ToRevelDate()}`, `establishment={RevelEstablishmentId}`.
  - Use `UriBuilder` or query helpers; do not string-concatenate credentials or unescaped values into URLs.
- Required source DTOs:
  - `RootObject.product_mix_data`
  - `RootObject.sales_data`
  - `RootObject.tax_data`
  - `RootObject.discounts_data`
  - `RootObject.voids_data`
- Key operations fields for mapping/reconciliation:
  - Product rows: `product_sku`, `product_name`, `product_description`, `product_category`, `product_class`, `n_items`, `price`, `tax`, `taxable_sales`, `untaxable_sales`, `discount`, `order_discount`, `voids_amount_total`, `row_type`.
  - Sales totals: `gross_sales`, `net_sales`, `total_sales`, `sales_tax`, `total_orders`.
  - Tax rows: `name`, `taxable_sales`, `tax`, `sales`, `tax_rate`.

## Build-Critical Push Details

The Fourth push side comes from the legacy SOAP webservice, not the newer CSV/API code.

- SOAP WSDL/reference:
  - `Grind/Revel.808nd.com/Service References/fhAPI/fhapi.wsdl`
  - Equivalent generated references also exist under `BluebirdFourth/client.fourth.com/Service References/fhAPI`.
- SOAP endpoint and namespace:
  - Endpoint: `http://ws.fourthhospitality.com/fhapi.asmx`
  - Target namespace: `http://ws.fourthhospitality.com/`
  - Legacy binding: `basicHttpBinding` with endpoint name `fhAPISoap`.
- Required SOAP flow:
  - `Login(userName, password)` returns `AuthenticationHeader`.
  - `AuthenticationHeader` contains `SessionID`.
  - `SubmitSales(AuthenticationHeader, XmlNode sales)` returns `SubmitSalesResult` as `double`.
  - Treat returned `0` as a failure/danger condition, matching the legacy Bluebird behavior.
- Required Fourth sales XML DTOs:
  - Root: `FourthHeader`
  - Header: `FourthHeaderOrganisationHeader`
  - Sales header: `FourthHeaderOrganisationHeaderSalesHeader`
  - Transaction: `FourthHeaderOrganisationHeaderSalesHeaderSalesTransaction`
- Required Fourth XML fields:
  - Organisation: `OrganisationID`, `UserName`, `Password`
  - Sales header: `SalesDate`, `Location`, `RevenueCentre`, `ActionIfDataExists`
  - Sales transaction: `PLU`, `Description`, `Quantity`, `VAT`, `TotalGrossSales`, `NetSalesPrice`, `GrossSalesPrice`, `TotalNetSales`, optional `CategoryCode`, `SaleType`
- XML generation behavior to preserve:
  - Serialize the `FourthHeader` using `XmlSerializer`.
  - Set the root `xmlns` attribute to empty.
  - Strip generated namespaces before submitting.
  - Submit the XML document node as the `sales` body member.
- Required Fourth config per integration:
  - Fourth username/password.
  - Organisation/unit/location id.
  - Site location code if pushing per establishment.
  - Revenue centre.
  - Push mode: per establishment is the behavior used by the active Bluebird path.

## Suggested Configuration Shape

Keep secrets out of source. Use user secrets, environment variables, Key Vault, or deployment secrets for real values.

```json
{
  "RevelFourthPipeline": {
    "DryRun": true,
    "BusinessDayStartHour": 4,
    "Revel": {
      "BaseUrl": "https://{tenant}.revelup.com/",
      "ApiKeySecret": "{secret}",
      "TimeoutSeconds": 600
    },
    "Fourth": {
      "SoapEndpoint": "http://ws.fourthhospitality.com/fhapi.asmx",
      "Username": "{secret}",
      "Password": "{secret}",
      "OrganisationId": "{unit-or-location-id}",
      "DefaultLocation": "{location-code}",
      "DefaultRevenueCentre": "1"
    },
    "Stores": [
      {
        "Name": "{store-name}",
        "RevelEstablishmentId": 1,
        "FourthLocation": "{location-code}",
        "FourthRevenueCentre": "1",
        "Active": true
      }
    ]
  }
}
```

## .NET 10 Build Notes

- The machine has a .NET 10 SDK available, so target `net10.0`.
- For a Worker Service, use the .NET generic host and either Quartz hosted integration or a simple `BackgroundService`.
- For HTTP pull code, use `IHttpClientFactory`.
- For SOAP, prove the client early using the WSDL in `Service References/fhAPI/fhapi.wsdl`. Expected packages are in the `System.ServiceModel.*` client family; use generated code or a small typed wrapper that can call `Login` and `SubmitSales`.
- For XML, use `System.Xml`, `System.Xml.Serialization`, and the ported `XmlStripper`/namespace-removal behavior.
- For tests, prefer fixture-based tests and mock HTTP/SOAP clients; live Revel/Fourth calls must be opt-in.

## Implementation Plan

1. Baseline the contracts.
   - Copy sample operations JSON from Revel test fixtures or capture sanitized fixtures.
   - Identify the exact Fourth SOAP endpoint/WSDL used by `fhAPI`.
   - Confirm whether Fourth wants one push per brand or per establishment. The legacy Bluebird job currently executes establishment-based pushes when active establishments exist.

2. Scaffold the .NET 10 projects.
   - Add a solution and the Domain, Infrastructure, Worker, and Tests projects.
   - Use nullable reference types and dependency injection from the start.
   - Add configuration through `IOptions<T>`. Do not copy hard-coded connection strings, passwords, API keys, or certificate bypass code from legacy projects.

3. Port the Revel pull.
   - Port `ToRevelDate()` exactly: `yyyy-MM-ddTHH:mm:ss`.
   - Treat the Revel base URL as dynamic configuration. Do not hard-code `shoreditchgrind.revelup.com`; that is only the legacy Grind tenant example.
   - Build the operations endpoint with the required flags: `show_opened=1`, `show_unpaid=1`, `show_irregular=1`.
   - Pull by establishment id using the establishment's Revel `establishment_id`, not the local DB primary key.
   - Preserve the nightly business range semantics. The old Fourth push uses `04:00` to `04:00`; the operations-report automated jobs sometimes use explicit start/end inputs. Make the start hour configurable.
   - Add retry with bounded attempts, non-success response logging, and raw response capture for failed deserializations.

4. Port the supporting models.
   - Bring across only the DTOs needed for operations reports and Fourth sales XML.
   - Use `System.Text.Json` with explicit property names unless Newtonsoft-specific behavior is required.
   - Keep money as `decimal` where possible. Some legacy DTOs use strings for numeric fields; parse through explicit helper methods that fail loudly or record field-level errors.

5. Build the Revel operations to Fourth mapping.
   - Map each relevant `product_mix_data` row into normalized transaction drafts.
   - Decide and document the money semantics before submitting:
     - likely inputs are `price`, `tax`, `taxable_sales`, `untaxable_sales`, `discount`, `order_discount`, and `voids_amount_total`;
     - legacy Fourth XML uses `TotalNetSales`, `VAT`, `TotalGrossSales`, `NetSalesPrice`, and `GrossSalesPrice`.
   - Reconcile totals from the source JSON against generated XML totals for every establishment/date before allowing a live push.
   - Preserve the legacy fallback behavior for missing/blank SKUs, but validate the Fourth PLU contract because the generated XML model marks PLU as a non-negative integer.

6. Port the Fourth SOAP push.
   - Regenerate a .NET-compatible SOAP client from `fhapi.wsdl` or create a small typed wrapper using supported `System.ServiceModel.*` client packages.
   - Preserve the old flow: authenticate/login, build the `AuthenticationHeader`, submit XML to `SubmitSales`, capture the returned numeric code.
   - Port `SalesSubmission.cs` XML shape and namespace-stripping behavior from `FourthClient.ConvertToXMLDoc`.
   - Keep request XML and SOAP response metadata in logs for audit/debugging, but do not log plaintext Fourth credentials.

7. Add orchestration and idempotency.
   - Worker loads active integrations/establishments, computes the business date range, pulls operations data, maps to XML, validates totals, then either dry-runs or submits.
   - Add a run ledger keyed by source system, establishment id, business date, range start/end, and payload checksum.
   - Prevent accidental duplicate live submissions unless explicitly run with a force/backfill flag.
   - Continue processing other establishments when one establishment fails, but mark the overall run as degraded.

8. Add observability.
   - Structured logs for pull start/end, HTTP status, deserialization result, mapped row count, totals, XML generation, SOAP result, and failure category.
   - Persist raw/sanitized payloads or payload hashes so nightly numbers are auditable.
   - Add alerts for empty sales on active establishments, total mismatches, SOAP return code `0`, and authentication failures.

9. Test before live traffic.
   - Unit tests:
     - date formatting and 04:00 business-day range generation;
     - operations URL generation;
     - JSON fixture deserialization;
     - operations-to-Fourth transaction mapping;
     - XML output shape and namespaces;
     - duplicate-run/idempotency behavior.
   - Integration tests:
     - Revel pull against a controlled date/establishment when credentials are available;
     - Fourth SOAP submit only behind an explicit live-test flag.
   - Dry-run comparison:
     - generate XML for historical dates and compare totals against Revel operations report totals before enabling live submit.

10. Roll out in phases.
    - Phase 1: new project builds, fixtures pass, XML generation only.
    - Phase 2: dry-run nightly for one establishment with persisted logs and reconciliation.
    - Phase 3: live submit for one establishment/date with manual verification in Fourth.
    - Phase 4: enable all active establishments.
    - Phase 5: add backfill tooling and remove any remaining hard-coded date windows.

## Known Risks And Decisions

- The legacy Bluebird job is order-item based; the requested new source is the Revel operations report JSON. The mapping must be validated financially rather than assumed equivalent.
- The old code disables certificate validation and forces legacy TLS through `ServicePointManager`. Do not port that behavior unless there is a separately approved operational exception.
- The old projects include hard-coded credentials and production connection strings in config files. Do not duplicate them in new code or docs.
- `PushToFourth3amJob.cs` currently has hard-coded backfill dates in `Execute`. The new worker must be schedule/config/CLI driven.
- SOAP support in .NET 10 should be proven early with a minimal login/submit client spike before broad porting.
- Decide whether Mongo persistence from `OpsReportHourlyWrapper` is still needed. If not, keep raw pull storage in the new run ledger/audit store instead.

## Done Criteria

- A .NET 10 solution builds with no dependency on .NET Framework projects.
- The new Revel operations pull reproduces the legacy URL/date behavior.
- The supporting operations-report and Fourth XML models are covered by fixture tests.
- Generated Fourth XML reconciles to source Revel totals for test dates.
- The SOAP client can authenticate and submit through a guarded live path.
- Nightly runs are idempotent, observable, and dry-run capable.
- No secrets, hard-coded production dates, or certificate-validation bypasses are introduced.
