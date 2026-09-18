using System;
using System.Collections.Generic;
using System.Linq;

namespace web.pipeline.fourth.com.Models;

public record SapQuestion(string Id, string Label, string Help = "", string Type = "text", string[] Options = null, bool Required = false, string When = null, string EqualsValue = null, bool NotEquals = false);
public record SapPhase(string Id, string Title, string Description, SapQuestion[] Questions);
public record SapQuestionGroup(string Id, string Title, string Description, string[] FieldIds);
public sealed class SapDiscovery
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Company { get; set; } = "";
    public string Contact { get; set; } = "";
    public string Email { get; set; } = "";
    public string Status { get; set; } = "Draft";
    public int Revision { get; set; }
    public int LastStep { get; set; }
    public Dictionary<string, string> Answers { get; set; } = new();
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public string Answer(string id) => Answers.GetValueOrDefault(id, "");
}
public record SapRevision(int Revision, string Actor, string Status, DateTimeOffset At, string Json);
public record SapEditor(SapDiscovery Document, int Step, IReadOnlyList<SapRevision> History = null);

// Stable identifiers and applicability rules are shared by the UI, validation and exports.
public static class SapDiscoverySchema
{
    public const int Version = 3;
    static SapQuestion Text(string id, string label, string help = "", bool required = false) => new(id, label, help, Required: required);
    static SapQuestion Notes(string id, string label, string help = "", bool required = false) => new(id, label, help, "textarea", Required: required);
    static SapQuestion Choice(string id, string label, string help, params string[] choices) => new(id, label, help, "select", choices, true);
    static SapQuestion[] WithDecision(string phase, params SapQuestion[] fields) => fields.Concat(new[] {
        Text(phase + ".decisionOwner", "Who approves this section?", "Enter the name and role of the accountable business or SAP owner.", true),
        Notes(phase + ".reason", "Why is this the right decision?", "Summarise the agreed approach, why it fits the business and where it applies.", true),
        Notes(phase + ".evidence", "Supporting evidence", "Add document titles, ticket references or approved configuration links. Never include credentials or customer personal data."),
        Notes(phase + ".exceptions", "Assumptions and exceptions", "Record anything provisional, out of scope or handled differently by location or scenario."),
        Notes(phase + ".actions", "What is still open?", "List the actions needed before design can be approved. Enter None when this section is resolved.", true),
        new SapQuestion(phase + ".actionOwner", "Who owns the open action?", "Name and role of the person responsible for resolving it.", Required: false, When: phase + ".actions", EqualsValue: "None", NotEquals: true),
        new SapQuestion(phase + ".due", "When is the action due?", Type: "date", When: phase + ".actions", EqualsValue: "None", NotEquals: true)
    }).ToArray();

