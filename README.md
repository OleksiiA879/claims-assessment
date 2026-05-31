# Enterprise Claims Management System — Greenfield Vertical Slice

[![Build & Validation Pipeline](https://img.shields.io/badge/Build-.NET%209%20%7C%20Angular%2018-blueviolet?style=flat-square)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%252B%20CQRS-brightgreen?style=flat-square)](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/)

A production-oriented greenfield implementation simulating an enterprise-grade Policy Administration System (PAS) Claims Module. Built with **.NET 9 / C# 13** backend architecture and an **Angular 18 Enterprise Core** frontend, deployed and running on Microsoft Azure.

---

## 🚀 Key Functional Capabilities
* **Automated FNOL Intake:** Multi-step reactive wizard processing multi-party data, linked assets, policy date-boundary evaluations, and atomic document lifecycle management.
* **Distributed Processing Framework:** Resilient background operation workers executing idempotent General Ledger ledger simulations and SLA breach state machines.

---

## 🛠️ Technology Stack & Foundations

### Backend Core
* **Runtime & Framework:** .NET 9 (C# 13 features: Primary Constructors, Collection Expressions)
* **API Architecture:** Clean Architecture with CQRS via MediatR 12+
* **Persistence Engine:** EF Core 9 (SQL Server 2022), Unit of Work (UoW) Pattern
* **Business Validation:** Pipeline-level FluentValidation
* **Background Jobs:** Hangfire (SQL Server Storage Engine)
* **Document Services:** Azure Blob Storage with short-lived SAS URL distribution tokens

### Frontend Architecture
* **Framework:** Angular 18 (Signals-based state management, Lazy-loaded structures)
* **Design Engine:** Angular Material Enterprise Layout tokens
* **Form Ingress:** Advanced AbstractControl Reactive Form groups

---

## 💻 Local Development Setup

### 1. External Infrastructure Preparation
Spin up a local containerized SQL Server instance:
```bash
docker compose up -d
2. Microservice ConfigurationModify configuration directives inside src/ClaimsModule.API/appsettings.json:JSON{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DiceusClaimsDb;User Id=sa;Password=YourSecurePassword123!;TrustServerCertificate=True;"
  },
  "Storage": {
    "Provider": "LocalFileSystem", 
    "FallbackPath": "wwwroot/uploads"
  },
  "TenantSettings": {
    "DefaultOrganisationId": "8f3b9e21-4c1d-4a8a-9e22-3b4c5d6e7f8a"
  }
}
3. Initialize Database & Reference DataMigrations automatically run on startup. To manually interact with tools via CLI:Bashdotnet ef database update --project src/ClaimsModule.Persistence --startup-project src/ClaimsModule.API
Note: System applies immutable reference data seeding (Cause of Loss matrices, Policy simulations like POL-2024-001001) natively inside core migrations.4. Bootstrapping Runtime EnvironmentsBackend Engine Execution:Bashcd src/ClaimsModule.API
dotnet run
Interactive Swagger Playground initializes at: https://localhost:5001/swagger/index.htmlAngular UI Compilation:Bashcd frontend
npm install
npm start
Active local client frame hosts at: http://localhost:4200🔐 Identity Simulation & RBAC GatingThe application implements a secure JWT bearer interceptor workflow simulation. Authenticate against the application stack via POST /api/auth/login using the reference accounts below to validate role-based UI gating:Security IdentityAssigned RBAC RoleTarget Capability VectorshandlerClaims HandlerAccess FNOL forms, modify metadata, settle reserves $\le \$10,000$.supervisorClaims SupervisorAll handler rights + process authority approvals $\le \$100,000$.managerClaims ManagerFull override permissions, unlock capital limits up to $\$10,000,000$.