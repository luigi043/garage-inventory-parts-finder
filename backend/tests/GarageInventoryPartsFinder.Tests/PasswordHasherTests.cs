using GarageInventoryPartsFinder.Application.Authentication;

namespace GarageInventoryPartsFinder.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void VerifyPasswordAcceptsMatchingPassword()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("Passw0rd!");

        Assert.True(hasher.VerifyPassword("Passw0rd!", hash));
    }

    [Fact]
    public void VerifyPasswordRejectsWrongPassword()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("Passw0rd!");

        Assert.False(hasher.VerifyPassword("wrong-password", hash));
    }
}
