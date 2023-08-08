namespace Rhymond.OpenAI.Plugins;

public sealed class LookupPlugin
{

    private readonly SearchClient _searchClient;
    private readonly RequestOverrides? _requestOverrides;

    public LookupPlugin(SearchClient searchClient, RequestOverrides? requestOverrides)
    {
        _searchClient = searchClient;
        _requestOverrides = requestOverrides;
    }

    [SKFunction, Description("Query Azure Cognitive Search")]
    public async Task<string> LookupAsync(string lookupQuery, SKContext context)
    {
        if (lookupQuery is string query)
        {
            return await _searchClient.LookupAsync(query, _requestOverrides);
        }

        throw new AIException(
            AIException.ErrorCodes.ServiceError,
            "Query skill failed to get query from context");
    }
}
