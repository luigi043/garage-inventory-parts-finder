# Testing Guide

## Backend

~~~powershell
cd backend
dotnet test GarageInventoryPartsFinder.slnx
~~~

Current test coverage includes:

- Password hashing.
- Request validation.
- Advanced feature planner snapshot generation.
- Readiness score behavior.

## Frontend

~~~powershell
cd frontend
npm install
npm run build
~~~

The frontend build verifies TypeScript types and creates a production Vite bundle.

## Manual Smoke Test

1. Start the database and API.
2. Start the frontend.
3. Sign in as $(@{RepoName=garage-inventory-parts-finder; ProjectName=GarageInventoryPartsFinder; SolutionName=GarageInventoryPartsFinder.slnx; Title=Garage Inventory And Parts Finder; Objective=Build a garage inventory platform for tracking parts, tools, suppliers, compatibility, VIN metadata, and stock location.; TargetUser=Garage owners and car enthusiasts tracking parts, tools, compatibility, and suppliers.; Workflow=The user adds vehicles and parts, marks compatibility, tracks stock location and supplier, then searches for parts that fit a selected car.; Advanced=Compatibility checker for vehicles, parts, and supplier options.; CoreEntities=System.Object[]; EntityClass=Part; EntityName=Part; EntityRoute=part; DemoEmail=demo@garageinventorypartsfinder.local; DemoPassword=Passw0rd!; DatabaseName=garage_inventory_parts_finder; DatabasePort=5422; ApiBaseUrl=http://localhost:5000}.DemoEmail).
4. Create and edit a Part record.
5. Refresh the Operations advanced feature panel.