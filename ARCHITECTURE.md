# Architectural Decision Records (ADR) — Claims Greenfield Slice

This document details the architectural decisions, structural taxonomy, and underlying patterns implemented in the DICEUS Claims Module enterprise delivery simulator.

---

## 1. Architectural Taxonomy (Clean Architecture)

The solution strictly adheres to DDD-infused Clean Architecture principles. Dependencies flow inwards toward the core enterprise domain.

┌──────────────────────────────────────────────────────────────────┐
│                         ClaimsModule.API                         │
│  (Controllers, Web API Config, JWT Emulation, Problem Details)   │
└────────────────────────────────┬─────────────────────────────────┘
│
▼
┌──────────────────────────────────────────────────────────────────┐
│                     ClaimsModule.Application                     │
│  (CQRS Commands/Queries, MediatR Behaviors, FluentValidation)    │
└────────────────────────────────┬─────────────────────────────────┘
│
▼
┌──────────────────────────────────────────────────────────────────┐
│                       ClaimsModule.Domain                        │
│  (Aggregates, Entities, Value Objects, Pure Domain Events)        │
└──────────────────────────────────────────────────────────────────┘
▲
│
┌────────────────────────────────┴─────────────────────────────────┐
│              ClaimsModule.Persistence & Infrastructure            │
│  (EF Core 9, SQL Server, Unit of Work, Hangfire Jobs, Azure Blob)│
└──────────────────────────────────────────────────────────────────┘


* **ClaimsModule.Domain:** Zero external dependencies. Houses the `Claim` Aggregate Root, capturing insurance concepts natively.
* **ClaimsModule.Application:** Pure business orchestration using the Mediator pattern. Contains declarative handlers and pipeline pipelines.
* **ClaimsModule.Persistence:** Encapsulates entity tracking, automated audit column interception, tenant global filtering, and Unit of Work database transactions.
* **ClaimsModule.Infrastructure:** Bridges runtime requirements to physical IO (Hangfire execution frames, Azure Blob Storage integrations with short-lived SAS engines).
* **ClaimsModule.API:** Thin ingress layer executing routing, HTTP response parsing via RFC 7807, and dependency mapping.

---

## 2. Pattern Implementations

### CQRS Engine (MediatR)
State manipulation and read evaluations are decoupled cleanly. 
* **Commands** wrap atomic behavioral intentions (e.g., `OpenReserveComponentCommand`). They utilize explicit transaction scopes via an ambient Unit of Work pipeline.
* **Queries** projection models bypass tracking mechanics completely (`.AsNoTracking()`) and pull data utilizing specialized DTO footprints engineered for dashboard optimization.

### Event-Sourced Reserve Ledgers
To safeguard accounting integrity, `ClaimReserveComponents` are never subjected to raw state updates. Balance modification follows an append-only architecture:
$$CurrentAmount = \sum ApprovedAmounts_{TransactionHistory}$$
Every allocation changes history data frames; recalculation triggers occur sequentially via persistent database state mapping.

### Asynchronous Distributed Ingress (Hangfire)
Processing heavy side-effects (such as General Ledger simulations) runs completely out-of-process via Hangfire.
* **Distributed Idempotency:** A composite idempotency token (`Reserve:{Id}:Change:{Seq}`) is structurally validated against an append-only persistent record lock prior to state modification to ensure execution frames remain entirely re-entrant safe during pipeline cluster failures.

---

## 3. Comprehensive Data Modeling Strategy

* **Tenant Isolation:** Enforced transparently at the database execution layer. Every entity links to an `OrganisationId` Guid. The `DbContext` configures a global entity query abstraction rule ensuring multi-tenant leakages are completely impossible:
  `modelBuilder.Entity<Claim>().HasQueryFilter(x => x.OrganisationId == _tenantProvider.GetTenantId());`
* **Concurrency Safeguards:** Aggregate boundaries maintain an intrinsic `ROWVERSION` (`byte[]`) field. Overlapping high-frequency operations trigger `DbUpdateConcurrencyException` failures, avoiding standard race condition vulnerability vectors.

---

## 4. Key Engineering Trade-offs & Decisions

| Decision | Rationale & Trade-off |
| :--- | :--- |
| **Pipeline Validation vs Controller Checks** | Placed FluentValidation inside a MediatR `IPipelineBehavior`. **Trade-off:** Minimal overhead per request; **Benefit:** Completely eliminates duplicated parsing inside controllers, keeping them exceptionally clean. |
| **RFC 7807 Problem Details** | Standardized errors into JSON format profiles using an execution middleware. **Benefit:** Translates native business and validation alerts seamlessly to Angular snackbars with 1:1 mapping accuracy. |
| **IStorageService Polymorphism** | Abstracted external storage structures. **Benefit:** Allows developers to operate locally without live Cloud access (`LocalFileSystem` provider fallback) while shifting to fully managed `AzureBlobStorage` inside cloud containers. |
| **Atomic Counters over UUID Generators** | Constructed an isolated `ClaimNumberSequence` database sequence allocation routine. **Benefit:** Guarantees sequence completeness for compliance auditing, preventing tracking gaps under intensive, concurrent claim creations. |
| **Target Runtime Upgrade (.NET 9)** | Elevated the system runtime compilation constraints directly to **.NET 9** and **C# 13**. **Benefit:** Leverages modern system optimizations, enhanced LINQ execution vector mappings, and structural primary constructor mechanics. |

---

## 5. Cloud Reference Architecture

[ Angular 18 Client ] ──( HTTPS + Bearer JWT )──> [ Azure App Service / Container ]
│
┌──────────────────────────────────────┴──────────────────────────────────────┐
▼                                      ▼                                      ▼
[ Azure SQL Database ]                 [ Azure Blob Storage ]                 [ Hangfire Background Workers ]
(Isolated Tenant Schemas, RowVer)           (Claim Attachments Container)             (Idempotent GL Transactions)