namespace UiUxGenomeLab.Domain;

public sealed class UiUxResearchResult
{
    public required string JobId { get; init; }
    public required DateTimeOffset StartedAtUtc { get; init; }
    public DateTimeOffset? CompletedAtUtc { get; set; }

    public List<UiUxDesignCandidate> AllCandidates { get; } = new();
    public UiUxDesignCandidate? BestCandidate { get; set; }

    public string ResearchBundlePath { get; set; } = string.Empty;
    public string IndexHtmlPath { get; set; } = string.Empty;
}