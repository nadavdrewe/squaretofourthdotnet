# Project Plan

## Current Objective

Finish the Square to Fourth update on .NET 10 with reproducible sandbox coverage for sales and hospitality/labor data.

## Current Status

- .NET 10 solution builds: `dotnet build service.pipeline.fourth.com.sln --no-restore` passed on 2026-07-07 with `14` existing warnings and `0` errors.
- Square sandbox read replay is passing for payment/refund readiness run `20260706235456`.
- Verified Square-to-Fourth generated payload coverage:
  - Filtered replay sales CSV rows: `144`.
  - Positive payment/tender rows: `10`.
  - Negative refund/tender rows: `3`.
  - Positive tender types: `CARD`, `CASH`, `EXTERNAL`.
  - Refund tender types: `CARD_REFUND`, `CASH_REFUND`, `EXTERNAL_REFUND`.
  - Unique Fourth product PLUs: `20`.
  - Unique Fourth modifier descriptions: `20`.
  - Discount rows: `18`.
  - Service-charge rows: `14`.
  - Timesheet XML records: `215`.
  - Distinct Fourth employees: `15`.
  - Distinct clock-in dates: `15`.
  - Closed clock-in/out rows: `210`.
  - Open clock-in rows: `5`.
- Latest replay artifacts:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706235456_PaymentRefundReplayFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706235456_PaymentRefundReplayFourthTimesheets.xml`
- Latest full-project verification on 2026-07-07:
  - `dotnet build service.pipeline.fourth.com.sln --no-restore` passed with `14` existing warnings and `0` errors.
  - `dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"` passed for the default suite.
  - Payment/refund Square seed test passed when run explicitly.
  - Payment/refund Square replay test passed when run explicitly.
  - `dotnet list service.pipeline.fourth.com.sln package --vulnerable --include-transitive` reported no vulnerable packages.
- DB audit logging is implemented through `PipelineRunRecords`; worker runtime SQL logging is enabled through Serilog.
- Structured DB event logging is implemented through `PipelineEventLogs` and visible at `/PipelineEventLogs/Index`.
- Latest DB logging verification on 2026-07-07:
  - Migration `20260707010423_pipelineEventLogs` applied to the configured SQL database.
  - Table `PipelineEventLogs` exists with `2` expected indexes.
  - Controlled worker generate-only run wrote `22` structured event rows with `0` failures.
  - New run records from that verification: `TimesheetXml` row `9` with `1296` rows and `SalesCsv` row `10` with `153` rows.
- Worker sales CSV upload is guarded by `SquareToFourthSales:UploadToFourth`; default `false` generates the CSV and stores a `Generated` run record without requiring Fourth login.
- Worker generate-only run against the remote SQL sandbox integration passed on 2026-07-07:
  - Remote SQL has `15` active payment/refund readiness employee mapping rows for store integration `1`.
  - Sales CSV `PipelineRunRecords` row `8`: `Generated`, `153` rows, payload stored in SQL.
  - Timesheet XML `PipelineRunRecords` row `7`: `Generated`, `1296` rows, payload stored in SQL.
  - Sales output: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthHospitalitySales_2026_07_07_001232.csv`
  - Timesheet output: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthTimesheets_2026_07_07_001229.xml`
  - Worker sales rows include nearby failed sandbox attempts in the same Square window; filtered replay is the clean successful-run baseline.
  - Worker timesheet XML intentionally includes all Square timecards in the configured location window; the `15` intended readiness Fourth employees are present.
- Failure email alerts are wired to `nadavdrewe@gmail.com`, but `PipelineAlerts:Enabled` remains `false` until SMTP host/from/credentials are supplied.
- Remaining operational caveat: the sandbox replay verifies Square read -> Fourth CSV/XML generation, but does not submit the replay artifacts to a live Fourth endpoint.

## Verification Commands

```powershell
dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxReplayTests.ReplayExpandedSandboxSeed_ThenVerifyFourthSpreadsheetPayloads" --logger "console;verbosity=detailed" --no-build
dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxReplayTests.ReplayReadinessSandboxSeed_ThenVerifyFourthPayloadCoverage" --logger "console;verbosity=detailed" --no-build
dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxReplayTests.ReplayPaymentRefundSandboxSeed_ThenVerifyTenderAndRefundCoverage" --logger "console;verbosity=detailed" --no-build
dotnet build service.pipeline.fourth.com.sln --no-restore
dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"
dotnet list service.pipeline.fourth.com.sln package --vulnerable --include-transitive
```

## Sandbox Seed Log

Record every intentional Square sandbox seed run here so old data can be found again by absolute date and run ID.

### 2026-07-06 Payment/Refund Product/Labor Seed

- Run ID: `20260706235456`.
- Sandbox location: `Default Test Account` (`L8WQDAS2AGWZC`).
- Sales order/payment/refund window UTC: `2026-07-06T23:52:56.1038633Z` to `2026-07-07T00:11:49.2112519Z`.
- Labor timecard window UTC: `2026-06-22T07:00:00.0000000Z` to `2026-07-07T00:10:27.3577176Z`.
- Seeded catalog products and modifiers:
  - Burger / Regular + Cheese
  - Fries / Large + Truffle Salt
  - Coffee / Flat White + Oat Milk
  - Tea / Breakfast + Honey
  - Pasta / Rigatoni + Parmesan
  - Salad / Caesar + Chicken
  - Steak / Sirloin + Peppercorn Sauce
  - Dessert / Tiramisu + Birthday Plate
  - Juice / Orange + Ginger Shot
  - Wine / House Red + Large Glass
  - Eggs / Poached + Smoked Salmon
  - Pancakes / Stack + Maple Syrup
  - Granola / Bowl + Greek Yoghurt
  - Soup / Tomato + Sourdough
  - Fish / Sea Bass + Lemon Butter
  - Cocktail / Negroni + Premium Gin
  - Beer / Pint + Lime
  - Merch / Tote Bag + Gift Wrap
  - Voucher / Gift Card + Envelope
  - Water / Sparkling + Ice
