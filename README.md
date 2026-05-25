# Garage Inventory And Parts Finder

> Portfolio-level Automotive Tech project planned as a 5-day .NET full-stack MVP.

## Objective

Build a garage inventory platform for tracking parts, tools, suppliers, compatibility, VIN metadata, and stock location.

## Target User

Garage owners and car enthusiasts tracking parts, tools, compatibility, and suppliers.

## Main Workflow

The user adds vehicles and parts, marks compatibility, tracks stock location and supplier, then searches for parts that fit a selected car.

## Advanced Portfolio Feature

Compatibility checker for vehicles, parts, and supplier options.

## Recommended Stack

ASP.NET Core Web API, React, TypeScript, PostgreSQL, EF Core, JWT auth.

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
- First Day 1 task started: objective, target user, workflow, stack, and entities are documented.
- Next task: create the real ASP.NET Core solution and frontend app.

## Local Development

Planned commands:

`ash
docker compose up -d
cd backend
dotnet restore
dotnet run
`

`ash
cd frontend
npm install
npm run dev
`

## Demo Notes

Add screenshots to docs/screenshots/ during Day 5.
