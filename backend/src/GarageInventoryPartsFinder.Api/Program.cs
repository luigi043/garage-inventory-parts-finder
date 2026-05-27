using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GarageInventoryPartsFinder.Api.Authentication;
using GarageInventoryPartsFinder.Application.Authentication;
using GarageInventoryPartsFinder.Application.Validation;
using GarageInventoryPartsFinder.Domain.Entities;
using GarageInventoryPartsFinder.Infrastructure.Persistence;
using GarageInventoryPartsFinder.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddDbContext<GarageInventoryPartsFinderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://127.0.0.1:5173",
                "https://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", timestampUtc = DateTime.UtcNow }));

var auth = app.MapGroup("/api/auth");

auth.MapPost("/register", async (
    RegisterRequest request,
    GarageInventoryPartsFinderDbContext dbContext,
    PasswordHasher passwordHasher,
    TokenService tokenService,
    CancellationToken cancellationToken) =>
{
    var errors = AuthRequestValidator.ValidateRegister(request);
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var normalizedEmail = request.Email.Trim().ToUpperInvariant();
    if (await dbContext.AuthUsers.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken))
    {
        return Results.Conflict(new { message = "A user with this email already exists." });
    }

    var user = new AuthUser
    {
        Email = request.Email.Trim(),
        NormalizedEmail = normalizedEmail,
        DisplayName = request.DisplayName.Trim(),
        PasswordHash = passwordHasher.HashPassword(request.Password)
    };

    var refreshToken = tokenService.CreateRefreshToken();
    user.RefreshTokens.Add(new RefreshToken
    {
        TokenHash = tokenService.HashRefreshToken(refreshToken),
        ExpiresAtUtc = tokenService.GetRefreshTokenExpiryUtc()
    });

    dbContext.AuthUsers.Add(user);
    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/auth/users/{user.Id}", tokenService.CreateResponse(user, refreshToken));
});

auth.MapPost("/login", async (
    LoginRequest request,
    GarageInventoryPartsFinderDbContext dbContext,
    PasswordHasher passwordHasher,
    TokenService tokenService,
    CancellationToken cancellationToken) =>
{
    var errors = AuthRequestValidator.ValidateLogin(request);
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var normalizedEmail = request.Email.Trim().ToUpperInvariant();
    var user = await dbContext.AuthUsers
        .Include(item => item.RefreshTokens)
        .SingleOrDefaultAsync(item => item.NormalizedEmail == normalizedEmail, cancellationToken);

    if (user is null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    var refreshToken = tokenService.CreateRefreshToken();
    user.RefreshTokens.Add(new RefreshToken
    {
        TokenHash = tokenService.HashRefreshToken(refreshToken),
        ExpiresAtUtc = tokenService.GetRefreshTokenExpiryUtc()
    });

    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok(tokenService.CreateResponse(user, refreshToken));
});

auth.MapPost("/refresh", async (
    RefreshTokenRequest request,
    GarageInventoryPartsFinderDbContext dbContext,
    TokenService tokenService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["refreshToken"] = ["Refresh token is required."]
        });
    }

    var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
    var storedToken = await dbContext.RefreshTokens
        .Include(item => item.AuthUser)
        .SingleOrDefaultAsync(item => item.TokenHash == tokenHash, cancellationToken);

    if (storedToken?.AuthUser is null || !storedToken.IsActive)
    {
        return Results.Unauthorized();
    }

    storedToken.RevokedAtUtc = DateTime.UtcNow;
    var refreshToken = tokenService.CreateRefreshToken();
    storedToken.AuthUser.RefreshTokens.Add(new RefreshToken
    {
        TokenHash = tokenService.HashRefreshToken(refreshToken),
        ExpiresAtUtc = tokenService.GetRefreshTokenExpiryUtc()
    });

    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok(tokenService.CreateResponse(storedToken.AuthUser, refreshToken));
});

