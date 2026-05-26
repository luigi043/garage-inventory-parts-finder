namespace GarageInventoryPartsFinder.Application.Authentication;

public sealed record RegisterRequest(string Email, string Password, string DisplayName);

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc);

public sealed record CurrentUserResponse(Guid UserId, string Email, string DisplayName);
