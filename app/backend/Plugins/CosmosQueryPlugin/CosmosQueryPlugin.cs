namespace Rhymond.OpenAI.Plugins;

public sealed class CosmosQueryPlugin
{

    private readonly CosmosClient _cosmosClient;
    private readonly string _database;
     private readonly string _container;

    public CosmosQueryPlugin(CosmosClient cosmosClient, string database, string container)
    {
        _cosmosClient = cosmosClient;
        _database = database;
        _container = container;
    }

    [SKFunction, Description("Query cosmos db collection"), SKName("QueryAsync")]
    public async Task<string> QueryAsync(string lookupQuery, SKContext context)
    {
        if (lookupQuery is string query)
        {
            return await _cosmosClient.QueryDataAsync(query, _database, _container);
        }

        throw new AIException(
            AIException.ErrorCodes.ServiceError,
            "Query skill failed to get query from context");
    }
}
