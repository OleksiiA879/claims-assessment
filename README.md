# Enterprise Claims Management

A working vertical slice of a multi-tenant claims platform. The solution combines a .NET 9 API, Angular 18 SPA, SQL Server persistence, Azure Blob-compatible document storage, and Hangfire background processing.

## Assessment coverage

| Area | Implemented |
|---|---|
| Backend | .NET 9/C# 13, Clean Architecture, MediatR CQRS, FluentValidation, EF Core 9, Hangfire, Swagger |
| Frontend | Angular 18, standalone lazy routes, Material, reactive FNOL stepper, typed service layer, JWT/error interceptors |
| Database | SQL Server/Azure SQL, reproducible EF migrations, seeded policies and cause-of-loss codes |
| Reserve ledger | Append-only history, computed balances, authority tiers, self-approval protection, manager override |
| Background processing | Re-entrant GL posting and idempotent SLA monitoring every 15 minutes |
| Storage | Local filesystem for development and private Azure Blob Storage with one-hour SAS URLs |
| Delivery | Multi-stage Docker image, Azure Bicep, build pipeline, and opt-in Azure deployment job |
| Documentation | Runbook, architecture decisions, AI workflow, and interaction history |

## Capabilities

- FNOL intake with policy-date validation, claimant and risk-object capture, and optional initial reserve.
- Claims list, detail, status transitions, audit history, parties, documents, and reserve management.
- Append-only reserve history with automatic, supervisor, and manager authority tiers.
- Idempotent GL posting and a 15-minute SLA monitor.
- Simulated JWT login and role switching for Handler, Supervisor, and Manager.
- OpenAPI/Swagger, RFC 7807 validation responses, EF Core migrations, Docker, Bicep, and GitHub Actions CI/CD.

## Solution structure

```text
src/
  ClaimsModule.Domain/          Entities, value objects, domain events and rules
  ClaimsModule.Application/     CQRS handlers, validators, DTOs and ports
  ClaimsModule.Persistence/     EF Core context, mappings, migrations and unit of work
  ClaimsModule.Infrastructure/  Blob/local storage and Hangfire jobs
  ClaimsModule.API/             Controllers, authentication, middleware and composition root
frontend/                       Angular 18 SPA
infra/                          Azure Bicep infrastructure
.github/workflows/              CI/CD pipeline
```

Dependencies point toward the domain: API, Persistence, and Infrastructure compose Application contracts without leaking EF Core or cloud SDKs into the domain.

## Prerequisites

- .NET SDK 9
- Node.js 20.11+ or 22
- Docker Desktop, or a reachable SQL Server 2022/Azure SQL instance

## Run locally

The quickest path starts SQL Server and the complete application through Docker:

```bash
docker compose up --build
```

Open:

- Application: <http://localhost:5000>
- Swagger: <http://localhost:5000/swagger>
- Health check: <http://localhost:5000/health>
- Hangfire dashboard (Development only): <http://localhost:5000/hangfire>

The database is created and seeded by EF Core migrations when the API starts. The compose password is a local-only development credential.

### Run without the application container

Start only SQL Server:

```bash
docker compose up -d sqlserver
```

Run the API:

```bash
dotnet restore ClaimsModule.sln
dotnet run --project src/ClaimsModule.API
```

Run the SPA in another terminal:

```bash
cd frontend
npm ci
npm start
```

The Angular development server uses `proxy.conf.json` to reach the API.

### Verify the build

```bash
dotnet restore ClaimsModule.sln
dotnet build ClaimsModule.sln --configuration Release --no-restore
dotnet test ClaimsModule.sln --configuration Release --no-build

cd frontend
npm ci
npm run build -- --configuration production
```

## Reference users

Call `POST /api/auth/login` with one of these usernames:

| Username | Role | Reserve authority |
|---|---|---|
| `handler` | Handler | Automatic approval through $10,000 |
| `supervisor` | Supervisor | Approval through $100,000 |
| `manager` | Manager | Approval above $100,000 |

The SPA role switcher performs this login and stores the returned development JWT. Self-approval remains prohibited.

> Authentication is intentionally simulated for this assessment. The password field accepts the demo value used by the SPA and is not a production identity implementation.

## Business rules

| Rule | Behavior |
|---|---|
| Future loss date | Rejected with HTTP 422 before the command handler runs |
| Loss outside policy dates | Stored as a warning; Draft is allowed but transition to Open is blocked |
| Claim without policy | Draft is allowed; reserve creation is blocked |
| Reserve up to $10,000 | Auto-approved and queued for GL posting |
| Reserve over $10,000 through $100,000 | Requires Supervisor or Manager |
| Reserve over $100,000 | Requires Manager |
| Aggregate reserves over $10,000,000 | Blocked unless a Manager records an audited override |
| Self-approval | Rejected with HTTP 422 |
| Reserve balance | Sum of approved/auto-approved history entries; no balance column is updated |
| Stale Draft/Open claim | SLA audit entry after 48 hours, at most once per 24 hours |

## Main API routes

