using OpenAI;
using OpenAI.Responses;
using UiUxGenomeLab.Domain;

namespace UiUxGenomeLab.Services;

//Use the Responses API with web_search where needed.
public sealed class OpenAiDesignClient
{
    private readonly OpenAIResponseClient _responses;
    private readonly string _designModel;

    public OpenAiDesignClient(IConfiguration config)
    {
        var apiKey = config["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing OpenAI:ApiKey");

        _designModel = config["OpenAI:Model"] ?? "gpt-4.1";

        _responses = new OpenAIResponseClient(
            model: _designModel,
            apiKey: apiKey);
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
        // Describe schema (simplified) – you can refine schema for more attributes.
        var schema = new
        {
            type = "object",
            properties = new
            {
                candidates = new
                {
                    type = "array",
                    items = new
                    {
                        type = "object",
                        properties = new
                        {
                            name = new { type = "string" },
                            summary = new { type = "string" },
                            layoutPattern = new { type = "string" },
                            navigationPattern = new { type = "string" },
                            colorPalette = new { type = "string" },
                            typographyScale = new { type = "string" },
                            componentLibraryStyle = new { type = "string" },
                            interactionNotes = new { type = "string" },
                            accessibilityNotes = new { type = "string" }
                        },
                        required = new[] {
                            "name","summary","layoutPattern","navigationPattern",
                            "colorPalette","typographyScale","componentLibraryStyle",
                            "interactionNotes","accessibilityNotes"
                        }
                    }
                }
            },
            required = new[] { "candidates" }
        };

        var options = new ResponseCreationOptions
        {
            TextOutput = new ResponseTextOutputOptions
            {
                Format = ResponseTextFormat.CreateJsonSchemaFormat(
                    name: "uiux_population",
                    schema: BinaryData.FromObjectAsJson(schema),
                    isStrict: true)
            }
        };

        string prompt =
            $"You are a senior UI/UX designer. Generate {populationSize} distinct, " +
            $"high-quality UI/UX layout concepts for this problem:\n\n" +
            $"Problem: {config.ProblemStatement}\n" +
            $"Target platform: {config.TargetPlatform}\n" +
            $"Brand tone: {config.BrandTone}\n\n" +
            "Focus on diversity of layout patterns, navigation, and visual styles " +
            "to maximize design space exploration.";

        var response = await _responses.CreateResponseAsync(
            userInputText: prompt,
            options,
            cancellationToken: ct);

        // Response should contain a single JSON blob that matches the schema
        var json = response.OutputText; // library exposes OutputText for simple cases
        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException("OpenAI returned empty design population.");

        var parsed = System.Text.Json.JsonDocument.Parse(json);
        var root = parsed.RootElement.GetProperty("candidates");

        var list = new List<UiUxDesignCandidate>(populationSize);
        int i = 0;
        foreach (var item in root.EnumerateArray())
        {
            var spec = new UiUxDesignSpec
            {
                LayoutPattern = item.GetProperty("layoutPattern").GetString()!,
                NavigationPattern = item.GetProperty("navigationPattern").GetString()!,
                ColorPalette = item.GetProperty("colorPalette").GetString()!,
                TypographyScale = item.GetProperty("typographyScale").GetString()!,
                ComponentLibraryStyle = item.GetProperty("componentLibraryStyle").GetString()!,
                InteractionNotes = item.GetProperty("interactionNotes").GetString()!,
                AccessibilityNotes = item.GetProperty("accessibilityNotes").GetString()!
            };

            list.Add(new UiUxDesignCandidate
            {
                Id = $"gen{generationIndex}-cand{i:000}",
                Name = item.GetProperty("name").GetString() ?? $"Concept {i + 1}",
                Summary = item.GetProperty("summary").GetString() ?? string.Empty,
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
        // Compress candidate specs to reduce tokens.
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
            "You are evaluating multiple UI/UX design concepts for the same product.\n" +
            "For each candidate, score on:\n" +
            "- usability (0–10)\n" +
            "- accessibility (0–10)\n" +
            "- visual clarity (0–10)\n" +
            "- implementation complexity (0–10, lower is better)\n\n" +
            "Return JSON with an array 'scores' where each item has:\n" +
            "{ id, usability, accessibility, visual_clarity, implementation_complexity, rationale }.\n\n" +
            "Context:\n" +
            $"Problem: {config.ProblemStatement}\n" +
            $"Target platform: {config.TargetPlatform}\n" +
            $"Brand tone: {config.BrandTone}\n\n" +
            "Candidates:\n" +
            System.Text.Json.JsonSerializer.Serialize(compact);

        var options = new ResponseCreationOptions
        {
            TextOutput = new ResponseTextOutputOptions
            {
                Format = ResponseTextFormat.JsonObject
            }
        };

        var response = await _responses.CreateResponseAsync(
            userInputText: prompt,
            options,
            cancellationToken: ct);

        var json = response.OutputText!;
        var doc = System.Text.Json.JsonDocument.Parse(json);
        var scores = doc.RootElement.GetProperty("scores");

        var byId = candidates.ToDictionary(c => c.Id, c => c);
        foreach (var s in scores.EnumerateArray())
        {
            var id = s.GetProperty("id").GetString();
            if (id is null || !byId.TryGetValue(id, out var cand)) continue;

            cand.UsabilityScore = s.GetProperty("usability").GetDouble();
            cand.AccessibilityScore = s.GetProperty("accessibility").GetDouble();
            cand.VisualClarityScore = s.GetProperty("visual_clarity").GetDouble();
            cand.ImplementationComplexityScore = s.GetProperty("implementation_complexity").GetDouble();
            cand.EvaluationRationale = s.GetProperty("rationale").GetString();
        }
    }
}