auth.MapPost("/logout", async (
    RefreshTokenRequest request,
    GarageInventoryPartsFinderDbContext dbContext,
    TokenService tokenService,
    CancellationToken cancellationToken) =>
{
    var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
    var storedToken = await dbContext.RefreshTokens.SingleOrDefaultAsync(item => item.TokenHash == tokenHash, cancellationToken);
    if (storedToken is not null)
    {
        storedToken.RevokedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    return Results.NoContent();
}).RequireAuthorization();

auth.MapGet("/me", async (ClaimsPrincipal principal, GarageInventoryPartsFinderDbContext dbContext, CancellationToken cancellationToken) =>
{
    var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userId, out var parsedUserId))
    {
        return Results.Unauthorized();
    }

    var user = await dbContext.AuthUsers.FindAsync(new object[] { parsedUserId }, cancellationToken);
    return user is null
        ? Results.Unauthorized()
        : Results.Ok(new CurrentUserResponse(user.Id, user.Email, user.DisplayName));
}).RequireAuthorization();

var workflowItems = app.MapGroup("/api/part").RequireAuthorization();

workflowItems.MapGet("/", async (GarageInventoryPartsFinderDbContext dbContext, CancellationToken cancellationToken) =>
{
    var items = await dbContext.Set<GarageInventoryPartsFinder.Domain.Entities.Part>()
        .OrderByDescending(item => item.CreatedAtUtc)
        .Take(50)
        .ToListAsync(cancellationToken);

    return Results.Ok(items);
});

workflowItems.MapGet("/{id:guid}", async (
    Guid id,
    GarageInventoryPartsFinderDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    var item = await dbContext.Set<GarageInventoryPartsFinder.Domain.Entities.Part>().FindAsync(new object[] { id }, cancellationToken);
    return item is null
        ? Results.NotFound()
        : Results.Ok(item);
});

workflowItems.MapPost("/", async (
    CreateWorkflowItemRequest request,
    GarageInventoryPartsFinderDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    var errors = AuthRequestValidator.ValidateName(request.Name);
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var item = new GarageInventoryPartsFinder.Domain.Entities.Part
    {
        Name = request.Name.Trim(),
        Notes = request.Notes?.Trim()
    };

    dbContext.Set<GarageInventoryPartsFinder.Domain.Entities.Part>().Add(item);
    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/part/{item.Id}", item);
});

workflowItems.MapPut("/{id:guid}", async (
    Guid id,
    CreateWorkflowItemRequest request,
    GarageInventoryPartsFinderDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    var errors = AuthRequestValidator.ValidateName(request.Name);
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var item = await dbContext.Set<GarageInventoryPartsFinder.Domain.Entities.Part>().FindAsync(new object[] { id }, cancellationToken);
    if (item is null)
    {
        return Results.NotFound();
    }

    item.Name = request.Name.Trim();
    item.Notes = request.Notes?.Trim();
    item.UpdatedAtUtc = DateTime.UtcNow;

    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok(item);
});
var advancedFeature = app.MapGroup("/api/advanced-feature").RequireAuthorization();

advancedFeature.MapGet("/", async (GarageInventoryPartsFinderDbContext dbContext, CancellationToken cancellationToken) =>
{
    var connectedRecords = await dbContext.Set<GarageInventoryPartsFinder.Domain.Entities.Part>().CountAsync(cancellationToken);
    var recentRecords = await dbContext.Set<GarageInventoryPartsFinder.Domain.Entities.Part>()
        .OrderByDescending(item => item.UpdatedAtUtc ?? item.CreatedAtUtc)
        .Take(5)
        .Select(item => item.Name)
        .ToListAsync(cancellationToken);

    var snapshot = GarageInventoryPartsFinder.Application.Advanced.AdvancedFeaturePlanner.Build(
        "Garage Inventory And Parts Finder",
        "Compatibility checker for vehicles, parts, and supplier options.",
        "Part",
        connectedRecords,
        8,
        recentRecords);

    return Results.Ok(snapshot);
});
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<GarageInventoryPartsFinderDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
        await DatabaseSeeder.SeedAsync(dbContext, passwordHasher);
    }
    catch (Exception exception)
    {
        logger.LogWarning(exception, "Development seed data could not be applied. Start PostgreSQL and run migrations to enable seeding.");
    }
}

app.Run();

public sealed record CreateWorkflowItemRequest(string Name, string? Notes);
