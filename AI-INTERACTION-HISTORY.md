# AI Interaction History

This file is a concise, auditable record of AI-assisted work. It intentionally summarizes interactions instead of presenting an invented verbatim chat export.

## Initial implementation

AI assistance was used to scaffold the Clean Architecture projects, CQRS request/handler structure, FluentValidation pipeline, EF Core mappings, Angular Material views, and local infrastructure. Human review corrected package alignment, deterministic seed identifiers, reserve-ledger behavior, and role gating.

## Assessment audit

The codebase was audited against the supplied target-deliverables, architecture, backend, frontend, and business-rule checklists. The audit identified these material gaps:

- Azure deployment and CI/CD deploy stages were placeholders.
- Document storage existed but was not reachable from API or UI.
- Parties, reserve listing/retraction, and policy coverage endpoints were absent.
- Reserve handlers updated a denormalized balance.
- Claim-number allocation committed separately from claim creation.
- Tenant filters were incomplete and command validation coverage was partial.
- The README contained malformed fenced blocks and unsupported deployment claims.

## Remediation interaction

The follow-up request was to close all checklist items and produce a working vertical slice. AI assistance implemented and reviewed:

- document upload/list/download with local and Azure Blob providers;
- party add/soft-delete workflows;
- reserve list/retract workflows and complete command validation;
- append-only reserve balance calculation;
- transactional SQL-locked claim-number allocation;
- global fixed-tenant query filters and soft-delete metadata;
- pure domain events without a MediatR dependency;
- Angular document, party, reserve, and authority UI;
- Bicep infrastructure and active GitHub Actions Azure deployment;
- corrected local, Docker, and Azure operating documentation.

## Human-owned deployment evidence

Azure subscription credentials, GitHub environment secrets, public deployment URLs, screenshots, and the original chat export are external evidence. They must be supplied by the repository owner and must not be fabricated or committed as secrets.
