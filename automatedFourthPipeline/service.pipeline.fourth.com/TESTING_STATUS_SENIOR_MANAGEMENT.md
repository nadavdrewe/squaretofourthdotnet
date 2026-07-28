# Square to Fourth Testing Status for Senior Management

Generated: 2026-07-07

## Executive Summary

The Square to Fourth integration has been tested end to end in the Square sandbox for both sales and hospitality/labor data. The latest readiness baseline is sandbox run `20260706235456`.

The verified flow is:

1. Seed realistic Square sandbox data.
2. Read the raw Square data back through Square APIs.
3. Transform the data into Fourth hospitality sales CSV and Fourth timesheet XML.
4. Run the worker in generate-only mode.
5. Persist generated payloads, run results, and structured stage/event logs to SQL audit records.

The current caveat is that live Fourth submission has not yet been executed. The Fourth upload code and OAuth login path are implemented, but production/pre-live Fourth credentials, scope, API base URL, and timesheet XML endpoint still need to be supplied before live upload can be enabled.

## Latest Test Window

| Area | Period Tested |
| --- | --- |
| Sales, payments, refunds | `2026-07-06T23:52:56.1038633Z` to `2026-07-07T00:11:49.2112519Z` |
| Staff clock-in/clock-out | `2026-06-22T07:00:00.0000000Z` to `2026-07-07T00:10:27.3577176Z` |
| Square sandbox location | `Default Test Account` / `L8WQDAS2AGWZC` |
| Readiness run ID | `20260706235456` |

## Coverage Tested

| Category | Raw Square Test Data | Verified Fourth Output |
| --- | ---: | ---: |
| Products | 20 catalog products | 20 Fourth product PLUs |
| Modifiers | 20 product modifiers | 20 modifier descriptions |
| Completed sales transactions | 10 paid orders | 10 `TAB_OPEN` and 10 `TAB_CLOSE` transaction pairs |
| Payments | 10 completed payments | 10 positive `TENDER` rows |
| Payment types | 8 card, 1 cash, 1 external | `CARD`, `CASH`, `EXTERNAL` |
| Refunds | 3 linked refunds | 3 negative `TENDER` rows |
| Refund types | 1 card, 1 cash, 1 external | `CARD_REFUND`, `CASH_REFUND`, `EXTERNAL_REFUND` |
| Open/unpaid orders | 2 deliberately created | 0 included in Fourth sales output |
| Staff | 15 Square team members | 15 Fourth employee numbers |
| Shifts/timecards | 215 timecard records | 215 timesheet XML records |
| Closed shifts | 210 closed clock-in/out records | 210 closed Fourth rows |
| Open shifts | 5 open clock-in records | 5 open Fourth rows |
| Clock-in dates | 15 dates | 15 dates |

## Sales Output Detail

The clean filtered replay for run `20260706235456` produced `144` Fourth sales CSV rows.

| Fourth CSV Row Type | Rows |
| --- | ---: |
| `TAB_OPEN` | 10 |
| `SALES_ITEM` | 43 |
| `MODIFIER_ITEM` | 36 |
| `DISC_ITEM` | 18 |
| `SERVICE_CHARGE` | 14 |
| `TENDER` | 13 |
| `TAB_CLOSE` | 10 |

Financial tender checks:

| Measure | Value |
| --- | ---: |
| Positive tender rows | 10 |
| Positive tender total | `823.69` |
| Negative refund tender rows | 3 |
| Negative refund total | `-14.50` |

The sales test includes tips, fixed discounts, percentage discounts, service charges, line-level tax, retail/voucher products, modifiers, open/unpaid order exclusion, multiple payment methods, and refunds.

## Hospitality and Labor Output Detail

The clean filtered replay produced `215` Fourth timesheet XML records.

| Timesheet Measure | Count |
| --- | ---: |
| Fourth employees | 15 |
| Clock-in dates | 15 |
| Closed clock-in/out rows | 210 |
| Open clock-in rows | 5 |
| Total XML records | 215 |

Employee mapping was verified through SQL-backed Square-to-Fourth mappings, with Square team members mapped to `SANDBOX-EMP-01` through `SANDBOX-EMP-15`.

## APIs Used

