using GarageInventoryPartsFinder.Application.Advanced;

namespace GarageInventoryPartsFinder.Tests;

public class AdvancedFeaturePlannerTests
{
    [Fact]
    public void BuildReturnsProjectSpecificAdvancedFeatureSnapshot()
    {
        var snapshot = AdvancedFeaturePlanner.Build(
            "Garage Inventory And Parts Finder",
            "Compatibility checker for vehicles, parts, and supplier options.",
            "Part",
            4,
            7,
            ["Demo record", "Second record"]);

        Assert.Equal("Compatibility checker for vehicles, parts, and supplier options.", snapshot.FeatureName);
        Assert.Equal("Part", snapshot.EntityName);
        Assert.Equal(4, snapshot.ConnectedRecords);
        Assert.Contains("Demo record", snapshot.RecentRecords);
        Assert.NotEmpty(snapshot.NextActions);
    }

    [Fact]
    public void ReadinessScoreIncreasesWhenWorkflowHasMoreRecords()
    {
        var emptySnapshot = AdvancedFeaturePlanner.Build(
            "Garage Inventory And Parts Finder",
            "Compatibility checker for vehicles, parts, and supplier options.",
            "Part",
            0,
            7,
            []);

        var activeSnapshot = AdvancedFeaturePlanner.Build(
            "Garage Inventory And Parts Finder",
            "Compatibility checker for vehicles, parts, and supplier options.",
            "Part",
            8,
            7,
            ["Ready item"]);

        Assert.True(activeSnapshot.ReadinessScore > emptySnapshot.ReadinessScore);
    }
}