using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GarageInventoryPartsFinder.Infrastructure.Persistence;

public class DesignTimeGarageInventoryPartsFinderDbContextFactory : IDesignTimeDbContextFactory<GarageInventoryPartsFinderDbContext>
{
    public GarageInventoryPartsFinderDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=garage_inventory_parts_finder;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<GarageInventoryPartsFinderDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new GarageInventoryPartsFinderDbContext(options);
    }
}