    public static readonly SapPhase[] Phases = {
        new("landscape", "SAP landscape", "Confirm the target systems, available integration services and the Square estate in scope.", WithDecision("landscape",
            Choice("sap.product", "Which SAP product is the target?", "The product and exact release determine which supported business interfaces are available.", "S/4HANA", "ECC", "Business One", "Other", "Not yet confirmed"),
            new("sap.otherProduct", "SAP product name", Required: true, When: "sap.product", EqualsValue: "Other"),
            Choice("sap.deployment", "How is the target SAP system deployed?", "Deployment affects connectivity, authentication and API activation.", "Public Cloud", "Private Cloud", "On-premises", "Not yet confirmed"),
            Text("sap.release", "Exact SAP release and support level", "Use the version shown by SAP administration, including feature pack or support package where applicable.", true),
            Text("sap.modules", "SAP modules and business functions in scope", "For example SD, MM, FI and Retail, or the relevant Business One functions and add-ons.", true),
            Notes("sap.products", "Available SAP integration and retail services", "Mark Integration Suite, Event Mesh, Cloud Connector, OSTA and CAR as available, unavailable or unknown.", true),
            Notes("sap.environments", "Available SAP environments", "List development, QA and production systems plus their owners. Do not include credentials.", true),
            Choice("sap.network", "Approved network route to SAP", "Private systems normally require Cloud Connector, VPN or another approved private route.", "Cloud Connector", "Public API", "VPN / private network", "Other / to confirm"),
            Text("sap.authentication", "Approved SAP authentication method", "For example OAuth, client certificate or technical user. Describe the method only, never a credential.", true),
            Text("sap.activationOwner", "Who can activate SAP APIs and connectivity?", "Name the team or role that approves communication scenarios, technical users and firewall changes.", true),
            Notes("scope.organisation", "Organisations and locations in scope", "Include legal entities, store count, countries, currencies and timezones.", true),
            Notes("square.access", "Square merchants, locations and test access", "List approved merchant/location IDs if known, rollout locations, sandbox availability and the Square authorisation owner. Never enter passwords or tokens.", true),
            Notes("square.catalog", "Square catalogue and mapping ownership", "Identify who owns variations/SKUs and how ad-hoc items, modifiers, bundles, units and missing SAP mappings are handled.", true))),
        new("posting", "Posting model", "Choose the SAP document approach, required latency and recovery expectations.", WithDecision("posting",
            Choice("posting.model", "How should a completed Square sale be represented in SAP?", "Receipt chain creates native SAP documents per receipt. Aggregates reduce document volume but retain receipts outside SAP. Retail pipeline requires a licensed SAP retail product.", "Receipt document chain", "Aggregated inventory and finance", "SAP retail POS pipeline", "Not yet decided"),
            Choice("posting.frequency", "How quickly must completed sales reach SAP?", "Lower latency increases availability, monitoring and support requirements.", "Real-time", "Near-real-time", "Hourly", "End of business day"),
            Choice("posting.invoices", "One native SAP invoice per Square receipt?", "Daily aggregates cannot also supply one native invoice per receipt.", "Yes", "No", "Not yet decided"),
            new("posting.retailProduct", "Which licensed SAP retail ingestion product will be used?", "Confirm the exact product and licence with the SAP owner.", Required: true, When: "posting.model", EqualsValue: "SAP retail POS pipeline"),
            new("posting.aggregation", "Which dimensions define an aggregate?", "Usually company, store, material/SKU, tax code, tender, currency and local business date.", "textarea", Required: true, When: "posting.model", EqualsValue: "Aggregated inventory and finance"),
            new("volume.daily", "Typical Square receipts per business day", "Use the total across the locations in scope.", Type: "number", Required: true),
            new("volume.peak", "Peak completed receipts per hour", "Use the busiest expected hour across the locations in scope.", Type: "number", Required: true),
            Notes("posting.outage", "Maximum outage and recovery requirements", "State the allowed delay, queue retention, recovery target, replay approval and operational owner.", true),
            new("posting.recovery", "Receipt-chain recovery owner and resume point", "Explain who resolves an incomplete order/delivery/goods-issue/billing chain and how the last successful SAP step is identified.", "textarea", Required: true, When: "posting.model", EqualsValue: "Receipt document chain"),
            Notes("posting.refunds", "How are returns and refunds linked to the original sale?", "Define the Square receipt/order reference and SAP document references that must be retained.", true))),
        new("inventory", "Inventory ownership", "Define the stock system of record and ensure every physical movement is represented once.", WithDecision("inventory",
            Choice("inventory.owner", "Which system is the inventory system of record?", "SAP ownership normally makes each completed sale the source of one goods issue. Split ownership needs an explicit location boundary.", "SAP", "Square", "Split by location", "Not yet decided"),
            new("inventory.boundary", "Which system owns stock at each location?", "List each location group and whether SAP or Square is authoritative.", "textarea", Required: true, When: "inventory.owner", EqualsValue: "Split by location"),
            Choice("inventory.direction", "What is the approved stock synchronisation direction?", "Sale-generated Square inventory events must not overwrite SAP or create a second reduction.", "SAP to Square", "Square to SAP via approved movements", "Bidirectional with explicit rules", "No quantity synchronisation"),
            new("inventory.frequency", "How often should stock quantities synchronise?", "For example every five minutes, hourly or after an approved close.", Required: true, When: "inventory.direction", EqualsValue: "No quantity synchronisation", NotEquals: true),
            new("inventory.quantity", "Which quantity should synchronise?", "On-hand and available-to-sell differ when stock is reserved, blocked or otherwise unavailable.", "select", new[] { "On-hand", "Available-to-sell", "Both, separately" }, true, "inventory.direction", "No quantity synchronisation", true),
            Choice("inventory.saleCount", "How should sale-generated Square inventory events be used?", "A completed sale and its inventory notification must never create two SAP stock reductions.", "Reconciliation only; no second goods issue", "Needs design review"),
            Text("inventory.negative", "What happens when SAP has insufficient stock?", "State whether posting is blocked, queued for resolution or allowed under an approved negative-stock rule.", true),
            Notes("inventory.movements", "How are non-sale stock movements handled?", "Cover supplier receipts, store transfers, damage, recounts and manual Square adjustments.", true),
            Choice("inventory.refund", "What happens to stock when money is refunded but goods are not returned?", "A financial refund alone must not increase physical stock.", "Finance credit only; no stock movement", "Needs design review"),
            Notes("inventory.returns", "How are physical returns and exchanges handled?", "Define the goods receipt or reversal, inspection, destination location and original-sale reference.", true),
            Text("inventory.batchSerial", "Are batch or serial numbers required?", "Enter None if not required. Otherwise explain where identifiers originate and how they are retained.", true),
            Notes("inventory.reconcile", "How is stock reconciled?", "State frequency, permitted tolerance, exception owner and resolution process.", true))),
        new("detail", "Sales detail", "Choose what SAP must retain and how users trace every posting back to Square.", WithDecision("detail",
            Choice("detail.level", "What is the lowest sales detail SAP must store?", "Receipt-level detail creates more SAP documents. Aggregates require receipt-level audit outside SAP.", "Receipt", "Receipt line", "Store / SKU / business day", "Store / tax / tender / business day", "Company / business day"),
            Choice("detail.customer", "How should customers be represented in SAP?", "Avoid transferring personal data unless the agreed business process requires it.", "Generic cash customer", "Identified Business Partner", "Mixed, with explicit rules"),
            new("detail.customerRule", "How are identified customers mapped?", "Describe how an existing Business Partner is found or when one may be created.", Required: true, When: "detail.customer", EqualsValue: "Generic cash customer", NotEquals: true),
            Choice("detail.margin", "Must SAP report product-level margin or COGS?", "Company totals cannot support product-level margin without material/SKU detail.", "Yes", "No", "Not yet decided"),
            Notes("detail.components", "Which discounts, modifiers, taxes and tenders must be visible in SAP?", "Separate information required in SAP from detail that may remain in Square or the integration audit store.", true),
            Text("detail.retention", "Where and how long is receipt-level audit retained?", "Name the system of record, retention period and accountable owner.", true),
            Text("detail.drillback", "How will users trace an SAP posting back to Square?", "Define the batch/document reference and the search route to Square orders and payments.", true),
            new("detail.closedRefunds", "How are partial refunds posted after an aggregate batch is closed?", "Define posting date, original batch reference and the policy for closed accounting periods.", "textarea", Required: true, When: "posting.model", EqualsValue: "Aggregated inventory and finance"),
            Text("detail.businessDay", "What defines the local business day?", "State the cutoff time, timezone and daylight-saving treatment. Preserve the original local business date.", true))),
        new("finance", "Finance & reconciliation", "Agree revenue, tax, tender, settlement and exception treatment with SAP FI and accounting owners.", WithDecision("finance",
            Choice("finance.trigger", "When is revenue recognised in SAP?", "Settlement is a later clearing event; a net payout must not replace gross sales, tax and tender posting.", "Completed sale", "Completed order", "Approved store close", "SAP billing", "Settlement - requires FI review"),
            Choice("finance.price", "Which system owns the final sale price?", "Choose the authority used to reconcile the amount charged to the customer.", "Square", "SAP", "Rules by scenario"),
            Choice("finance.tax", "Which system owns the final tax value?", "The accounting/tax owner must also approve SAP tax-code and condition mappings.", "Square", "SAP", "Rules by scenario"),
            Choice("finance.documents", "What finance documents are required in SAP?", "Receipt-level billing must agree with the posting model selected earlier.", "Receipt-level billing", "Summarised journals", "Retail pipeline documents", "To be confirmed by FI"),
            Notes("finance.tenders", "How are tenders and liabilities treated?", "Cover cash, card, gift cards, store credit, tips, split tenders and any other tender types.", true),
            Notes("finance.accounts", "Which posting categories and account rules are required?", "Cover revenue, tax, COGS, inventory, clearing, fees, liabilities, suspense and bank. Account numbers can be supplied later by FI.", true),
            Choice("finance.payouts", "Is Square payout and fee processing in scope?", "Payouts clear the processor balance and fees require separate treatment.", "Include payouts and fees", "Existing finance process handles settlement", "Not yet decided"),
            new("finance.clearing", "How will payouts clear and reconcile to the bank?", "State the payout source, gross-to-net controls, fee posting and accountable owner.", "textarea", Required: true, When: "finance.payouts", EqualsValue: "Include payouts and fees"),
            new("finance.settlementOwner", "Which existing process owns settlement?", "Identify the system/team handling clearing and the handoff/control totals this integration must provide.", Required: true, When: "finance.payouts", EqualsValue: "Existing finance process handles settlement"),
            Notes("finance.adjustments", "How are refunds, voids, disputes and chargebacks posted?", "Define finance and stock treatment for each scenario, including exchanges.", true),
            Notes("finance.currency", "How are currency, rounding and tolerances controlled?", "State exchange-rate authority, posting period, rounding treatment and permitted reconciliation tolerance.", true),
            Text("finance.fiOwner", "Who is the SAP FI approver?", "Enter name and role.", true),
            Text("finance.taxOwner", "Who is the accounting or tax approver?", "Enter name and role.", true))),
        new("mapping", "Mappings & acceptance", "Provide stable identifiers, exception controls and the evidence required before implementation.", WithDecision("mapping",
            Notes("mapping.organisation", "How do Square locations map to SAP organisations?", "One row per mapping: merchant ID | location ID | company code | plant | storage location | sales organisation | channel | division | profit centre.", true),
            Notes("mapping.products", "How do Square variations map to SAP materials?", "One row per mapping: Square variation ID | SKU | SAP material | unit of measure. Use stable IDs, not display names.", true),
            Notes("mapping.finance", "How do taxes, discounts and tenders map?", "One row: type | Square ID or tender | SAP condition/code/account | currency | notes.", true),
            Notes("mapping.documents", "Which SAP configuration must be confirmed?", "List document types, movement types, pricing conditions and account determination. API names remain provisional until release and process are confirmed.", true),
            Notes("controls.replay", "How are duplicates, retries and failed records controlled?", "Use merchant + location + order as the sale key and retain payment/refund/event IDs. Define replay approval, retry limits, dead-letter ownership and reconciliation.", true),
            Notes("controls.acceptance", "Which scenarios and totals must pass acceptance?", "Include sales, tax, tenders, discounts, partial refunds, physical/non-physical returns, duplicate events, outages, partial SAP chains, payout fees and business-day cutoffs.", true),
            Text("controls.goLive", "What is the rollout plan and who supports it?", "State target date, initial locations, cutover decision owner and operational support owner.", true)))
    };

