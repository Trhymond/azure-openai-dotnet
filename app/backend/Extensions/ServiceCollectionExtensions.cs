

namespace Rhymond.OpenAI.Extensions;

internal static class ServiceCollectionExtensions
{

    private static readonly DefaultAzureCredential _azureCredential = new();

    internal static IServiceCollection AddAzureServices(this IServiceCollection services)
    {

        services.AddSingleton<BlobServiceClient>(sp =>
        {
            var blobServiceClient = new BlobServiceClient(
                new Uri(AppSettings.AzureStorageAccountEndpoint), _azureCredential);

            return blobServiceClient;
        });

        services.AddSingleton<BlobContainerClient>(sp =>
        {
            return sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(AppSettings.AzureStorageContainer);
        });

        services.AddSingleton<SearchClient>(sp =>
        {
            var searchClient = new SearchClient(
                new Uri(AppSettings.AzureSearchServiceEndpoint), AppSettings.AzureSearchIndex, _azureCredential);

            return searchClient;
        });

        services.AddSingleton<DocumentAnalysisClient>(sp =>
        {
            var documentAnalysisClient = new DocumentAnalysisClient(
                new Uri(AppSettings.AzureOpenAiServiceEndpoint), _azureCredential);
            return documentAnalysisClient;
        });

        services.AddSingleton<OpenAIClient>(sp =>
        {
            var openAIClient = new OpenAIClient(
                new Uri(AppSettings.AzureOpenAiServiceEndpoint), _azureCredential);

            return openAIClient;
        });

        services.AddSingleton<IApproachBasedService, RetrieveThenReadApproachService>();
        services.AddSingleton<IApproachBasedService, ReadRetrieveReadApproachService>();
        services.AddSingleton<IApproachBasedService, ReadDecomposeAskApproachService>();
        services.AddSingleton<ApproachServiceResponseFactory>();

        services.AddSingleton<SemanticKernelFactory>();
        services.AddSingleton<ReadRetrieveReadChatService>();

        return services;

    }

    internal static IServiceCollection AddMemoryStore(this IServiceCollection services)
    {
        return services.AddSingleton<IMemoryStore, CorpusMemoryStore>();
    }
}