# Square to SAP splash-page concept

## Selected direction

`square-sap-splash-v2.png` is the selected direction and has now been implemented in the application. It replaces the darker first exploration with a brighter, more mature enterprise composition: editorial copy on warm white, a single midnight integration blueprint, SAP blue as the action colour, and the five-decision model at the fold.

Implementation files:

- `web.pipeline.fourth.com/Views/Sap/Index.cshtml`
- `web.pipeline.fourth.com/Views/Shared/_SapLayout.cshtml`
- `web.pipeline.fourth.com/Views/Shared/_SapBrand.cshtml`
- `web.pipeline.fourth.com/wwwroot/css/sap-landing-v2.css`

The PNG is the visual target and `square-sap-splash.mocklang.json` is the implementation model. The model fixes the hierarchy, copy, tokens, responsive behavior, accessibility requirements and logo rules while leaving the implementation free to use the existing Razor structure.

## Design direction

- Enterprise integration blueprint, not a generic marketing template.
- Dark technical canvas with restrained SAP-blue/cyan illumination.
- Official Square and SAP assets with generous clear space.
- One dominant outcome statement, two actions and three trust cues.
- A truthful architecture story: Square → map/validate/reconcile → SAP operations → inventory and finance.
- The five-decision model is visible at the fold so the next step is immediately understandable.

## Required page language

**Eyebrow:** Square sales. SAP operations.

**Headline:** Every sale. A bigger picture.

**Lead:** Plan the connection between Square sales, SAP inventory and finance — with every decision, owner and mapping in one shared workspace.

**Primary action:** Define your integration

**Secondary action:** Explore the decision model

**Trust line:** Saved drafts · Private resume code · Export-ready

**Independence label:** Independent integration discovery workspace

## Build notes

Use the locally hosted files already present at `/images/brand/square-official.png` and `/images/brand/sap.svg`. Keep the existing routes and server behavior. The blueprint is explanatory; it must not present fabricated health, throughput or status data. Avoid claims of certification or partnership.

The first implementation pass should update `Views/Sap/Index.cshtml`, `Views/Shared/_SapLayout.cshtml`, and the SAP stylesheets only. The requirements workflow can inherit the same tokens afterward.