- Sales edge cases covered: card payments, cash payments, external payments, linked card refunds, linked cash refunds, linked external refunds, tips, fixed discounts, percentage discounts, service charges, additive tax on line items, retail/voucher lines, open/unpaid orders excluded from generated Fourth sales.
- Paid orders and payments:
  - Order `G113HdDDldfznmp3J0X8MkwW6hcZY`; payment `dIx222jzmOy8BRm6mAaWeZ9dISWZY`.
  - Order `48aAmWT7DsfjEZANXuhYj6r2nMKZY`; payment `z5a7Nf8TQnvCppCH51ZUPZo6L3dZY`.
  - Order `kkbuzwmd4e6FUxQXz2bUnbokucPZY`; payment `TdQLaLBdmiqtezHGhrJjW8Vxy8RZY`.
  - Order `ip0z44BYGl4RNjnsi5SktJHMMuCZY`; payment `LvPHfdVKGj3kw44fkzE5vpJOtvZZY`.
  - Order `Sr7SxbohMegyAO8rEoxcVuFuQkTZY`; payment `3P4NpFPigUZ38yAuBgQj2xS5h3CZY`.
  - Order `MKQGepZRW7ONqpQ5GlfeiDKX788YY`; payment `L7n60vWk5BskPatJ7P4GkqvNPlRZY`.
  - Order `CBycgHdJrPbvc3OsihZXe2lqvwWZY`; payment `LrnCogQNGQDoIJ3YAibpnHC0yHAZY`.
  - Order `Iyn15D7kIs7OrxY5NljrJr1KM3eZY`; payment `xgUUE32b91gWVHXdmBugUl1HVIVZY`.
  - Order `2r60qaf2LSphJfdEjI6wsWW2yTGZY`; payment `FY5dRxzxbDVgGLpZKTCVAPqLe05YY`; tender source `CASH`.
  - Order `6ThaezMo5n8WzTELHidQVSW4IfPZY`; payment `vVfAWfl1H5pqnccV3oFRRP8Ppi9YY`; tender source `EXTERNAL`.
- Linked refunds:
  - Payment `dIx222jzmOy8BRm6mAaWeZ9dISWZY`; refund `dIx222jzmOy8BRm6mAaWeZ9dISWZY_PwPYHxE359DWOdtfnJDufxsSbpf5SJ0xmE914YsT8BH`; amount `2.50`; status `PENDING`; mapped to `CARD_REFUND`.
  - Payment `FY5dRxzxbDVgGLpZKTCVAPqLe05YY`; refund `FY5dRxzxbDVgGLpZKTCVAPqLe05YY_X37MLZ2UoAHBUOidWdM3Thw5Ax9UKOZR4gXYs1sOiQK`; amount `5.00`; status `PENDING`; mapped to `CASH_REFUND`.
  - Payment `vVfAWfl1H5pqnccV3oFRRP8Ppi9YY`; refund `vVfAWfl1H5pqnccV3oFRRP8Ppi9YY_l7YFO8nvzXHC0iQDouIxrnLb4QnzTnrLMv3CGbs3OLN`; amount `7.00`; status `PENDING`; mapped to `EXTERNAL_REFUND`.
- Square sometimes returns a refund `OrderId` that differs from the original order. The replay matches refunds to orders by `OrderId` or by refund `PaymentId` against the order's tender payment IDs.
- Ignored open/unpaid orders:
  - `8i8KtvGvViFnEI49Ytg6ZkxpyQZZY`
  - `qByteD5Dm9Y9sAXRnff08RjBACJZY`
- Team members:
  - `TMyZhi71qSX9iBzF` -> `SANDBOX-EMP-01`
  - `TMA3pOJ02_tWK7f3` -> `SANDBOX-EMP-02`
  - `TMAhsqZ8m44pPnL9` -> `SANDBOX-EMP-03`
  - `TMxLvjlDVdfebR0n` -> `SANDBOX-EMP-04`
  - `TM0-YdS5k-7eG68t` -> `SANDBOX-EMP-05`
  - `TM4836VzBN7pyeYe` -> `SANDBOX-EMP-06`
  - `TM_8g6ZL2O_GrNKa` -> `SANDBOX-EMP-07`
  - `TMFZSGeacfeJ2yQc` -> `SANDBOX-EMP-08`
  - `TMAD99yhvJrDifMc` -> `SANDBOX-EMP-09`
  - `TMLNWiW_aPyAR7mw` -> `SANDBOX-EMP-10`
  - `TM6dTrt7WDXGNXcx` -> `SANDBOX-EMP-11`
  - `TMVvPK_XradnxFRg` -> `SANDBOX-EMP-12`
  - `TMTVMbWTq4Wiz2pQ` -> `SANDBOX-EMP-13`
  - `TMHXnABOL3qHtPEZ` -> `SANDBOX-EMP-14`
  - `TMiCZpb9_Lrm_uLe` -> `SANDBOX-EMP-15`
- Timecards: `210` closed timecards plus `5` open timecards, covering `15` employees across `15` clock-in dates in the filtered replay.
- Generated Fourth artifacts:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-seed\20260706235456_SquareSandboxFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-seed\20260706235456_SquareSandboxFourthTimesheets.xml`
- Read-only replay artifacts:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706235456_PaymentRefundReplayFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706235456_PaymentRefundReplayFourthTimesheets.xml`
- Filtered replay artifact verification:
  - Fourth sales CSV rows: `144`.
  - Transaction row counts: `TAB_OPEN=10`, `SALES_ITEM=43`, `MODIFIER_ITEM=36`, `DISC_ITEM=18`, `SERVICE_CHARGE=14`, `TENDER=13`, `TAB_CLOSE=10`.
  - Positive tender rows: `10`.
  - Negative refund tender rows: `3`.
  - Positive tender types: `CARD`, `CASH`, `EXTERNAL`.
  - Refund tender types: `CARD_REFUND`, `CASH_REFUND`, `EXTERNAL_REFUND`.
  - Positive tender total: `823.69`.
  - Negative refund total: `-14.50`.
  - Unique Fourth product PLUs in sales rows: `20`.
  - Unique Fourth modifier descriptions: `20`.
  - Timesheet XML records: `215`.
  - Distinct Fourth employee numbers in XML: `15`.
  - Distinct clock-in dates in XML: `15`.
  - Closed/open clock rows: `210` closed, `5` open.