    public static IReadOnlyList<SapQuestionGroup> GroupsFor(SapPhase phase)
    {
        var groups = phase.Id switch
        {
            "landscape" => new[] {
                new SapQuestionGroup("platform", "SAP platform", "Confirm the product, deployment, release and capabilities before naming interfaces.", new[] { "sap.product", "sap.otherProduct", "sap.deployment", "sap.release", "sap.modules", "sap.products", "sap.environments" }),
                new SapQuestionGroup("connectivity", "Connectivity and access", "Capture approved routes and owners, never credentials.", new[] { "sap.network", "sap.authentication", "sap.activationOwner" }),
                new SapQuestionGroup("scope", "Square and business scope", "Define exactly which organisations, Square locations and catalogue processes are included.", new[] { "scope.organisation", "square.access", "square.catalog" })
            },
            "posting" => new[] {
                new SapQuestionGroup("approach", "Posting approach", "Choose one primary SAP document model and a compatible latency.", new[] { "posting.model", "posting.frequency", "posting.invoices", "posting.retailProduct", "posting.aggregation" }),
                new SapQuestionGroup("resilience", "Volume and recovery", "Size the flow and agree what happens when SAP or part of a document chain fails.", new[] { "volume.daily", "volume.peak", "posting.outage", "posting.recovery", "posting.refunds" })
            },
            "inventory" => new[] {
                new SapQuestionGroup("ownership", "Ownership and synchronisation", "Set the source of truth and prevent the same sale reducing stock twice.", new[] { "inventory.owner", "inventory.boundary", "inventory.direction", "inventory.frequency", "inventory.quantity", "inventory.saleCount" }),
                new SapQuestionGroup("movements", "Movement rules and reconciliation", "Define every important stock movement, exception and control.", new[] { "inventory.negative", "inventory.movements", "inventory.refund", "inventory.returns", "inventory.batchSerial", "inventory.reconcile" })
            },
            "detail" => new[] {
                new SapQuestionGroup("record", "SAP sales record", "Choose the minimum useful detail without creating unnecessary SAP volume or personal-data transfer.", new[] { "detail.level", "detail.customer", "detail.customerRule", "detail.margin", "detail.components" }),
                new SapQuestionGroup("audit", "Audit, refunds and business day", "Keep a dependable route from SAP totals or documents back to the source transaction.", new[] { "detail.retention", "detail.drillback", "detail.closedRefunds", "detail.businessDay" })
            },
            "finance" => new[] {
                new SapQuestionGroup("recognition", "Revenue, price and tax", "Agree what creates the accounting event and which system supplies authoritative values.", new[] { "finance.trigger", "finance.price", "finance.tax", "finance.documents" }),
                new SapQuestionGroup("settlement", "Tenders, accounts and settlement", "Separate gross sales posting from processor payout, fees and bank clearing.", new[] { "finance.tenders", "finance.accounts", "finance.payouts", "finance.clearing", "finance.settlementOwner" }),
                new SapQuestionGroup("exceptions", "Adjustments and approval", "Cover the financial exceptions, tolerances and accountable approvers.", new[] { "finance.adjustments", "finance.currency", "finance.fiOwner", "finance.taxOwner" })
            },
            "mapping" => new[] {
                new SapQuestionGroup("mappings", "Business mappings", "Use stable Square and SAP identifiers rather than display names.", new[] { "mapping.organisation", "mapping.products", "mapping.finance", "mapping.documents" }),
                new SapQuestionGroup("controls", "Operational controls and acceptance", "Define replay, exception, reconciliation and rollout evidence before implementation.", new[] { "controls.replay", "controls.acceptance", "controls.goLive" })
            },
            _ => Array.Empty<SapQuestionGroup>()
        };
        return groups.Append(new SapQuestionGroup("decision", "Decision record", "Capture approval, rationale, evidence, exceptions and any remaining actions.", new[] {
            phase.Id + ".decisionOwner", phase.Id + ".reason", phase.Id + ".evidence", phase.Id + ".exceptions",
            phase.Id + ".actions", phase.Id + ".actionOwner", phase.Id + ".due"
        })).ToArray();
    }

