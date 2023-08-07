namespace Rhymond.OpenAI.Plugins;

public sealed class LookupPlugin {

    private readonly SearchClient _searchClient;
    private readonly RequestOverrides? _requestOverrides;

    public LookupPlugin(SearchClient searchClient)
    {
        _searchClient = searchClient;
    }

    [SKFunction, Description("Query Azure Cognitive Search")]
    public async Task<string> QueryAsync(string searchQuery)
    {
        return await Task.FromResult(searchQuery);
    }
}
