# Garage Inventory And Parts Finder

> Portfolio-level full-stack project planned as a 5-day .NET MVP.

## Objective

Build a garage inventory platform for tracking parts, tools, suppliers, compatibility, VIN metadata, and stock location.

## Target User

Garage owners and car enthusiasts tracking parts, tools, compatibility, and suppliers.

## Main Workflow

The user adds vehicles and parts, marks compatibility, tracks stock location and supplier, then searches for parts that fit a selected car.

## Advanced Portfolio Feature

Compatibility checker for vehicles, parts, and supplier options.

## Recommended Stack

ASP.NET Core Web API, React, TypeScript, PostgreSQL, EF Core, JWT auth, and Docker Compose.

## Core Entities

- User
- Vehicle
- Part
- PartCompatibility
- Supplier
- StockLocation
- PurchaseRecord
- Tool

## 5-Day MVP Plan

- Day 1: Scope, architecture, scaffold backend/frontend, database setup.
- Day 2: Backend auth, domain entities, validation, seed data, tests.
- Day 3: Frontend dashboard, forms, routing, API integration.
- Day 4: Advanced feature implementation and workflow polish.
- Day 5: Tests, screenshots, README, deployment notes, final polish.

## Current Status

- Repository scaffold created.
- Backend ASP.NET Core solution created in ackend/GarageInventoryPartsFinder.slnx.
- Frontend React TypeScript app scaffold created in rontend/.
- PostgreSQL Docker Compose setup added.
- EF Core persistence layer and InitialCreate migration added.
- Day 1 setup complete.

## Local Development

Copy environment defaults:

`ash
copy .env.example .env
`

Start PostgreSQL:

`ash
docker compose up -d
`

Backend:

`ash
cd backend
dotnet restore GarageInventoryPartsFinder.slnx
dotnet ef database update --project src/GarageInventoryPartsFinder.Infrastructure --startup-project src/GarageInventoryPartsFinder.Api --context GarageInventoryPartsFinderDbContext
dotnet run --project src/GarageInventoryPartsFinder.Api
`

Frontend:

`ash
cd frontend
npm install
npm run dev
`

## Database

- Database: $DatabaseName
- Host port: $Port
- Provider: PostgreSQL via Npgsql Entity Framework Core provider

## Demo Notes

Add screenshots to docs/screenshots/ during Day 5.