- Worker generate-only artifacts:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthHospitalitySales_2026_07_07_001232.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthTimesheets_2026_07_07_001229.xml`
  - SQL run rows: `SalesCsv` row `8`, `TimesheetXml` row `7`.
- Worker window verification:
  - Fourth sales CSV rows: `153`.
  - Transaction row counts: `TAB_OPEN=13`, `SALES_ITEM=43`, `MODIFIER_ITEM=36`, `DISC_ITEM=18`, `SERVICE_CHARGE=14`, `TENDER=16`, `TAB_CLOSE=13`.
  - Positive tender rows: `10`.
  - Negative refund tender rows: `6`.
  - Timesheet XML records: `1296`.
  - Intended mapped Fourth employee numbers present: `15`.
- Note: the seed and worker sales windows include nearby failed sandbox attempts, so full-window artifacts contain extra refund rows. The filtered replay for the successful paid order IDs is the clean readiness baseline.

### 2026-07-06 Readiness Product/Payment/Labor Seed

- Run ID: `20260706122223`.
- Sandbox location: `Default Test Account` (`L8WQDAS2AGWZC`).
- Sales order window UTC: `2026-07-06T12:20:23.9957074Z` to `2026-07-06T12:39:45.2131183Z`.
- Labor timecard window UTC: `2026-06-22T07:00:00.0000000Z` to `2026-07-06T12:37:54.5698547Z`.
- Seeded catalog products and modifiers:
  - Burger / Regular + Cheese
  - Fries / Large + Truffle Salt
  - Coffee / Flat White + Oat Milk
  - Tea / Breakfast + Honey
  - Pasta / Rigatoni + Parmesan
  - Salad / Caesar + Chicken
  - Steak / Sirloin + Peppercorn Sauce
  - Dessert / Tiramisu + Birthday Plate
  - Juice / Orange + Ginger Shot
  - Wine / House Red + Large Glass
  - Eggs / Poached + Smoked Salmon
  - Pancakes / Stack + Maple Syrup
  - Granola / Bowl + Greek Yoghurt
  - Soup / Tomato + Sourdough
  - Fish / Sea Bass + Lemon Butter
  - Cocktail / Negroni + Premium Gin
  - Beer / Pint + Lime
  - Merch / Tote Bag + Gift Wrap
  - Voucher / Gift Card + Envelope
  - Water / Sparkling + Ice
- Sales edge cases covered: paid orders, completed payments, tips, fixed discounts, percentage discounts, service charges, additive tax on line items, retail/voucher lines, open/unpaid orders excluded from generated Fourth sales.
- Paid orders in generated Fourth payload:
  - `QM8WZgzBetTZEg6UzPDEHw5VJpJZY`
  - `qxQYQyJnonfT06EnGTU0pKRFWpZZY`
  - `QImKgsN26trPW3mj2Dwclf90nYLZY`
  - `WPMOrrBqTrEBz6TLnxaJTevLt1TZY`
  - `iD5XUonAA2fW0D6ZJTtZnAiyWORZY`
  - `qdk9zgg68C8IUTmtlWekMrb0ZZMZY`
  - `82u6Z26fbroF5Jxwu0gQTRI3GGJZY`
  - `sugvLkOMEx0rdDFVsxV8wNkCYNAZY`
  - `ej8eJXDoKQdctngoYu2UAaPARTXZY`
  - `Cp0FL2It6ZUoElnvcL3zHkskCnFZY`
  - `61OwmDF6Nrz6Jtt7Bd91px8R51BZY`
  - `YGDY9THenlqIGSe7ULTtjVMft8IZY`
- Note: the sales window captured four completed paid orders from the immediately preceding validation-failure attempt plus eight paid orders from the successful readiness run, so the generated Fourth CSV contains `12` `TENDER` rows.
- Ignored open/unpaid orders:
  - `6d8n7nBKB2EwZqpFrEKdpRUYUUIZY`
  - `mT0Xr6JrJeIZ6KqQfrhwobVtXBbZY`
- Team members:
  - `TMYjZmf3yuCfXwNc` -> `SANDBOX-EMP-01`
  - `TMUx6Ek0JtzIaJki` -> `SANDBOX-EMP-02`
  - `TMFdnMUFblFitZlg` -> `SANDBOX-EMP-03`
  - `TMMLYzDPbOmMFTU5` -> `SANDBOX-EMP-04`
  - `TMZOQW6HkugugklC` -> `SANDBOX-EMP-05`
  - `TM5Spifr7kizoxY-` -> `SANDBOX-EMP-06`
  - `TMzIUTMg-nJIweI6` -> `SANDBOX-EMP-07`
  - `TMKk2lrw5vjXbwpY` -> `SANDBOX-EMP-08`
  - `TMFLHCweX2vkmtBP` -> `SANDBOX-EMP-09`
  - `TM_m4BTGpXkzsEH5` -> `SANDBOX-EMP-10`
  - `TMVYHsyUg2kWKQqb` -> `SANDBOX-EMP-11`
  - `TMZFM0U7kT3lofUC` -> `SANDBOX-EMP-12`
  - `TM3sL8Zbb5Cz-llR` -> `SANDBOX-EMP-13`
  - `TMsQhufBIE0zU2nO` -> `SANDBOX-EMP-14`
  - `TMC7dISTNqgNCXYU` -> `SANDBOX-EMP-15`
- Timecards: `210` closed timecards plus `5` open timecards, covering `15` employees across `15` clock-in dates.
- Generated Fourth artifacts:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-seed\20260706122223_SquareSandboxFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-seed\20260706122223_SquareSandboxFourthTimesheets.xml`
- Read-only replay artifacts:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706122223_ReadinessReplayFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260706122223_ReadinessReplayFourthTimesheets.xml`
- Artifact verification:
  - Fourth sales CSV rows: `207`.
  - Transaction row counts: `TAB_OPEN=12`, `SALES_ITEM=65`, `MODIFIER_ITEM=57`, `DISC_ITEM=30`, `SERVICE_CHARGE=19`, `TENDER=12`, `TAB_CLOSE=12`.
  - Tender total: `1168.71`.
  - Taxed sales rows: `2`.
  - Unique Fourth product PLUs in sales rows: `20`.
  - Unique Fourth modifier descriptions: `20`.
  - Timesheet XML records: `215`.
  - Distinct Fourth employee numbers in XML: `15`.
  - Distinct clock-in dates in XML: `15`.
  - Closed clock-in/out rows: `210`.
  - Open clock-in rows: `5`.

### 2026-07-04 Expanded Product/Payment/Labor Seed

- Run ID: `20260704232039`.
- Sandbox location: `Default Test Account` (`L8WQDAS2AGWZC`).
- Sales order window UTC: `2026-07-04T23:18:39.2726976Z` to `2026-07-04T23:36:29.9151453Z`.
- Labor timecard window UTC: `2026-06-27T07:00:00.0000000Z` to `2026-07-04T23:35:55.4340721Z`.
- Seeded catalog products and modifiers:
  - Burger / Regular + Cheese
  - Fries / Large + Truffle Salt
  - Coffee / Flat White + Oat Milk
  - Tea / Breakfast + Honey
  - Pasta / Rigatoni + Parmesan
  - Salad / Caesar + Chicken
  - Steak / Sirloin + Peppercorn Sauce
  - Dessert / Tiramisu + Birthday Plate
  - Juice / Orange + Ginger Shot
  - Wine / House Red + Large Glass
- Catalog variation IDs:
  - Burger: `THTDY5FDKRLYJVIVBC3TXWHU`
  - Fries: `P2KEORCEWKTA7GTGDR7X2PR6`
  - Coffee: `FTZKNIQ2VRHAKC3RZK5UIKQQ`
  - Tea: `IC56PRW3LYTAOGGZ2CY2V3SW`
  - Pasta: `KMMQAKUCBIXIPSXLUVWYJWDJ`
  - Salad: `JAOINKXC5YRMQMMMX43STB2V`
  - Steak: `2PD2PLHA4HS4L7KEYJLACQBL`
  - Dessert: `TS4AMTCYOB7N3S34U6IMB6YP`
  - Juice: `KZ5WXL5HPXNO2IKAAUYDNPIF`
  - Wine: `46APWMHUSXDAOOEASAVRREWW`
- Paid orders and payments:
  - Order `IYQQVjAr4VrfoM2iSPQFnMNosBMZY`; payment `Tj9SVeYhF18RncWlNPE4XNGkEvDZY`.
  - Order `QWPRZ1ybM0cO5U9w77xYykBLt6AZY`; payment `D1sdMvEprnQ8qcTMsaD9VgBpa68YY`.
  - Order `4gSfQCksBesWS5reVNdugbcmPW8YY`; payment `F0Pnmu7CqBJ9fArGxw7mF2SPS9TZY`.
  - Order `uzQImDCsa4vyr63EUWh6Atg6RLaZY`; payment `VuKOde6lz8cfgVqJZ8olUO4BeS8YY`.
- Artifact note: the sales window also captured four paid orders from the immediately preceding assertion-run seed attempt `20260704231906`, so the generated Fourth CSV contains `8` `TENDER` rows in total.
- Ignored open/unpaid order: `Qa3gRodfsSCblVvUmh9eH8qKqx5YY`.
- Team members:
  - `TMUgrlW3NkxhCC7A` -> `SANDBOX-EMP-01`
  - `TMR0GsrflLiBDAdG` -> `SANDBOX-EMP-02`
  - `TMrqvySaweUlI6nC` -> `SANDBOX-EMP-03`
  - `TMqPkaJzDQJ7XUwu` -> `SANDBOX-EMP-04`
  - `TMEpcOdQjMX1WgaO` -> `SANDBOX-EMP-05`
  - `TMDi6n5dYcWZwMqB` -> `SANDBOX-EMP-06`
  - `TMbcjlRxw3gqnDKJ` -> `SANDBOX-EMP-07`
  - `TMok6MwuanneQR2e` -> `SANDBOX-EMP-08`
  - `TMbnMAKm0athTYoO` -> `SANDBOX-EMP-09`
  - `TMFA51hfPp69f2N6` -> `SANDBOX-EMP-10`
- Timecards: `70` closed timecards, covering `10` employees across `7` dates.
- Generated Fourth artifacts:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-seed\20260704232039_SquareSandboxFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-seed\20260704232039_SquareSandboxFourthTimesheets.xml`
- Artifact verification:
  - Fourth sales CSV rows: `104`.
  - Transaction row counts: `TAB_OPEN=8`, `SALES_ITEM=28`, `MODIFIER_ITEM=26`, `DISC_ITEM=14`, `SERVICE_CHARGE=12`, `TENDER=8`, `TAB_CLOSE=8`.
  - Unique Fourth product PLUs in sales rows: `10`.
  - Unique Fourth modifier descriptions: `10`.
  - Timesheet XML records: `70`.
  - Distinct Fourth employee numbers in XML: `10`.
  - Distinct clock-in dates in XML: `7`.

