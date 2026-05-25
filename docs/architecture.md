# Architecture Plan

## Backend

Planned structure:

`	ext
backend/
  src/
    Api/
    Application/
    Domain/
    Infrastructure/
  tests/
`

Responsibilities:

- Api: controllers, middleware, Swagger, auth setup.
- Application: DTOs, services, validation, use cases.
- Domain: entities, enums, domain rules.
- Infrastructure: EF Core, repositories, auth/token services, seed data.
- 	ests: unit and integration tests.

## Frontend

Planned structure:

`	ext
frontend/
  src/
    components/
    features/
    routes/
    services/
    stores/
    types/
`

## Database

Initial database name:

`	ext
garage_inventory_parts_finder
`
