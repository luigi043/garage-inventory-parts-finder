using Microsoft.EntityFrameworkCore;
using GarageInventoryPartsFinder.Domain.Entities;

namespace GarageInventoryPartsFinder.Infrastructure.Persistence;

public class GarageInventoryPartsFinderDbContext(DbContextOptions<GarageInventoryPartsFinderDbContext> options) : DbContext(options)
{
    public DbSet<AuthUser> AuthUsers => Set<AuthUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<User> UserRecords => Set<User>();
    public DbSet<Vehicle> VehicleRecords => Set<Vehicle>();
    public DbSet<Part> PartRecords => Set<Part>();
    public DbSet<PartCompatibility> PartCompatibilityRecords => Set<PartCompatibility>();
    public DbSet<Supplier> SupplierRecords => Set<Supplier>();
    public DbSet<StockLocation> StockLocationRecords => Set<StockLocation>();
    public DbSet<PurchaseRecord> PurchaseRecordRecords => Set<PurchaseRecord>();
    public DbSet<Tool> ToolRecords => Set<Tool>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<PartCompatibility>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<StockLocation>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<PurchaseRecord>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1000);
        });

        modelBuilder.Entity<Tool>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Notes).HasMaxLength(1000);
        });
        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Email).HasMaxLength(256).IsRequired();
            entity.Property(item => item.NormalizedEmail).HasMaxLength(256).IsRequired();
            entity.Property(item => item.DisplayName).HasMaxLength(120).IsRequired();
            entity.Property(item => item.PasswordHash).HasMaxLength(512).IsRequired();
            entity.HasIndex(item => item.NormalizedEmail).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.TokenHash).HasMaxLength(128).IsRequired();
            entity.HasIndex(item => item.TokenHash).IsUnique();
            entity.HasOne(item => item.AuthUser)
                .WithMany(user => user.RefreshTokens)
                .HasForeignKey(item => item.AuthUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}