## Implementation Log

### 2026-07-07 Structured DB Logging Slice

- Confirmed previous DB logging was partial:
  - `PipelineRunRecords` stored final generated/uploaded/skipped/failed payload outcomes.
  - Serilog wrote runtime log messages to SQL.
  - Missing piece was structured stage/event logging for job, brand, store, Square reads, transforms, files, uploads, and failures.
- Added `PipelineEventLog` model and `PipelineEventLogs` table.
- Applied migration `20260707010423_pipelineEventLogs` to the configured SQL database.
- `PipelineEventLogs` columns include:
  - correlation ID;
  - optional pipeline run record ID;
  - brand/store/store integration identifiers;
  - Square location and Fourth unit/location identifiers;
  - source/target systems;
  - data type, stage, event type, and status;
  - UTC window and transaction date;
  - item/row counts, duration, HTTP status, output file path, external reference;
  - message, details JSON, error text, and timestamps.
- Added indexes:
  - `IX_PipelineEventLogs_CorrelationId_WhenCreatedUTC`.
  - `IX_PipelineEventLogs_StoreIntegrationId_DataType_TransactionDate_WhenCreatedUTC`.
- Worker event logging now records:
  - job start/brand load/job completion/failure;
  - brand start, Square credential resolution, store integration load, Fourth OAuth login start/success/failure, Square locations/catalog/team reads, brand completion/failure;
  - store start and resolved data windows;
  - Square order/payment/refund read counts;
  - Square labor timecard read counts;
  - Fourth sales CSV row creation counts, including row-type and tender summaries;
  - Fourth timesheet XML row creation counts, including mapped employee/open/closed summaries;
  - CSV/XML file write events;
  - Fourth upload response status when uploads are enabled;
  - run-record persistence events linked to the `PipelineRunRecords` row ID.
- Added read-only admin screens:
  - `/PipelineEventLogs/Index`.
  - `/PipelineEventLogs/Details/{id}`.
- Added navigation link under `Fourth Logs`.
- Verification:
  - `dotnet build domain.pipeline.fourth.com\domain.pipeline.fourth.com.csproj --no-restore` passed with `0` warnings and `0` errors.
  - `dotnet build squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj --no-restore /nodeReuse:false` passed with `0` warnings and `0` errors.
  - `dotnet build web.pipeline.fourth.com\web.pipeline.fourth.com.csproj --no-restore /nodeReuse:false` passed with the existing `4` web warnings and `0` errors.
  - SQL verification confirmed `PipelineEventLogs` exists with both expected indexes.
  - Controlled worker generate-only run against payment/refund readiness windows wrote `22` event rows, `0` failed event rows, `4` Square read events, `2` transform events, `2` file events, and `2` run-record events.
  - Latest generated SQL run records from the DB logging check: `TimesheetXml` row `9` with `1296` rows and `SalesCsv` row `10` with `153` rows.

### 2026-07-07 Payment and Refund Coverage Slice

- Added Square refunds read support through `square.pipeline.fourth.com/Services/RefundsService.cs`.
- Extended `SquareLocationDataset` with `refundsForOrders` and updated `SquareToFourthCSVGenerator` to read Square refunds for the sales window.
- Refund matching now uses refund `OrderId` when available and falls back to matching refund `PaymentId` against each order's tender payment IDs because Square sandbox refund order IDs can differ from the original paid order ID.
- Added negative Fourth `TENDER` rows for refunds:
  - `CARD_REFUND`
  - `CASH_REFUND`
  - `EXTERNAL_REFUND`
- Updated positive tender mapping to prefer `Payment.SourceType`, so Square external payments produce `EXTERNAL` instead of falling back to `OTHER`.
- Expanded `SquareSandboxEndToEndSeedTests.SeedSandboxSquareData_ThenGenerateFourthCsv`:
  - Creates card, cash, and external paid orders.
  - Creates linked card, cash, and external refunds.
  - Asserts positive tender types and negative refund tender types separately.
  - Keeps the `20` product/modifier and `15` employee/timecard readiness coverage.
