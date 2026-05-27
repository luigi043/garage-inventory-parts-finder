namespace GarageInventoryPartsFinder.Application.Advanced;

public sealed record AdvancedFeatureSnapshot(
    string ProjectName,
    string FeatureName,
    string EntityName,
    int ConnectedRecords,
    int ModuleCount,
    int ReadinessScore,
    string Maturity,
    IReadOnlyList<string> Highlights,
    IReadOnlyList<string> NextActions,
    IReadOnlyList<string> RecentRecords,
    DateTime GeneratedAtUtc);

public static class AdvancedFeaturePlanner
{
    public static AdvancedFeatureSnapshot Build(
        string projectName,
        string featureName,
        string entityName,
        int connectedRecords,
        int moduleCount,
        IEnumerable<string> recentRecords)
    {
        var cleanRecentRecords = recentRecords
            .Where(record => !string.IsNullOrWhiteSpace(record))
            .Select(record => record.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(5)
            .ToArray();

        var readinessScore = Math.Clamp(
            30 + Math.Min(connectedRecords, 12) * 5 + Math.Min(moduleCount, 10) * 2,
            0,
            95);

        var maturity = connectedRecords switch
        {
            0 => "Design ready",
            < 3 => "Prototype ready",
            < 8 => "Workflow ready",
            _ => "Demo ready"
        };

        var highlights = new[]
        {
            $"Tracks {connectedRecords} protected {entityName} records.",
            $"Connects {moduleCount} planned modules to the Day 4 feature concept.",
            cleanRecentRecords.Length > 0
                ? $"Latest signal: {cleanRecentRecords[0]}."
                : "Waiting for the first workflow record to produce stronger signals."
        };

        var nextActions = new[]
        {
            $"Build the first interactive slice of {featureName}.",
            $"Use {entityName} activity as the live input for the feature.",
            "Add one Day 5 screenshot showing this advanced panel with real data."
        };

        return new AdvancedFeatureSnapshot(
            projectName,
            featureName,
            entityName,
            connectedRecords,
            moduleCount,
            readinessScore,
            maturity,
            highlights,
            nextActions,
            cleanRecentRecords,
            DateTime.UtcNow);
    }
}