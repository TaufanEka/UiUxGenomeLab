namespace UiUxGenomeLab.Services;

/*
 * Todo consider scraping Google. Reword searches and the questions that are asked… perform a Google search to define the question, copy the top 10 results. 
 * Todo also, consider top rated websites such as stack exchange, and places where upvoted answers occur(ed).
 * However, wire a generic search provider (backed by Bing Web Search API, Google Custom Search, or similar) 
 * and optionally mix in OpenAI web_search tool.
 */

public record SearchResult(string Title, string Url, string Snippet);

public interface ISearchProvider
{
    Task<IReadOnlyList<SearchResult>> SearchAsync(string query, int top, CancellationToken ct);
}

// Stub – implement via Bing, Google Custom Search, etc.
public sealed class NoopSearchProvider : ISearchProvider
{
    public Task<IReadOnlyList<SearchResult>> SearchAsync(string query, int top, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<SearchResult>>(Array.Empty<SearchResult>());
}