- Added explicit read-only replay coverage:
  - Test name: `SquareSandboxReplayTests.ReplayPaymentRefundSandboxSeed_ThenVerifyTenderAndRefundCoverage`.
  - Run ID: `20260706235456`.
  - Purpose: verify old Square sandbox payments, refunds, items/catalog, team members, and clock-in/out data can be read back from Square and regenerated into Fourth payloads without creating new Square records.
- Updated remote SQL sandbox store integration `1` with `15` active Square team member to Fourth employee number mappings for the payment/refund readiness employees.
- Ran the worker in generate-only mode against the payment/refund readiness windows with Fourth upload disabled:
  - Sales CSV `PipelineRunRecords` row `8`: `Generated`, `153` rows.
  - Timesheet XML `PipelineRunRecords` row `7`: `Generated`, `1296` rows.
  - Worker sales output: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthHospitalitySales_2026_07_07_001232.csv`.
  - Worker timesheet output: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-payment-refund-replay-v2\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthTimesheets_2026_07_07_001229.xml`.
  - Worker sales include nearby failed sandbox attempts in the same Square window; filtered replay is the clean successful-run baseline.
- Validation:
  - `dotnet build domain.pipeline.fourth.com\domain.pipeline.fourth.com.csproj --no-restore` passed on 2026-07-06.
  - `dotnet build tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --no-restore` passed on 2026-07-06.
  - `dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxEndToEndSeedTests.SeedSandboxSquareData_ThenGenerateFourthCsv" --logger "console;verbosity=minimal" --no-build` passed on 2026-07-06.
  - `dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxReplayTests.ReplayPaymentRefundSandboxSeed_ThenVerifyTenderAndRefundCoverage" --logger "console;verbosity=minimal" --no-build` passed on 2026-07-07.
  - `dotnet build squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj --no-restore` passed on 2026-07-07.
  - `dotnet build service.pipeline.fourth.com.sln --no-restore` passed with `14` existing warnings and `0` errors on 2026-07-07.
  - `dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"` passed on 2026-07-07.
  - `dotnet list service.pipeline.fourth.com.sln package --vulnerable --include-transitive` reported no vulnerable packages on 2026-07-07.

### 2026-07-06 Readiness Sandbox Coverage Slice

- Expanded `SquareSandboxEndToEndSeedTests.SeedSandboxSquareData_ThenGenerateFourthCsv` from the prior 10-product/10-employee coverage to a broader readiness data set.
- Product coverage now includes `20` catalog products and `20` modifiers across food, drinks, retail, vouchers, breakfast, and water/non-food edge cases.
- Sales/order coverage now includes completed payments, tips, fixed discounts, percentage discounts, service charges, additive tax on sales line items, and open/unpaid orders that must be excluded from Fourth sales output.
- Hospitality/labor coverage now creates `15` Square team members mapped to `SANDBOX-EMP-01` through `SANDBOX-EMP-15`.
- Timecard coverage now creates `210` closed clock-in/out rows across `14` historical days plus `5` open timecards, producing `215` Fourth timesheet XML records across `15` clock-in dates.
- Added explicit read-only replay coverage:
  - Test name: `SquareSandboxReplayTests.ReplayReadinessSandboxSeed_ThenVerifyFourthPayloadCoverage`.
  - Run ID: `20260706122223`.
  - Purpose: verify the seeded Square data can be read back from Square and regenerated into Fourth sales CSV/timesheet XML without creating new Square records.
- Generated/replayed artifact counts:
  - Sales CSV rows: `207`.
  - Transaction row counts: `TAB_OPEN=12`, `SALES_ITEM=65`, `MODIFIER_ITEM=57`, `DISC_ITEM=30`, `SERVICE_CHARGE=19`, `TENDER=12`, `TAB_CLOSE=12`.
  - Unique Fourth product PLUs: `20`.
  - Unique modifier descriptions: `20`.
  - Timesheet XML records: `215`.
  - Distinct Fourth employees: `15`.
  - Closed/open clock rows: `210` closed, `5` open.
- Updated remote SQL sandbox store integration `1` with `15` active Square team member to Fourth employee number mappings for the readiness employees.
- Ran the worker in generate-only mode against the readiness windows with Fourth upload disabled:
  - Sales CSV `PipelineRunRecords` row `4`: `Generated`, `207` rows.
  - Timesheet XML `PipelineRunRecords` row `3`: `Generated`, `436` rows.
  - Worker sales output: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-readiness-replay\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthHospitalitySales_2026_07_06_125958.csv`.
  - Worker timesheet output: `C:\Users\nadav\AppData\Local\Temp\fourth-worker-readiness-replay\2026_07_06_Square Sandbox Verification_Default Test Account_SquareFourthTimesheets_2026_07_06_125955.xml`.
  - Worker timesheets include all Square location timecards in the configured 14-day window; all `15` intended readiness Fourth employees are present.
- Validation:
  - `dotnet build tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --no-restore` passed on 2026-07-06.
  - `dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxEndToEndSeedTests.SeedSandboxSquareData_ThenGenerateFourthCsv" --logger "console;verbosity=minimal" --no-build` passed on 2026-07-06.
  - `dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxReplayTests.ReplayReadinessSandboxSeed_ThenVerifyFourthPayloadCoverage" --logger "console;verbosity=minimal" --no-build` passed on 2026-07-06.
  - `dotnet build service.pipeline.fourth.com.sln --no-restore` passed with `0` warnings and `0` errors on 2026-07-06.
  - `dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"` passed on 2026-07-06.
  - `dotnet list service.pipeline.fourth.com.sln package --vulnerable --include-transitive` reported no vulnerable packages on 2026-07-06.

### 2026-07-05 Fourth OAuth Login Slice

- Implemented documented Fourth OAuth 2.0 token login in `com.fourth.pipeline.pos`.
- Supported grant paths:
  - `client_credentials` when `ClientId`/`ClientSecret` are present.
  - Legacy `password` behavior for old username/password-only Fourth credentials.
  - `refresh_token` helper for APIs that return refresh tokens.
- Token endpoint behavior:
  - Explicit token endpoint can be supplied in `BaseCredential.SupplimentalData1`.
  - If omitted, the client derives `[ROOT]/oauth/connect/token` from the API base URL and strips `/api/...` where present.
- Fourth credential field conventions:
  - `BaseEndpoint`: Fourth API base URL.
  - `ClientId`: OAuth client ID.
  - `ClientSecret`: OAuth client secret.
  - `SupplimentalData1`: OAuth token endpoint override.
  - `SupplimentalData2`: OAuth scope.
  - `LatestAccessToken` / `RefreshToken`: stored after successful test/login.