    public static string ExampleFor(SapQuestion question)
    {
        if (question.Id.EndsWith(".actions", StringComparison.Ordinal)) return "Enter None, or list each action that remains open.";
        if (question.Id.EndsWith(".reason", StringComparison.Ordinal)) return "Summarise the agreed decision, why it was chosen and where it applies.";
        if (question.Id.EndsWith(".evidence", StringComparison.Ordinal)) return "e.g. approved design document, ticket or configuration reference";
        if (question.Id.EndsWith(".exceptions", StringComparison.Ordinal)) return "e.g. one location follows a different process; confirm before rollout";
        return question.Id switch
        {
            "sap.release" => "e.g. S/4HANA 2023 FPS02",
            "sap.modules" => "e.g. SD, MM and FI",
            "sap.products" => "Integration Suite: available\nEvent Mesh: unknown\nCloud Connector: available\nOSTA/CAR: not licensed",
            "sap.environments" => "DEV - available, SAP Platform team\nQA - available, SAP Platform team\nProduction - available, change approval required",
            "scope.organisation" => "e.g. 2 legal entities, 12 UK stores, GBP, Europe/London",
            "square.access" => "e.g. sandbox and production available; 12 locations; authorisation owned by Retail Systems",
            "square.catalog" => "e.g. Operations owns SKUs; integration support resolves unmapped variations",
            "posting.aggregation" => "e.g. legal entity + store + material + tax code + tender + currency + business date",
            "posting.outage" => "e.g. queue for 72 hours; recover within 4 hours; replay approved by Integration Support",
            "inventory.boundary" => "e.g. SAP: all managed stores; Square: pop-up locations only",
            "inventory.frequency" => "e.g. every 15 minutes",
            "inventory.negative" => "e.g. queue the sale and alert Inventory Control",
            "inventory.batchSerial" => "e.g. None",
            "detail.customerRule" => "e.g. match by approved external ID; do not create automatically",
            "detail.retention" => "e.g. integration audit store, 7 years, owned by Finance Systems",
            "detail.drillback" => "e.g. SAP reference stores the Square order ID and integration batch ID",
            "detail.businessDay" => "e.g. 04:00 Europe/London, including daylight-saving changes",
            "finance.tenders" => "e.g. card to processor clearing; cash to store cash; gift card to liability",
            "finance.accounts" => "List required posting categories and the FI owner; account numbers may follow after design review.",
            "finance.clearing" => "e.g. gross sales clear against Square payouts; fees post separately; daily control total",
            "mapping.organisation" => "merchant ID | location ID | company code | plant | storage location | sales organisation | channel | division | profit centre",
            "mapping.products" => "variation ID | SKU | SAP material | unit of measure",
            "mapping.finance" => "type | Square ID/tender | SAP condition, code or account | currency | notes",
            "controls.acceptance" => "List scenarios and expected Square/SAP control totals.",
            "controls.goLive" => "e.g. 15 October; 2 pilot stores; cutover owner; support owner",
            _ => ""
        };
    }

