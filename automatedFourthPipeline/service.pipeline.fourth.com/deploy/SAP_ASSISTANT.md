# SAP requirements guide

The guide is an optional, form-aware assistant for the Square to SAP requirements
workspace. It explains questions, reviews draft answers and proposes answers that a
customer can explicitly apply. It cannot save, submit, approve or change a workspace.

## Design

- Model: `gpt-5.6-terra` with low reasoning effort. The model is configurable so it
  can be changed without a code release after a quality/cost evaluation.
- API: OpenAI Responses API with strict structured output and `store: false`.
- Context: versioned system rules, the current applicable form schema, a compact set
  of cross-section architectural decisions, unsaved fields in the open section,
  validation issues and at most six in-memory chat turns. Company, contact and email
  are excluded. Owner, participant and evidence fields are represented only as
  provided/not provided rather than sending their values.
- Retrieval: no vector database is needed. The requirements schema is small and is
  included directly, which keeps the answer grounded in the exact deployed form.
- Safety: user content is treated as untrusted data; likely credentials, email
  addresses and phone numbers are rejected before an API call; prompts are moderated;
  suggestions are server-filtered to visible current fields and exact select options;
  output is rendered as text, not HTML.
- Form control: field-level help uses explicit explain, review, workshop, field and
  readiness modes. Suggested replacements require a second confirmation, can be
  undone and remain unsaved until the customer saves. Accepted AI drafts are recorded
  as field IDs in revision attribution; chat content is not retained.
- Reliability: transient 429/5xx responses are retried once. Refusals, incomplete or
  malformed output and timeouts are handled explicitly. When live guidance is down,
  the UI returns deterministic schema/validation guidance instead of changing data.
- Control: rate limited by workspace and IP, with a default 40-request daily workspace
  budget. Conversations are not persisted. Logs contain latency, token counts, model,
  prompt/context versions and a hashed workspace reference, never prompt/answer bodies.
  API keys remain server-side and are never exposed to the browser.

The guide is advisory. Tax, finance, security, licensing and SAP configuration
decisions still require the named customer or implementation owner.

## Production activation

Create a restricted OpenAI project for this service, add billing/usage limits, create
a project API key and store it in the deployment password manager. On the web server,
open an elevated PowerShell session and run:

```powershell
cd C:\inetpub\wwwroot\SquareToFourth\deploy
.\Configure-SapAssistant.ps1
```

Enter the API key only at the secure prompt. The script updates IIS-hosted environment
variables, backs up `web.config` and recycles the application pool. It does not print
the key. To disable the assistant and remove the key from `web.config`:

```powershell
.\Configure-SapAssistant.ps1 -Disable
```

The assistant is hidden from customers while disabled. Administrators can see its
configuration state on `/sap/admin`.

## Acceptance checks

1. Ask it to explain each section without supplying an answer. It should name the
   client owner or evidence needed when facts are missing.
2. Verify a suggested select answer exactly matches one of that field's options and
   only changes the form after **Use this draft** is selected.
3. Confirm unsaved answers are included in a review but remain unsaved after chat.
4. Try to provide a password or API key. The request must be rejected before an API
   call is made.
5. Test S/4HANA, ECC and Business One examples; unsupported assumptions must be clear.
6. Test refund, duplicate stock movement, settlement and aggregated posting conflicts.
7. Verify Save, resume, PDF export, submit and administrator approval still work with
   the assistant enabled and with it disabled.
8. Review API usage and errors after the first customer sessions. Do not log request
   bodies or model responses.
9. Run the 40 scenarios in `design/sap-assistant-evaluation.json` before changing the
   model or prompt. Each scenario identifies required and prohibited answer behaviour.

An actual OpenAI call must be tested after a real project key is installed. Automated
tests cover payload shape, stateless operation, context redaction, field scoping,
usage limits, transient retry and suggestion filtering; they do not prove account
billing, network access or model availability.
