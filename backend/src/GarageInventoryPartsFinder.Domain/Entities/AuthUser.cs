namespace GarageInventoryPartsFinder.Domain.Entities;

public class AuthUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Email { get; set; } = string.Empty;

    public string NormalizedEmail { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
