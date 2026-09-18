# Square to SAP requirements portal

## Live entry points

- Landing page: https://squaresap.store/
- Customer start/resume: https://squaresap.store/sap/requirements
- Existing portal entry: https://squaretofourth.store/sap/requirements
- Administrator review: https://squaresap.store/sap/admin

This release collects and reviews requirements. It does not implement or enable
production SAP posting. Exported answers are design inputs, not executable configuration.

## Customer workflow

1. Create a workspace with company, contact name and email.
2. Retain the reference and private access code shown once. Treat the code as a password.
3. Complete SAP landscape, sales processing, inventory, reporting, finance, and mapping/acceptance sections.
4. Save drafts as needed. Resume on another browser using the reference and code.
5. Review validation messages, confirm the answers, and submit for operator review.

Private browser access is host-specific. Use the same domain consistently, or
resume with the code on the other domain. Email is contact information, not a
verified identity. Customer audit names are self-declared. Do not enter credentials
or secrets in answers or evidence fields.

## Administrator workflow

Sign in with an existing platform Administrator account at /Access/Login, then
open /sap/admin. Open a submission to inspect answers, outstanding decisions and
revision history. Export CSV or JSON for implementation planning. Approval requires
a submitted, complete record without unresolved decisions/actions. Reopen to amend.
Approval is a requirements-review state; it does not deploy a connector or constitute
customer identity verification.

## Persistence and deployment

The existing application database holds dbo.SapDiscoveryDocuments and
dbo.SapDiscoveryRevisions. Resume secrets are stored as hashes. Every saved revision
has an answer snapshot; optimistic concurrency rejects stale edits. Access cookies
use existing ASP.NET Data Protection. Preserve its keys and production configuration.

Apply sap-discovery.sql before serving the new feature. Deploy-SapDiscovery.ps1
performs the additive schema update and backs up the application while preserving
production configuration. Setup-SapDomain.ps1 configures the IIS hostname and nginx
front proxy. Run nginx reload operations as SYSTEM on this server. The certificate
renewal task uses Reload-SapCertificate.ps1; retain this hook and the renewal task.

Initial release: sap-20260917-01. Server application backup:
C:\Deploy\SquareToFourth\backups\SquareToFourth-20260917-223257.

## Verification

- .NET 10 web build and release publish succeeded.
- Seven focused SapDiscoverySchemaTests passed.
- Test-SapDiscovery.ps1 passed live create/save/validation/concurrency/submission,
  revision history, CSV export, private-code resume and customer approval denial.
- The script creates a clearly labelled deployment-test workspace, not SAP postings.
- Administrator approval remains unverified: the previously supplied administrator
  password was rejected by production. Use the current credential; do not weaken
  or replace authentication to complete this check.
- Desktop and mobile customer layout were inspected; a responsive grid typo was
  corrected in source and the live CSS after publishing the initial release archive.

Run the smoke script only when creating another test workspace is intended. Keep
normal database backups: application-file rollback does not restore database data.

## Requirements refinement (schema 2)

Deployed release: sap-20260917-03. Pre-release server application backup:
C:\Deploy\SquareToFourth\backups\SquareToFourth-20260917-233706.

- Conditional follow-ups now cover receipt-chain recovery, aggregated-batch refunds,
  stock quantity/frequency, other SAP products, and settlement ownership versus implementation.
- Added Square authorisation/location scope and catalogue/unmapped-item ownership.
- Resume returns to the last saved step; old documents default to the first step.
  References and private codes tolerate letter casing. Progress is saved explicitly,
  not automatically: use Save draft or Save & continue before leaving a section.
- Contradictory business decisions can be retained in a draft, but block submission.
- Review includes Download requirements PDF. The protected server-side export contains
  applicable saved answers, status/revision, validation issues and revision-log metadata.
  Full historical answer snapshots remain in JSON/the portal. PDF generation uses
  PDFsharp/MigraDoc 6.2.4 with Windows fonts; a Linux deployment needs a font resolver.
- Ten focused tests passed, including long-answer PDF pagination. All 12 rendered
  test PDF pages were visually inspected. Live smoke testing passed private downloads,
  partial-draft resume, wrong-code rejection, full-answer persistence and submission.
- Administrator approval still requires the current administrator credential; this
  pass did not change authentication or verify that outstanding administrator step.

## Guided requirements assistant

The optional assistant is documented in `SAP_ASSISTANT.md`. It is disabled unless a
server-side OpenAI project key is configured. The application does not send company,
contact or email fields, does not persist chat history and does not permit the model
to save, submit or approve a workspace. Use `Configure-SapAssistant.ps1` on the server
to enable or disable it without committing credentials.

Assistant framework release: `sap-20260918-02`. The release was deployed with the
assistant disabled. Production readiness, landing/form content, branded assets and
static assistant assets passed external checks. Activation still requires a real
OpenAI project API key followed by the live-model evaluation described in
`SAP_ASSISTANT.md`.
