

namespace Rhymond.OpenAI.Extensions;

internal static class ServiceCollectionExtensions {

    private static readonly DefaultAzureCredential _azureCredential = new();

    internal static IServiceCollection AddAzureServices(this IServiceCollection services) {

        services.AddSingleton<BlobServiceClient>(sp =>
        {
            // var config = sp.GetRequiredService<IConfiguration>();
            var azureStorageAccountEndpoint = AppSettings.AzureStorageAccountEndpoint;
            ArgumentNullException.ThrowIfNullOrEmpty(azureStorageAccountEndpoint);

            var blobServiceClient = new BlobServiceClient(
                new Uri(azureStorageAccountEndpoint), _azureCredential);

            return blobServiceClient;
        }); 

        services.AddSingleton<BlobContainerClient>(sp =>
        {
            var azureStorageContainer = AppSettings.AzureStorageContainer;
            return sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(azureStorageContainer);
        });

        services.AddSingleton<SearchClient>(sp =>
        {
            var (azureSearchServiceEndpoint, azureSearchIndex) =
                (AppSettings.AzureSearchServiceEndpoint, AppSettings.AzureSearchIndex);

            ArgumentNullException.ThrowIfNullOrEmpty(azureSearchServiceEndpoint);

            var searchClient = new SearchClient(
                new Uri(azureSearchServiceEndpoint), azureSearchIndex, _azureCredential);

            return searchClient;
        });

        services.AddSingleton<DocumentAnalysisClient>(sp =>
        {
            var azureOpenAiServiceEndpoint = AppSettings.AzureOpenAiServiceEndpoint;
            ArgumentNullException.ThrowIfNullOrEmpty(azureOpenAiServiceEndpoint);

            var documentAnalysisClient = new DocumentAnalysisClient(
                new Uri(azureOpenAiServiceEndpoint), _azureCredential);
            return documentAnalysisClient;
        });

        services.AddSingleton<OpenAIClient>(sp =>
        {
            var azureOpenAiServiceEndpoint = AppSettings.AzureOpenAiServiceEndpoint;

            ArgumentNullException.ThrowIfNullOrEmpty(azureOpenAiServiceEndpoint);

            var openAIClient = new OpenAIClient(
                new Uri(azureOpenAiServiceEndpoint), _azureCredential);

            return openAIClient;
        });

        // services.AddSingleton<IKernel>(sp => {
        //     var logger = sp.GetRequiredService<ILogger>();
        //     var config = new KernelConfig()
        //         .SetDefaultHttpRetryConfig(new Microsoft.SemanticKernel.Reliability.HttpRetryConfig{
        //             MaxRetryCount = 3,
        //             UseExponentialBackoff = true
        //         });
        //     var memoryStore = sp.GetRequiredService<IMemoryStore>();

        //     var builder = Kernel.Builder
        //         .WithAzureChatCompletionService(AppSettings.AzureOpenAiChatGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey)
        //         .WithAzureTextEmbeddingGenerationService(AppSettings.AzureOpenAiEmbeddingDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey)
        //         .WithLogger(logger)
        //         .WithConfiguration(config);

        //     builder.WithMemoryStorage(memoryStore);

        //     var kernel = builder.Build();

        //     return kernel;
        // });

        // services.AddSingleton<IKernel>(sp =>
        // {
        //     // Semantic Kernel doesn't support Azure AAD credential for now
        //     // so we implement our own text completion backend
        //     var azureOpenAiGptDeployment = AppSettings.AzureOpenAiGptDeployment;

        //     var openAITextService = sp.GetRequiredService<AzureOpenAITextCompletionService>();

        //     var kernel = new KernelBuilder()
        //         .WithAIService<AzureOpenAITextCompletionService>(azureOpenAiGptDeployment!, _ => openAITextService)
        //         .Build();

        //     return kernel;
        // });

        // services.AddSingleton<IKernel>(sp => {
        //     var builder = new KernelBuilder();
        //     builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiChatGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey);
        //     IKernel kernel = kernel.Build();

        //     return kernal;
        // });


        services.AddSingleton<SemanticKernelFactory>();

        // services.AddSingleton<AzureOpenAITextCompletionService>();
        // services.AddSingleton<AzureOpenAIChatCompletionService>();
        services.AddSingleton<ReadRetrieveReadChatService>();

        services.AddSingleton<IApproachBasedService, RetrieveThenReadApproachService>();
        services.AddSingleton<IApproachBasedService, ReadRetrieveReadApproachService>();
        services.AddSingleton<IApproachBasedService, ReadDecomposeAskApproachService>();
        services.AddSingleton<ApproachServiceResponseFactory>();

        return services;

    }

    internal static IServiceCollection AddMemoryStore(this IServiceCollection services)
    {
        return services.AddSingleton<IMemoryStore, CorpusMemoryStore>();
    }
}