- Updated worker live-upload login path to create `FourthApiService` from the full Fourth credential and persist returned Fourth tokens.
- Updated `/BaseCredentials/Create` and `/BaseCredentials/Edit` to include Fourth OAuth fields.
- Updated credential index/details to avoid rendering stored access tokens, refresh tokens, passwords, and client secrets in clear text.
- Added `FourthOAuthClientTests` covering:
  - `client_credentials` form body shape.
  - bearer token storage on successful login.
  - default Fourth root token endpoint derivation.
- Validation:
  - `dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~FourthOAuthClientTests" --logger "console;verbosity=minimal"` passed with `2` tests.
  - `dotnet build service.pipeline.fourth.com.sln --no-restore` passed with `0` warnings and `0` errors.
  - `dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"` passed.

### 2026-07-05 Worker Sandbox Generate-Only Verification Slice

- Seeded the remote SQL Server database with one active Square-to-Fourth sandbox integration for Square location `L8WQDAS2AGWZC`.
- Active remote setup now includes:
  - Brand: `Square Sandbox Verification`.
  - Store: `Default Test Account`.
  - Store integration: Square-to-Fourth POS sales, active.
  - Square credential: static sandbox access token with sandbox Square base endpoint stored on the credential.
  - Fourth store config: `SANDBOX_UNIT`, site/location code `L8WQDAS2AGWZC`.
  - Employee mappings: `10` active rows mapping the expanded seed team members to `SANDBOX-EMP-01` through `SANDBOX-EMP-10`.
- Updated worker Square endpoint resolution:
  - `SquareApi:BaseUrl` config wins when supplied.
  - `SquareSandbox:BaseUrl` config is supported for local sandbox runs.
  - The active Square credential `BaseEndpoint` is used as the DB-backed fallback.
- Updated worker host configuration to load optional `appsettings.Local.json`, matching the sandbox test configuration pattern.
- Added `SquareToFourthSales:RunOnStartup` so a scheduled worker job can be triggered immediately for controlled verification/backfill runs.
- Ran the worker against the recorded sandbox windows with both Fourth uploads disabled:
  - Sales window UTC: `2026-07-04T23:18:39.2726976Z` to `2026-07-04T23:36:29.9151453Z`.
  - Timesheet window UTC: `2026-06-27T07:00:00.0000000Z` to `2026-07-04T23:35:55.4340721Z`.
- Worker output verification:
  - Sales CSV rows: `104`.
  - Transaction row counts: `TAB_OPEN=8`, `SALES_ITEM=28`, `MODIFIER_ITEM=26`, `DISC_ITEM=14`, `SERVICE_CHARGE=12`, `TENDER=8`, `TAB_CLOSE=8`.
  - Unique Fourth product PLUs: `10`.
  - Unique modifier descriptions: `10`.
  - Tender total: `406.04`.
  - Timesheet XML records: `221`.
  - Distinct employees in worker XML: `41`.
  - Intended mapped Fourth employees present: `10`.
  - Distinct clock-in dates: `8`.
- SQL persistence verification:
  - `PipelineRunRecords` row `1`: `TimesheetXml`, `Generated`, `221` rows, XML payload length `60927`.
  - `PipelineRunRecords` row `2`: `SalesCsv`, `Generated`, `104` rows, CSV payload length `51015`.
- Validation after this slice:
  - `dotnet build service.pipeline.fourth.com.sln --no-restore` passed with `0` warnings and `0` errors.
  - `dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"` passed.
  - `dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxReplayTests.ReplayExpandedSandboxSeed_ThenVerifyFourthSpreadsheetPayloads" --logger "console;verbosity=minimal" --no-build` passed.
  - `dotnet list service.pipeline.fourth.com.sln package --vulnerable --include-transitive` reported no vulnerable packages.

### 2026-07-05 Worker Sales Generate-Only Slice

- Added `SquareToFourthSales:UploadToFourth` config flag.
- Default is `false` in `squareservice.pipeline.fourth.com/appsettings.json`.
- Worker behavior when sales upload is disabled:
  - Reads Square orders/payments/catalog/team data as before.
  - Generates the Fourth sales CSV file.
  - Writes a `PipelineRunRecords` row with `DataType=SalesCsv` and `Status=Generated`.
  - Skips Fourth login and `SendSalesDataToFourth`, so current sandbox/static-token setups can exercise the worker path without live Fourth credentials.
- Worker behavior when sales upload is enabled:
  - Requires active Fourth credentials.
  - Logs in to Fourth and uploads sales CSV as before.
- Timesheet XML upload remains guarded separately by `SquareToFourthTimesheets:UploadToFourth`.

### 2026-07-05 Final Documentation and Verification Pass

- Re-ran expanded Square sandbox replay test against old Square data for run `20260704232039`; test passed.
- Rechecked generated replay payloads:
  - Sales CSV rows: `52`.
  - Transaction row counts: `TAB_OPEN=4`, `SALES_ITEM=14`, `MODIFIER_ITEM=13`, `DISC_ITEM=7`, `SERVICE_CHARGE=6`, `TENDER=4`, `TAB_CLOSE=4`.
  - Tender total: `203.02`.
  - Unique Fourth product PLUs: `10`.
  - Unique modifier descriptions: `10`.
  - Timesheet XML records: `70`.
  - Distinct Fourth employee numbers: `10`.
  - Distinct clock-in dates: `7`.
  - Closed clock-in/out rows: `70`.
- Regenerated audit workbook at `C:\Code\updatedChucs\automatedFourthPipeline\service.pipeline.fourth.com\outputs\square-replay-audit\20260704232039_SquareReplayAudit.xlsx`; workbook error scan found `0` error cells.
- Re-ran full solution build: `dotnet build service.pipeline.fourth.com.sln --no-restore` passed with `0` warnings and `0` errors.
- Re-ran default test suite: `dotnet test service.pipeline.fourth.com.sln --no-build --logger "console;verbosity=minimal"` passed.
- Re-ran vulnerable package scan: no vulnerable packages reported for all projects.
- Marked legacy live Square/Fourth credential-dependent tests as explicit so default test runs no longer fail on empty/stale credentials.
- Added root `README.md` with current status, verification commands, artifact paths, config notes, and remaining operational caveats.

### 2026-07-05 Expanded Read Replay and Spreadsheet Audit Slice

- Added explicit replay coverage for expanded Square sandbox run `20260704232039`.
- Test name: `SquareSandboxReplayTests.ReplayExpandedSandboxSeed_ThenVerifyFourthSpreadsheetPayloads`.
- Purpose: verify old Square sandbox data can still be read back from Square and transformed into Fourth spreadsheet-style payloads without creating new Square records.
- Replay source windows:
  - Sales order/payment window UTC: `2026-07-04T23:18:39.2726976Z` to `2026-07-04T23:36:29.9151453Z`.
  - Labor timecard window UTC: `2026-06-27T07:00:00.0000000Z` to `2026-07-04T23:35:55.4340721Z`.