| System | API/Area | Used For |
| --- | --- | --- |
| Square | Locations API | Resolve the Square store/location under test. |
| Square | Catalog API | Read products, variations, categories, and modifiers. |
| Square | Orders API | Create/read orders and identify completed versus open/unpaid orders. |
| Square | Payments API | Create/read card, cash, and external payments. |
| Square | Refunds API | Create/read linked payment refunds. |
| Square | Team Members API | Create/read team members used for hospitality/labor mapping. |
| Square | Labor API | Create/read timecards for clock-in/clock-out testing. |
| Fourth | OAuth 2.0 token endpoint | Implemented and tested for Fourth login/token handling. |
| Fourth | Sales CSV upload client | Implemented, but not yet live-submitted pending Fourth credentials/endpoints. |
| Fourth | Timesheet XML upload client | Implemented, but not yet live-submitted pending Fourth credentials/endpoints. |
| SQL Server | Pipeline audit tables | Persist generated CSV/XML payloads, row counts, statuses, windows, and file paths. |
| SQL Server | Pipeline event log table | Persist structured job, brand, store, API-read, transform, file, upload, and failure events. |

## Raw Square Data vs Generated Fourth Files

Raw Square validation reads the sandbox data directly through the Square APIs for the recorded windows and order IDs. This confirms the source data exists in Square and that the integration can still retrieve old data after it has been seeded.

Generated Fourth validation transforms that raw Square data into:

- Fourth hospitality sales CSV.
- Fourth timesheet XML.
- SQL `PipelineRunRecords` rows containing payload text, row counts, status, output file path, and data window.
- SQL `PipelineEventLogs` rows containing structured stage-level read/transform/file/upload/failure events and count summaries.

The clean filtered replay is the sign-off baseline because it isolates the successful run ID and order IDs. The worker generate-only run uses a broader date/location window, which is closer to production behavior. That broader worker run intentionally picked up nearby failed sandbox attempts and additional historical timecards, so its counts are larger.

| Test Layer | Sales Output | Labor Output | Notes |
| --- | ---: | ---: | --- |
| Filtered replay baseline | 144 CSV rows | 215 XML records | Clean sign-off view for run `20260706235456`. |
| Worker generate-only run | 153 CSV rows | 1296 XML records | Full location/date window; persisted to SQL audit rows. |

Worker generate-only SQL audit records:

| Data Type | SQL Run Record | Status | Row Count |
| --- | ---: | --- | ---: |
| Sales CSV | 10 | `Generated` | 153 |
| Timesheet XML | 9 | `Generated` | 1296 |

Structured DB event logging verification:

| Event Log Measure | Count |
| --- | ---: |
| Total event rows | 22 |
| Failed event rows | 0 |
| Square read events | 4 |
| Transform events | 2 |
| File output events | 2 |
| Run-record persistence events | 2 |

## Test Artifacts

Filtered replay artifacts:

- Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706235456_PaymentRefundReplayFourthCsv.csv`
- Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706235456_PaymentRefundReplayFourthTimesheets.xml`

Worker generate-only artifacts:

- Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthHospitalitySales_2026_07_07_001232.csv`
- Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthTimesheets_2026_07_07_001229.xml`

## Technical Verification Completed

| Check | Result |
| --- | --- |
| Payment/refund sandbox seed test | Passed |
| Payment/refund read-only replay test | Passed |
| Worker generate-only run | Passed |
| Structured DB event logging | Passed |
| Full solution test suite | Passed |
| Vulnerable package scan | No vulnerable packages reported |
| Full solution build | Passed with 14 existing warnings and 0 errors |

## Current Readiness Position

The Square read, transformation, worker generation, SQL persistence, product/modifier coverage, payment/refund coverage, and hospitality/labor coverage are ready for a Fourth endpoint validation run.

Before this can be called live end to end, the project still needs:

- Fourth production or pre-live `ClientId`.
- Fourth `ClientSecret`.
- Fourth OAuth scope.
- Fourth API base URL.
- Fourth sales endpoint confirmation.
- Fourth timesheet XML endpoint confirmation.
- SMTP details if failure email alerts should be enabled.
- A narrow live Fourth upload test window reviewed against Fourth-side receipt/validation logs.
