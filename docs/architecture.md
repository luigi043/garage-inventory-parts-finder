# Architecture Plan

## Backend

Created structure:

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

Responsibilities:

- Api: controllers, middleware, Swagger/OpenAPI, auth setup.
- Application: DTOs, services, validation, use cases.
- Domain: entities, enums, domain rules.
- Infrastructure: EF Core, repositories, auth/token services, seed data.
- Tests: unit and integration tests.

## Frontend

Created structure:

`	ext
frontend/
  src/
    components/
    features/
    lib/
`

## Database

`	ext
garage_inventory_parts_finder
`

## Current Architecture Decision

The backend uses a layered solution so business rules can live outside controllers:

`	ext
Api -> Application -> Domain
Api -> Infrastructure -> Application, Domain
Tests -> Application, Domain
`
