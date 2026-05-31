# AI-Native Delivery & Workflow Report

This report outlines the orchestration strategy, prompt structuring, and collaborative engineering cycle between the candidate and AI tools (Claude 3.5 Sonnet / Cursor) to deliver the Claims Greenfield Vertical Slice.

---

## 1. AI Tooling & Context Strategy
*  Claude 3.5 Sonnet 
* **Context Loading Strategy:** The raw Functional Requirements Specification (FRS) and Technical Assessment PDF were parsed into a dedicated markdown context file. This file was attached to the file to ensure all codebase generations conformed strictly to DICEUS structural constraints, naming conventions, and tenant isolation policies from the first prompt.

---

## 2. Dynamic Workflow Execution

[ FRS Context ] ──> [ Domain Models & Value Objects ] ──> [ EF Core Fluent API ]
│
[ Angular Scaffold ] <─── [ MediatR Pipelines & Handlers ] <───────┘


1. **Domain-Driven Architecture First:** Modeled pure domain abstractions (`Claim`, `ReserveComponent`) and immutable value objects before generating any boilerplate application services.
2. **Behavior-Driven Prompting:** Instead of asking for "CRUD APIs," prompts specified the boundaries of MediatR commands, their mapping to business rules (`BR-C-*`), and explicit output expectations.
3. **Parallelized Agent Execution:** Utilized Cursor Agent workflows to scaffold the Angular 18 Material layouts, reactive forms state structures, and API client abstractions concurrently while the backend test suites and compilation checks were running locally.

---

## 3. Representative Prompts & Intent

### Prompt 1: Multi-Step Aggregate Strategy
> *"Act as a Principal Software Architect. Given the attached Claims FRS, design the `Claims` aggregate root in `ClaimsModule.Domain`. Ensure that child records like `LossEvents`, `ClaimParties`, and `ClaimRiskObjects` are encapsulated. Expose state-changing domain methods (e.g., `TransitionToStatus`, `AddParty`) that safeguard domain invariants. Do not leak EF Core dependencies or primitive obsession into this layer."*

### Prompt 2: Idiomatic MediatR Pipeline & Custom Validation
> *"Implement the `CreateClaimCommand` handler using MediatR. Wire up an automatic pipeline behavior utilizing FluentValidation. If field-level rules `BR-C-01` through `BR-C-07` violate business constraints, throw a custom `ValidationException` containing a structured dictionary of severe vs warning alerts. The execution path must fail fast before entering the handler."*

### Prompt 3: Re-entrant Background Processing
> *"Generate a Hangfire background job `PostGLReserveChangeJob`. It must accept an idempotency key structured as `Reserve:{ReserveId}:Change:{ChangeSequence}`. Implement a strict double-check pattern against the persistence store: if the `PostingStatus` equals `Posted`, the job must immediately terminate as a No-Op. Ensure explicit database transaction locking to prevent race conditions during distributed automatic retries."*

---

## 4. AI Hallucinations & Manual Refinements

While AI accelerated boilerplate production, critical engineering intervention was required to maintain production-grade integrity:

| Aspect / Hallucination | AI-Generated Flaw | Manual Senior Refinement |
| :--- | :--- | :--- |
| **Package Alignment (.NET 9)** | Suggested mismatched `.NET 8` packages and legacy `MediatR` syntax. | Hard-pinned dependencies to unified **.NET 9** targets (`Microsoft.EntityFrameworkCore.SqlServer` v9.0.x, MediatR v12+). |
| **Data Seeding Determinism** | Generated `Guid.NewGuid()` inside EF Core `HasData()` seed configurations. | Replaced with deterministically generated, static Namespace-based GUIDs to ensure migrations are perfectly reproducible and hash-stable across environments. |
| **Aggregate Synchronization** | Updated `ClaimReserveComponents.CurrentAmount` via direct updates in the handler. | Re-architected to an **event-sourced transaction model**. The aggregate now dynamically computes balances by summing active history rows via query logic to ensure strict alignment with auditing. |
| **UI Role Gating** | Generated component-level visibility checks using primitive boolean flags. | Re-written using Angular **Signals** combined with a custom structural directive (`*appHasRole`) that integrates natively with the simulated JWT Auth Interceptor context. |

---

## 5. High-Value AI Benefits
* **Speed to Market:** Drastically cut down time spent on repetitive CQRS mapping profiles, boilerplate DTO declarations, and mock data generators.
* **UI Scaffolding:** Accelerated the implementation of verbose Angular Material forms, custom validation error extractors, and CSS Grid layouts.
