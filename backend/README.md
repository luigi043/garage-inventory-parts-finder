# Backend

Status: Day 1 complete.

## Created Structure

`	ext
backend/
  GarageInventoryPartsFinder.slnx
  src/
    GarageInventoryPartsFinder.Api/
    GarageInventoryPartsFinder.Application/
    GarageInventoryPartsFinder.Domain/
    GarageInventoryPartsFinder.Infrastructure/
  tests/
    GarageInventoryPartsFinder.Tests/
`

## Project References

`	ext
Api -> Application, Infrastructure
Application -> Domain
Infrastructure -> Application, Domain
Tests -> Application, Domain
`

## Persistence

- EF Core provider: Npgsql for PostgreSQL.
- DbContext: $ProjectName.Infrastructure.Persistence.GarageInventoryPartsFinderDbContext
- Migration: InitialCreate

## Commands

`ash
dotnet restore GarageInventoryPartsFinder.slnx
dotnet build GarageInventoryPartsFinder.slnx
dotnet ef database update --project src/GarageInventoryPartsFinder.Infrastructure --startup-project src/GarageInventoryPartsFinder.Api --context GarageInventoryPartsFinderDbContext
dotnet run --project src/GarageInventoryPartsFinder.Api
`