- Read-side verification now covers:
  - Square payments read from Square and mapped to one Fourth `TENDER` row per paid order.
  - Square catalog/product SKUs mapped to Fourth `SALES_ITEM` rows through `SalesItemPLU`.
  - Square item modifiers mapped to Fourth `MODIFIER_ITEM` rows.
  - Square team members read from Square and mapped to Fourth employee numbers.
  - Square timecards read from Square and mapped to Fourth timesheet XML rows with closed clock-in/out pairs.
- Replay artifacts generated on 2026-07-05:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260704232039_ExpandedReplayFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260704232039_ExpandedReplayFourthTimesheets.xml`
  - Audit workbook: `C:\Code\updatedChucs\automatedFourthPipeline\service.pipeline.fourth.com\outputs\square-replay-audit\20260704232039_SquareReplayAudit.xlsx`
- Validation:
  - Expanded replay test passed on 2026-07-05.
  - Replay produced `52` Fourth sales CSV rows for the four expanded paid orders.
  - Replay produced `70` Fourth timesheet XML records, covering `10` employees across `7` dates.
  - Audit workbook formula/error scan matched `0` error cells.

### 2026-07-05 Worker Logging and Failure Alert Slice

- Confirmed DB-backed audit records exist for both Square-to-Fourth sales CSV and timesheet XML flows through `PipelineRunRecords`.
- Enabled the worker Serilog pipeline by wiring `UseSerilog()` and reading the existing `Serilog` appsettings block, so runtime worker logs are written through the configured SQL sink.
- Added `PipelineAlertService` for failure email notifications.
- Alert trigger points:
  - Whole scheduled job failure.
  - Brand-level setup/auth/login failure.
  - Store-level sales CSV failure.
  - Store-level timesheet XML failure.
  - Non-2xx Fourth upload responses for sales and timesheets, including Fourth status code and response body.
- Alerts are non-blocking: if email sending fails, that failure is logged but the original pipeline failure remains the primary error.
- Config added under `PipelineAlerts` in `squareservice.pipeline.fourth.com/appsettings.json`.
- Current recipient: `nadavdrewe@gmail.com`.
- Current status: alerting is wired but disabled until SMTP `Host`, `FromAddress`, and credentials are supplied.
- Validation: `dotnet build squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj --no-restore` passed on 2026-07-05.

### 2026-07-05 Expanded Sandbox Coverage Slice

- Expanded `SquareSandboxEndToEndSeedTests.SeedSandboxSquareData_ThenGenerateFourthCsv` to seed a larger end-to-end Square sandbox data set.
- Product coverage now includes `10` catalog products, each with a dedicated modifier, and a full-menu paid order that exercises every product/modifier pair.
- Payment coverage now asserts Square completed payments are present, payment order IDs match the paid Square orders, and Fourth `TENDER` rows reconcile to Square order totals plus tips.
- Discount/service-charge/tip coverage includes item discounts, discount rows, service charge rows, and tender totals.
- Hospitality/labor coverage now creates `10` team members mapped to `SANDBOX-EMP-01` through `SANDBOX-EMP-10` and `70` closed timecards covering `7` days.
- Validation:
  - `dotnet build tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj` passed on 2026-07-05.
  - `dotnet test tests.domain.pipelines.fourth.com\tests.domain.pipelines.fourth.com.csproj --filter "FullyQualifiedName~SquareSandboxEndToEndSeedTests.SeedSandboxSquareData_ThenGenerateFourthCsv" --logger "console;verbosity=detailed" --no-build` passed on 2026-07-05.

### 2026-07-05 DB Employee Mapping Slice

- Added DB-backed Square team member to Fourth employee number mapping.
- New table: `SquareEmployeeMappings`.
- Mapping scope: `StoreIntegrationId`, so Fourth employee numbers can differ per store/site.
- Worker behavior:
  - Loads active mappings with each active Square-to-Fourth store integration.
  - DB mappings override the legacy `SquareToFourthTimesheets` appsettings mapping fallback.
  - Unmapped Square team members still fall back to Square `TeamMemberId` to keep XML traceable.
- Added MVC admin CRUD at `/SquareEmployeeMappings`.
- Added a link from store integration edit pages and the integration config menu.
- Applied migration `20260704211421_squareEmployeeMappings` to remote SQL Server database `FourthSalesPipelineContext` on `173.212.231.129`.
- Remote DB check on 2026-07-05:
  - `SquareEmployeeMappings` exists.
  - Current active store integrations: `0`.
  - Current mapping rows: `0`.

### 2026-07-05 Pipeline Run Record Slice

- Added durable SQL records for Square-to-Fourth sales and timesheet pipeline outcomes.
- New table: `PipelineRunRecords`.
- Worker now writes records for:
  - Sales CSV skipped/no data, skipped/no rows, uploaded, and failed outcomes.
  - Timesheet XML skipped/no data, skipped/no rows, generated, uploaded, and failed outcomes.
- Stored fields include brand/store/integration identifiers, Square location, Fourth unit/location, source/target systems, data type, status, UTC data window, transaction date, row count, output file path, payload format, payload text, Fourth status/response, error message, and timestamps.
- Applied migration `20260704230556_pipelineRunRecords` to remote SQL Server database `FourthSalesPipelineContext` on `173.212.231.129`.
- Remote DB check on 2026-07-05:
  - `PipelineRunRecords` exists.
  - Index `IX_PipelineRunRecords_StoreIntegrationId_DataType_TransactionDate` exists.
  - Current run records: `0` because no active store integrations are configured in the remote DB yet.

### 2026-07-05 Pipeline Run Record Visibility Slice

- Added read-only MVC admin surface for persisted pipeline run records.
- New routes:
  - `/PipelineRunRecords/Index` lists the latest `500` records by creation time.
  - `/PipelineRunRecords/Details/{id}` shows identifiers, UTC window, file path, row count, payload, Fourth response, and error text.
- Added navigation link under `Fourth Logs`.
- Validation:
  - `dotnet build web.pipeline.fourth.com\web.pipeline.fourth.com.csproj` passed on 2026-07-05.
  - Remote SQL query against `PipelineRunRecords` passed on 2026-07-05; current row count remains `0` pending active store integrations.

### 2026-07-05 Square Setup Idempotency Slice

- Fixed `BrandsController.CreateNewSquareToFourthSalesIntegration` so setup processes both existing matched stores and newly-created stores.
- The setup flow now creates missing `StoreIntegration`, `SquareStoreConfig`, and `FourthSalesApiStoreConfig` rows for existing stores when the Square location already matches by store name.
- Re-running setup is idempotent for active Square-to-Fourth integrations with the same Square location id; existing integrations are skipped instead of duplicated.
- Tightened credential selection in the setup flow to use active `SquareApi` credentials only.
- Updated the worker Square credential path:
  - Refresh-token OAuth remains preferred when a refresh token is present.
  - Static stored `LatestAccessToken` is accepted when no refresh token exists, which allows sandbox/static-token DB setups to run through the same worker path.
- Validation:
  - `dotnet build web.pipeline.fourth.com\web.pipeline.fourth.com.csproj` passed on 2026-07-05.
  - `dotnet build squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj` passed on 2026-07-05.

### 2026-07-04 Package Vulnerability Cleanup Slice

- Removed unused SQLite references from `web.pipeline.fourth.com`; the app uses SQL Server for both EF contexts, so `Microsoft.EntityFrameworkCore.Sqlite` and `SQLitePCLRaw.lib.e_sqlite3` were unnecessary.
- Removed unused `Microsoft.Extensions.Logging.Debug` from `web.pipeline.fourth.com`.
- Removed unused `System.Net.Http.Json` from `tests.pos.pipeline.fourth.com`.
- Added direct safe package references to override vulnerable transitives:
  - `System.Security.Cryptography.Xml` `10.0.9` in `domain.pipeline.fourth.com`.
  - `System.Security.Cryptography.Xml` `10.0.9` in `com.fourth.pipeline.credentials`.
  - `MailKit` `4.17.0` and `MimeKit` `4.17.0` in `tests.domain.pipelines.fourth.com`.
- Validation:
  - `dotnet build service.pipeline.fourth.com.sln --no-restore` passed with `0` warnings and `0` errors on 2026-07-04.
  - `dotnet list service.pipeline.fourth.com.sln package --vulnerable --include-transitive` reported no vulnerable packages for all 12 projects on 2026-07-04.

### 2026-07-04 Worker Timesheet XML Slice

- Added Square labor timecard retrieval via `LaborService`.
- Added Square timecard to Fourth timesheet mapping and XML generation.
- Wired `squareservice.pipeline.fourth.com` nightly job to generate per-store timesheet XML artifacts before sales CSV processing.
- Default XML output directory: `C:\FourthPipeline\SquareToFourthTimesheets`.
- Fourth XML upload is guarded by config:
  - `SquareToFourthTimesheets:UploadToFourth` defaults to `false`.
  - `SquareToFourthTimesheets:XmlEndpoint` defaults to empty and must be supplied before upload is enabled.
- Current employee mapping behavior: Fourth `EmpNo` falls back to Square `TeamMemberId` because no Square-to-Fourth employee mapping table/config exists yet.
- Validation: `dotnet build service.pipeline.fourth.com.sln --no-restore` passed on 2026-07-04.

### 2026-07-04 Timesheet Employee Mapping Slice

- Added config-based Square team member to Fourth employee number mapping for timesheet XML.
- Global mappings live under `SquareToFourthTimesheets:EmployeeNumberMappings`.
- Per-location overrides live under `SquareToFourthTimesheets:LocationEmployeeNumberMappings:{SquareLocationId}`.
- Example:

```json
"SquareToFourthTimesheets": {
  "EmployeeNumberMappings": {
    "TM_SQUARE_TEAM_MEMBER_ID": "FOURTH_EMP_NO"
  },
  "LocationEmployeeNumberMappings": {
    "L_SQUARE_LOCATION_ID": {
      "TM_SQUARE_TEAM_MEMBER_ID": "LOCATION_SPECIFIC_FOURTH_EMP_NO"
    }
  }
}
```

- Location-specific mappings override global mappings.
- Unmapped Square team members still fall back to Square `TeamMemberId` so the XML remains traceable.
- Validation: `dotnet build squareservice.pipeline.fourth.com\squareservice.pipeline.fourth.com.csproj --no-restore` and `SquareTimesheetXmlContractTests` passed on 2026-07-04.

### 2026-07-04 Sandbox Replay Slice

- Added a non-destructive replay test for the recorded sandbox seed run `20260704204200`.
- Test name: `SquareSandboxReplayTests.ReplayRecordedSandboxSeed_ThenGenerateFourthArtifacts`.
- Purpose: verify old Square sandbox data can still be found by recorded UTC windows and transformed into Fourth sales CSV/timesheet XML without creating new Square records.
- Replay source windows:
  - Sales order window UTC: `2026-07-04T20:40:00.7555900Z` to `2026-07-04T20:57:12.3297190Z`.
  - Labor timecard window UTC: `2026-07-04T15:42:15.4922078Z` to `2026-07-04T20:57:12.3297190Z`.
- Replay artifacts generated on 2026-07-04:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260704204200_ReplayFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-replay\20260704204200_ReplayFourthTimesheets.xml`
- Validation: replay test passed on 2026-07-04 with `22` Fourth sales rows, `3` paid orders, ignored open order excluded, and `3` timecard rows.

