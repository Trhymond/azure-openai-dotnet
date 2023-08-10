
namespace Rhymond.OpenAI.Extensions;

internal static class CosmosClentExtensions
{
    internal static async Task<string> QueryDataAsync(
        this CosmosClient cosmosClient,
        string cosmosDatabase,
        string cosmosContainer,
        string query) 
    {
            
            var database = cosmosClient.GetDatabase(cosmosDatabase);
            var container = database.GetContainer(cosmosContainer);

            QueryDefinition queryDefinition = new QueryDefinition(query);
                // .WithParameter("@status", "start%");
            //var query = f"select top 10 * from employee_plans p " # where p.display_name = '{key_field}'"  
            using FeedIterator<Employee> feed = container.GetItemQueryIterator<Employee>(
                queryDefinition: queryDefinition
            );

            // var result = new List<Employee>();
            var sb = new StringBuilder();
            while (feed.HasMoreResults)
            {
                FeedResponse<Employee> response = await feed.ReadNextAsync();
                foreach(Employee item in response){
                    sb.AppendLine(string.Format("title: {0}, plan: {1}, plan type: {2}, dependend count: {3}, high deductible: {4}  ", item.Title, item.Plan, item.PlanType, item.DependendCount, item.HasHsa));
                }
            }

            return sb.ToString();
    }
}