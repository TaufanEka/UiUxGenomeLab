using System.Text.Json;
using UiUxGenomeLab.Domain;

namespace UiUxGenomeLab.Services;

// Use the local HTTP wrapper to call OpenAI Responses API so code compiles reliably with the installed package.
public sealed class OpenAiDesignClient
{
    private readonly OpenAiHttpClient _responses;
    private readonly string _designModel;

    public OpenAiDesignClient(IConfiguration config, OpenAiHttpClient responses)
    {
        _designModel = config[""OpenAI:Model""] ?? ""gpt-4.1"";
        _responses = responses;
    }

    /// <summary>
    /// Generate a batch of candidate designs in one Responses call,
    /// using structured outputs to reduce cost.
    /// </summary>
    public async Task<IReadOnlyList<UiUxDesignCandidate>> GenerateInitialPopulationAsync(
        UiUxResearchConfig config,
        int generationIndex,
        int populationSize,
        CancellationToken ct)
    {
        var schemaExample = new
        {
            candidates = new[]
            {
                new {
                    name = """",
                    summary = """",
                    layoutPattern = """",
                    navigationPattern = """",
                    colorPalette = """",
                    typographyScale = """",
                    componentLibraryStyle = """",
                    interactionNotes = """",
                    accessibilityNotes = """"
                }
            }
        };

        string prompt =
            $""You are a senior UI/UX designer. Generate {populationSize} distinct, "" +
            $""high-quality UI/UX layout concepts for this problem. Return a single JSON object with a 'candidates' array that matches the schema example.\n\n"" +
            $""Schema example: {JsonSerializer.Serialize(schemaExample)}\n\n"" +
            $""Problem: {config.ProblemStatement}\n"" +
            $""Target platform: {config.TargetPlatform}\n"" +
            $""Brand tone: {config.BrandTone}\n\n"" +
            ""Focus on diversity of layout patterns, navigation, and visual styles to maximize design space exploration."";

        var json = await _responses.CreateTextResponseAsync(prompt, ct);

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException(""OpenAI returned empty design population."");

        using var parsed = JsonDocument.Parse(json);
        if (!parsed.RootElement.TryGetProperty(""candidates"", out var root))
            throw new InvalidOperationException(""OpenAI returned JSON that doesn't contain 'candidates'."");

        var list = new List<UiUxDesignCandidate>(populationSize);
        int i = 0;
        foreach (var item in root.EnumerateArray())
        {
            var spec = new UiUxDesignSpec
            {
                LayoutPattern = item.GetProperty(""layoutPattern"").GetString() ?? string.Empty,
                NavigationPattern = item.GetProperty(""navigationPattern"").GetString() ?? string.Empty,
                ColorPalette = item.GetProperty(""colorPalette"").GetString() ?? string.Empty,
                TypographyScale = item.GetProperty(""typographyScale"").GetString() ?? string.Empty,
                ComponentLibraryStyle = item.GetProperty(""componentLibraryStyle"").GetString() ?? string.Empty,
                InteractionNotes = item.GetProperty(""interactionNotes"").GetString() ?? string.Empty,
                AccessibilityNotes = item.GetProperty(""accessibilityNotes"").GetString() ?? string.Empty
            };

            list.Add(new UiUxDesignCandidate
            {
                Id = $""gen{generationIndex}-cand{i:000}"",
                Name = item.GetProperty(""name"").GetString() ?? $""Concept {i + 1}"",
                Summary = item.GetProperty(""summary"").GetString() ?? string.Empty,
                Spec = spec,
                SourcePrompt = prompt
            });

            i++;
        }

        return list;
    }

    /// <summary>
    /// Ask OpenAI to heuristically score a batch of candidates.
    /// </summary>
    public async Task ScoreCandidatesAsync(
        IReadOnlyList<UiUxDesignCandidate> candidates,
        UiUxResearchConfig config,
        CancellationToken ct)
    {
        var compact = candidates.Select(c => new
        {
            c.Id,
            c.Name,
            c.Summary,
            c.Spec.LayoutPattern,
            c.Spec.NavigationPattern,
            c.Spec.ColorPalette,
            c.Spec.TypographyScale,
            c.Spec.ComponentLibraryStyle,
            c.Spec.InteractionNotes,
            c.Spec.AccessibilityNotes
        });

        string prompt =
            ""You are evaluating multiple UI/UX design concepts for the same product.\n"" +
            ""For each candidate, score on:\n"" +
            ""- usability (0–10)\n"" +
            ""- accessibility (0–10)\n"" +
            ""- visual clarity (0–10)\n"" +
            ""- implementation complexity (0–10, lower is better)\n\n"" +
            ""Return JSON with an array 'scores' where each item has:\n"" +
            ""{ id, usability, accessibility, visual_clarity, implementation_complexity, rationale }.\n\n"" +
            ""Context:\n"" +
            $""Problem: {config.ProblemStatement}\n"" +
            $""Target platform: {config.TargetPlatform}\n"" +
            $""Brand tone: {config.BrandTone}\n\n"" +
            ""Candidates:\n"" +
            JsonSerializer.Serialize(compact);

        var json = await _responses.CreateTextResponseAsync(prompt, ct);

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException(""OpenAI returned empty scoring response."");

        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty(""scores"", out var scores))
            throw new InvalidOperationException(""Scoring response missing 'scores' array."");

        var byId = candidates.ToDictionary(c => c.Id, c => c);
        foreach (var s in scores.EnumerateArray())
        {
            var id = s.GetProperty(""id"").GetString();
            if (id is null || !byId.TryGetValue(id, out var cand)) continue;

            if (s.TryGetProperty(""usability"", out var u)) cand.UsabilityScore = u.GetDouble();
            if (s.TryGetProperty(""accessibility"", out var a)) cand.AccessibilityScore = a.GetDouble();
            if (s.TryGetProperty(""visual_clarity"", out var v)) cand.VisualClarityScore = v.GetDouble();
            if (s.TryGetProperty(""implementation_complexity"", out var ic)) cand.ImplementationComplexityScore = ic.GetDouble();
            if (s.TryGetProperty(""rationale"", out var r)) cand.EvaluationRationale = r.GetString();
        }
    }
}
