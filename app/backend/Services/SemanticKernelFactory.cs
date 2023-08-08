namespace Rhymond.OpenAI.Services;

public class SemanticKernelFactory
{
    private static readonly DefaultAzureCredential _azureCredential = new();

    public static IKernel GetKernel<T>(ILogger<T> logger, CompletionTypes completionType, IMemoryStore memoryStore)
    {

        var config = new KernelConfig()
            .SetDefaultHttpRetryConfig(new Microsoft.SemanticKernel.Reliability.HttpRetryConfig
            {
                MaxRetryCount = 3,
                UseExponentialBackoff = true
            });

        var builder = Kernel.Builder
            .WithLogger(logger)
            .WithConfiguration(config)
            .WithAzureTextEmbeddingGenerationService(AppSettings.AzureOpenAiEmbeddingDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential)
            .WithMemoryStorage(memoryStore);

        if (completionType == CompletionTypes.Chat)
        {
            builder.WithAzureChatCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential);
        }
        else if (completionType == CompletionTypes.Text)
        {
            builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential);
        }

        var kernel = builder.Build();
        return kernel;
    }

    public static IKernel GetKernel<T>(ILogger<T> logger, CompletionTypes completionType)
    {

        var config = new KernelConfig()
            .SetDefaultHttpRetryConfig(new Microsoft.SemanticKernel.Reliability.HttpRetryConfig
            {
                MaxRetryCount = 3,
                UseExponentialBackoff = true
            });



        var builder = Kernel.Builder
            .WithLogger(logger)
            .WithConfiguration(config);

        if (completionType == CompletionTypes.Chat)
        {
            builder.WithAzureChatCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential);
        }
        else if (completionType == CompletionTypes.Text)
        {
            builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential);
        }

        var kernel = builder.Build();
        return kernel;
    }

    public static IKernel GetKernel<T>(ILogger<T> logger)
    {
        return GetKernel(logger, CompletionTypes.Text);
    }

}

