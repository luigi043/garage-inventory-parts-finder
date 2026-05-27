# Demo Guide

## Login

~~~text
Email: demo@garageinventorypartsfinder.local
Password: Passw0rd!
~~~

## Suggested Walkthrough

1. Start PostgreSQL with docker compose up -d.
2. Apply migrations from the backend folder.
3. Start the API and frontend.
4. Sign in with the demo user.
5. Create a Part record.
6. Edit that record from the detail panel.
7. Open Operations and refresh the advanced feature readiness panel.

## What To Point Out

- JWT login and refresh-token-backed session recovery.
- Protected CRUD API for /api/part.
- Protected /api/advanced-feature endpoint.
- Application-layer planner and backend tests.
- Responsive React dashboard with loading, empty, and error states.