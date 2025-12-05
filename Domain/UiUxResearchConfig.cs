namespace UiUxGenomeLab.Domain;

public sealed class UiUxResearchConfig
{
    public required string ProblemStatement { get; init; } // what product/screen/flow
    public required string TargetPlatform { get; init; }   // "mobile", "desktop", "responsive"
    public required string BrandTone { get; init; }        // "playful", "calm", "enterprise"
    public required int PopulationSize { get; init; }      // e.g. 20–50 per generation
    public required int MaxGenerations { get; init; }      // e.g. 50
    public TimeSpan MaxDuration { get; init; }             // e.g. 8 hours

    public string OutputDirectory { get; init; } = "Output";
    public string ModelName { get; init; } = "gpt-4.1";
}
