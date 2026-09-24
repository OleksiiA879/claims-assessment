# Enterprise Claims Management

A working vertical slice of a multi-tenant claims platform. The solution combines a .NET 9 API, Angular 18 SPA, SQL Server persistence, Azure Blob-compatible document storage, and Hangfire background processing.

## Capabilities

- FNOL intake with policy-date validation, claimant and risk-object capture, and optional initial reserve.
- Claims list, detail, status transitions, audit history, parties, documents, and reserve management.
- Append-only reserve history with automatic, supervisor, and manager authority tiers.
- Idempotent GL posting and a 15-minute SLA monitor.
- Simulated JWT login and role switching for Handler, Supervisor, and Manager.
- OpenAPI/Swagger, RFC 7807 validation responses, EF Core migrations, Docker, Bicep, and GitHub Actions CI/CD.

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

To run each application separately:

```bash
docker compose up -d sqlserver
dotnet restore ClaimsModule.sln
dotnet run --project src/ClaimsModule.API
```

In another terminal:

```bash
cd frontend
npm ci
npm start
```

The Angular development server uses `proxy.conf.json` to reach the API.

## Reference users

Call `POST /api/auth/login` with one of these usernames:

| Username | Role | Reserve authority |
|---|---|---|
| `handler` | Handler | Automatic approval through $10,000 |
| `supervisor` | Supervisor | Approval through $100,000 |
| `manager` | Manager | Approval above $100,000 |

The SPA role switcher performs this login and stores the returned development JWT. Self-approval remains prohibited.

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

## Architecture and AI artifacts

- [ARCHITECTURE.md](ARCHITECTURE.md) documents boundaries, business rules, and trade-offs.
- [AI-WORKFLOW.md](AI-WORKFLOW.md) documents the AI-assisted engineering process.
- [AI-INTERACTION-HISTORY.md](AI-INTERACTION-HISTORY.md) records the auditable interaction summary without fabricated transcripts.
