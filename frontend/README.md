# Claims Management Frontend

Angular 18 SPA for the Claims Management module, using Angular Material with an indigo primary theme.

## Prerequisites

- Node.js 18+
- Backend API running at `http://localhost:5000`

## Setup

```bash
cd frontend
npm install
npm start
```

Open `http://localhost:4200`.

## Features

- **Claims list** — paginated table with status, date range, and cause-of-loss filters; color-coded status badges
- **FNOL intake** — 3-step reactive form with Material stepper (loss details, parties, risk & reserve)
- **Claim detail** — tabs: Overview, Parties, Reserves, Documents, Audit
- **Mock auth** — role switcher (handler / supervisor / manager) in the header; JWT stored in `localStorage`
- **HTTP interceptors** — Bearer token, global loading overlay, error snackbars

## Project structure

```
src/app/
  core/           models, services, interceptors
  features/claims/  lazy routes: list, detail, fnol-intake
  layout/         header with role switcher
  shared/         pipes, loading overlay
```

## API

All HTTP calls go through `AuthService`, `ClaimsService`, `PolicyService`, and `ReferenceService`. Base URL is set in `src/environments/`.
