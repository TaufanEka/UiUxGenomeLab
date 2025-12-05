using OpenAI.Responses;

namespace UiUxGenomeLab.Services;

/*
If you want to save costs, you can:
Cache refined questions.
Let one Responses call output multiple refined variants and pick one locally.
Use cheaper models (e.g. gpt-4.1-mini) for prompt refinement.
*/

public sealed class PromptRefinementService
{
    private readonly OpenAIResponseClient _responses;
    private readonly ISearchProvider _search;

    public PromptRefinementService(OpenAIResponseClient responses, ISearchProvider search)
    {
        _responses = responses;
        _search = search;
    }

    public async Task<string> RefineQuestionAsync(string rawQuestion, CancellationToken ct)
    {
        // 1) External search for better-phrased queries
        var searchResults = await _search.SearchAsync(rawQuestion, top: 10, ct);
        var searchSummary = System.Text.Json.JsonSerializer.Serialize(searchResults);

        // 2) Ask OpenAI (optionally with web_search tool) to craft the best research prompt
        var options = new ResponseCreationOptions
        {
            Tools = { ResponseTool.CreateWebSearchTool() }, // OpenAI web_search tool
            TextOutput = new ResponseTextOutputOptions
            {
                Format = ResponseTextFormat.Text
            }
        };

        var prompt =
            "You are a UI/UX research assistant. Given a rough question and some " +
            "web search results, rewrite the question into the clearest, most effective " +
            "research prompt for discovering UI/UX patterns.\n\n" +
            $"Raw question:\n{rawQuestion}\n\n" +
            "Top web results (title/url/snippet):\n" +
            searchSummary + "\n\n" +
            "Return ONLY the improved question text.";

        var response = await _responses.CreateResponseAsync(
            userInputText: prompt,
            options,
            cancellationToken: ct);

        return response.OutputText?.Trim() ?? rawQuestion;
    }
}
