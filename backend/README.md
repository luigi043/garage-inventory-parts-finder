# Backend Scaffold

Status: Started

This is the first active implementation task for $title.

## Goal

Create the ASP.NET Core backend solution with a clean structure for API, application logic, domain models, infrastructure, and tests.

## Planned Commands

`ash
dotnet new sln -n GarageInventoryPartsFinder
dotnet new webapi -n GarageInventoryPartsFinder.Api -o src/GarageInventoryPartsFinder.Api
dotnet new classlib -n GarageInventoryPartsFinder.Application -o src/GarageInventoryPartsFinder.Application
dotnet new classlib -n GarageInventoryPartsFinder.Domain -o src/GarageInventoryPartsFinder.Domain
dotnet new classlib -n GarageInventoryPartsFinder.Infrastructure -o src/GarageInventoryPartsFinder.Infrastructure
dotnet new xunit -n GarageInventoryPartsFinder.Tests -o tests/GarageInventoryPartsFinder.Tests
`

## First Backend Deliverable

- Create solution and projects.
- Add project references.
- Add Swagger/OpenAPI.
- Add health check endpoint.
- Prepare EF Core persistence layer.
