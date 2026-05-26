using GarageInventoryPartsFinder.Domain.Entities;

namespace GarageInventoryPartsFinder.Tests;

public class ArchitectureSmokeTests
{
    [Fact]
    public void DomainEntityTypeIsAvailable()
    {
        Assert.NotNull(typeof(User));
    }
}