    public static bool Applies(SapQuestion q, SapDiscovery d) => q.When == null ||
        (!string.IsNullOrWhiteSpace(d.Answer(q.When)) && (q.NotEquals ? d.Answer(q.When) != q.EqualsValue : d.Answer(q.When) == q.EqualsValue));
    public static IEnumerable<SapQuestion> Applicable(SapDiscovery d) => Phases.SelectMany(p => p.Questions).Where(q => Applies(q, d));
    public static List<string> Validate(SapDiscovery d, bool complete)
    {
        var errors = new List<string>();
        foreach (var q in Applicable(d))
        {
            var value = d.Answer(q.Id);
            if (complete && q.Required && string.IsNullOrWhiteSpace(value)) errors.Add(q.Label + " is required.");
            if (value.Length > 8000) errors.Add(q.Label + " must be at most 8,000 characters.");
            if (value != "" && q.Options != null && !q.Options.Contains(value)) errors.Add(q.Label + " has an invalid choice.");
            if (value != "" && q.Type == "number" && (!int.TryParse(value, out var n) || n < 0)) errors.Add(q.Label + " must be a non-negative whole number.");
            if (value != "" && q.Type == "date" && !DateOnly.TryParseExact(value, "yyyy-MM-dd", out _)) errors.Add(q.Label + " must be a valid date.");
        }
        if (complete && d.Answer("posting.model") == "Aggregated inventory and finance" &&
            (d.Answer("posting.invoices") == "Yes" || d.Answer("finance.documents") == "Receipt-level billing" || new[] { "Receipt", "Receipt line" }.Contains(d.Answer("detail.level"))))
            errors.Add("Aggregated posting conflicts with receipt-level SAP documents/detail. Agree one consistent posting model.");
        if (complete && d.Answer("detail.level") == "Company / business day" && d.Answer("detail.margin") == "Yes")
            errors.Add("Company/day totals cannot support product-level margin. Select product detail or change the margin requirement.");
        if (complete)
            foreach (var p in Phases)
                if (d.Answer(p.Id + ".actions") is var a && !string.IsNullOrWhiteSpace(a) && !a.Equals("None", StringComparison.OrdinalIgnoreCase) &&
                    (string.IsNullOrWhiteSpace(d.Answer(p.Id + ".actionOwner")) || string.IsNullOrWhiteSpace(d.Answer(p.Id + ".due"))))
                    errors.Add(p.Title + ": open actions need an owner and due date (or enter None if resolved).");
        return errors;
    }
}
