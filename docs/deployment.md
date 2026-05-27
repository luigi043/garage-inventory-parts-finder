# Deployment Notes

## Services

- Database: PostgreSQL.
- Backend: ASP.NET Core Web API.
- Frontend: React static build.

## Required Environment Variables

~~~text
ConnectionStrings__DefaultConnection=...
Jwt__Issuer=garage-inventory-parts-finder
Jwt__Audience=garage-inventory-parts-finder-client
Jwt__SigningKey=<strong-production-secret>
VITE_API_BASE_URL=<deployed-api-url>
~~~

## Recommended Hosting

- Azure Database for PostgreSQL for the database.
- Azure App Service or Azure Container Apps for the API.
- Azure Static Web Apps, Vercel, or Netlify for the frontend.

## Release Checklist

- Run dotnet test GarageInventoryPartsFinder.slnx.
- Run 
pm run build in rontend.
- Apply EF Core migrations.
- Configure CORS to allow the deployed frontend URL.
- Replace all development secrets.