using Microsoft.EntityFrameworkCore;
using GarageInventoryPartsFinder.Application.Authentication;
using GarageInventoryPartsFinder.Domain.Entities;
using GarageInventoryPartsFinder.Infrastructure.Persistence;

namespace GarageInventoryPartsFinder.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        GarageInventoryPartsFinderDbContext dbContext,
        PasswordHasher passwordHasher,
        CancellationToken cancellationToken = default)
    {
        const string demoEmail = "demo@garageinventorypartsfinder.local";
        var normalizedEmail = demoEmail.ToUpperInvariant();

        if (!await dbContext.AuthUsers.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken))
        {
            dbContext.AuthUsers.Add(new AuthUser
            {
                Email = demoEmail,
                NormalizedEmail = normalizedEmail,
                DisplayName = "Demo User",
                PasswordHash = passwordHasher.HashPassword("Passw0rd!")
            });
        }

        if (!await dbContext.Set<Part>().AnyAsync(cancellationToken))
        {
            dbContext.Set<Part>().Add(new Part
            {
                Name = "Garage Inventory And Parts Finder demo record",
                Notes = "Seed data created during Day 2 setup."
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