| Area | Routes |
|---|---|
| Claims | `GET/POST /api/claims`, `GET /api/claims/{id}`, `PUT /api/claims/{id}/status` |
| Parties | `POST /api/claims/{id}/parties`, `DELETE /api/claims/{id}/parties/{partyId}` |
| Reserves | `GET/POST /api/claims/{id}/reserves`, approve, reject, and retract routes |
| Documents | `GET/POST /api/claims/{id}/documents`, authenticated download route |
| Policies | `GET /api/policies/search`, `GET /api/policies/{id}/coverage` |
| Reference | cause-of-loss and claim-status routes |

Swagger contains the complete request and response contracts.

Validation failures use `application/problem+json`:

```json
{
  "type": "ValidationError",
  "title": "One or more validation errors occurred.",
  "status": 422,
  "errors": {
    "LossDate": ["Loss date cannot be in the future."]
  }
}
```

## Configuration

Settings can be supplied through `src/ClaimsModule.API/appsettings.json` or standard ASP.NET Core environment variables:

| Setting | Purpose |
|---|---|
| `ConnectionStrings__DefaultConnection` | SQL Server/Azure SQL connection |
| `Storage__Provider` | `LocalFileSystem` or `AzureBlob` |
| `Storage__LocalPath` | Local document directory |
| `Storage__Azure__ConnectionString` | Blob Storage connection |
| `Storage__Azure__Container` | Private document container |
| `Jwt__Key` | JWT signing key |

Local files are served only through the authenticated API. Azure Blob downloads use one-hour, read-only SAS URLs.

## Database migrations

```bash
dotnet tool install --global dotnet-ef --version 9.0.5
dotnet ef database update \
  --project src/ClaimsModule.Persistence \
  --startup-project src/ClaimsModule.API
```

All schema and seed changes are represented by migrations. Monetary values use `decimal(19,4)`, timestamps use SQL Server `datetimeoffset`, aggregate roots use `rowversion`, and GUID keys use `NEWSEQUENTIALID()` defaults.

Claim numbers use `CLM-YYYY-NNNNNNN`. A SQL Server `MERGE ... WITH (HOLDLOCK)` increments the tenant/year counter inside the same transaction as claim creation.

## Azure deployment

`infra/main.bicep` provisions:

- Linux Azure App Service for the API and compiled Angular SPA
- Azure SQL logical server and database
- Private Azure Blob Storage container

Create a resource group, then deploy manually:

```bash
az group create --name claims-production --location westeurope
az deployment group create \
  --resource-group claims-production \
  --template-file infra/main.bicep \
  --parameters prefix=claimsprod \
               sqlAdministratorLogin=<login> \
               sqlAdministratorPassword=<password> \
               jwtSigningKey=<32-plus-character-secret>
```

For automated deployment, configure the GitHub `production` environment with:

| Type | Name |
|---|---|
| Secret | `AZURE_CLIENT_ID` |
| Secret | `AZURE_TENANT_ID` |
| Secret | `AZURE_SUBSCRIPTION_ID` |
| Secret | `SQL_ADMIN_LOGIN` |
| Secret | `SQL_ADMIN_PASSWORD` |
| Secret | `JWT_SIGNING_KEY` |
| Variable | `AZURE_RESOURCE_GROUP` |
| Variable | `AZURE_RESOURCE_PREFIX` |
| Variable | `AZURE_DEPLOY_ENABLED` (`true` enables deployment) |

The workflow builds both applications, embeds the Angular output in the API artifact, provisions Azure resources, deploys App Service, and lets the API apply migrations during startup.

## CI/CD behavior

- Pull requests and pushes to `main` run backend restore/build/test, Angular production build, Bicep validation, and artifact publication.
- Azure deployment is skipped safely until the repository variable `AZURE_DEPLOY_ENABLED` is set to `true`.
- Deployment uses GitHub OIDC rather than a stored Azure password.
- The API, SPA, migrations, and Hangfire worker deploy as one App Service artifact.

## Seeded demonstration data

- Active policies: `POL-2024-001001`, `POL-2024-001002`, `POL-2025-002001`, `POL-2025-002002`
- Expired policy for the warning scenario: `POL-2023-000099`
- Cause-of-loss references include fire, flood, theft, vehicle, liability, equipment, wind, injury, and other

## Troubleshooting

| Symptom | Resolution |
|---|---|
| `global.json` cannot find SDK 9 | Install the .NET 9 SDK and confirm with `dotnet --list-sdks` |
| API cannot connect immediately after Compose starts | Wait for the SQL Server health check; the API retries through its container restart policy |
| Angular API requests fail in development | Confirm the API is listening on port 5000 and start Angular with `npm start` |
| Azure deployment job is skipped | Set `AZURE_DEPLOY_ENABLED=true` and configure the listed environment secrets/variables |
| Blob provider fails at startup | Supply `Storage__Azure__ConnectionString` or select `LocalFileSystem` |

## Architecture and AI artifacts

- [ARCHITECTURE.md](ARCHITECTURE.md) documents boundaries, business rules, and trade-offs.
- [AI-WORKFLOW.md](AI-WORKFLOW.md) documents the AI-assisted engineering process.
- [AI-INTERACTION-HISTORY.md](AI-INTERACTION-HISTORY.md) records the auditable interaction summary without fabricated transcripts.
