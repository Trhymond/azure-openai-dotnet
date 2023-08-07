// namespace Rhymond.OpenAI.Plugins;

public sealed class RetrieveRelatedDocumentsPlugin {

    private readonly SearchClient _searchClient;
    private readonly RequestOverrides? _requestOverrides;

    public RetrieveRelatedDocumentsPlugin(SearchClient searchClient, RequestOverrides? requestOverrides)
    {
        _searchClient = searchClient;
        _requestOverrides = requestOverrides;
    }

    [SKFunction, Description("Query Azure Cognitive Search")]
    public async Task<string> QueryAsync(string searchQuery)
    {
        if (searchQuery is string query)
        {
            var result = await _searchClient.QueryDocumentsAsync(query, _requestOverrides);

            return result;
        }

        throw new AIException(
            AIException.ErrorCodes.ServiceError,
            "Query skill failed to get query from context");
    }
}