### 2026-07-04 Expanded Edge-Case Seed

- Run ID: `20260704204200`.
- Sandbox location: `Default Test Account` (`L8WQDAS2AGWZC`).
- Seed start UTC: `2026-07-04T20:40:00.7555900Z`.
- Sales order window UTC: `2026-07-04T20:40:00.7555900Z` to `2026-07-04T20:57:12.3297190Z`.
- Labor timecard window UTC: `2026-07-04T15:42:15.4922078Z` to `2026-07-04T20:57:12.3297190Z`.
- Catalog variations:
  - Burger: `WLLEG5CWMCPYIJEQ5P6WMCJA`
  - Fries: `HFQEGL6ZEHVZACROZQVYPRUO`
  - Coffee: `TJP55KHJJ5MVLILPPI3ZLA2H`
  - Tea: `WXYPODJM2USK44EJ4Y54OJYU`
- Paid orders:
  - Discounted hospitality/tip order: `sGCkIIE4Y0104kib5sF1HBCEzD8YY`
  - Coffee service-charge order: `KjXFD36HSOnhH65cflL6W5c790TZY`
  - Quantity/modifier/percentage-discount edge order: `e19KDgZupWhsbDzCG5v6KcVAIWXZY`
- Ignored open/unpaid order: `yP5SQp8IlXeNArt5zXQYau6pUhGZY`.
- Team members:
  - Front of House: `TMbg6uJB2DvQ7rM0`
  - Kitchen: `TMQdYcjHghGEVElc`
  - Manager: `TMCFkTZd86HFxkf3`
- Timecards:
  - Closed Front of House timecard: `CDQ46SQPAC8D5`
  - Closed Kitchen timecard: `DTWANW2S3S8KJ`
  - Open Manager timecard: `QBDF4KVRA1CFF`
- Generated Fourth artifacts:
  - Sales CSV: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-seed\20260704204200_SquareSandboxFourthCsv.csv`
  - Timesheet XML: `C:\Users\nadav\AppData\Local\Temp\fourth-square-sandbox-seed\20260704204200_SquareSandboxFourthTimesheets.xml`
- Validation:
  - Seeded Fourth sales rows: `22`.
  - Completed-sales pipeline excluded the open/unpaid order.
  - Timesheet XML included two closed timecards and one open timecard.
