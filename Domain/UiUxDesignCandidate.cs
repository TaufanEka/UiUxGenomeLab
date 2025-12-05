namespace UiUxGenomeLab.Domain;

public sealed class UiUxDesignCandidate
{
    public required string Id { get; init; }
    public required string Name { get; init; }

    // High-level description of the concept
    public required string Summary { get; init; }

    // JSON-serializable spec – output from OpenAI structured response
    public required UiUxDesignSpec Spec { get; init; }

    // Scores (heuristics + model-evaluated)
    public double UsabilityScore { get; set; }
    public double AccessibilityScore { get; set; }
    public double VisualClarityScore { get; set; }
    public double ImplementationComplexityScore { get; set; }
    public double OverallFitness =>
        (UsabilityScore * 0.35) +
        (AccessibilityScore * 0.25) +
        (VisualClarityScore * 0.25) -
        (ImplementationComplexityScore * 0.15);

    // Trace
    public string? EvaluationRationale { get; set; }
    public string? SourcePrompt { get; set; }
    public string? RefinedQuestion { get; set; }
}

// A structured spec that you get from OpenAI as JSON via Responses API
public sealed class UiUxDesignSpec
{
    public required string LayoutPattern { get; init; }            // e.g. "single-column wizard"
    public required string NavigationPattern { get; init; }        // e.g. "bottom nav", "tabs"
    public required string ColorPalette { get; init; }             // e.g. tokens or Tailwind-like notation
    public required string TypographyScale { get; init; }          // e.g. "Display / H1 / body / caption"
    public required string ComponentLibraryStyle { get; init; }    // e.g. "Material-ish", "Neumorphic"
    public required string InteractionNotes { get; init; }         // micro-interactions, transitions
    public required string AccessibilityNotes { get; init; }       // contrast, focus states, etc.

    // You can expand this for your stack, e.g. constraints for React/Tailwind/etc.
}
