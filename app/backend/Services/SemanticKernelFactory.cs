namespace Rhymond.OpenAI.Services;

public class SemanticKernelFactory
{
    private static readonly DefaultAzureCredential _azureCredential = new();

    public static IKernel GetKernel<T>(ILogger<T> logger, IMemoryStore memoryStore)
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
            .WithAzureChatCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential, true)
            .WithAzureTextEmbeddingGenerationService(AppSettings.AzureOpenAiEmbeddingDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential)
            .WithMemoryStorage(memoryStore);

        var kernel = builder.Build();
        return kernel;
    }

    public static IKernel GetKernel<T>(ILogger<T> logger)
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
            .WithAzureChatCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential, true);

        var kernel = builder.Build();
        return kernel;
    }

    //public static IKernel GetKernel<T>(ILogger<T> logger, CompletionTypes completionType)
    //{

    //    var config = new KernelConfig()
    //        .SetDefaultHttpRetryConfig(new Microsoft.SemanticKernel.Reliability.HttpRetryConfig
    //        {
    //            MaxRetryCount = 3,
    //            UseExponentialBackoff = true
    //        });



    //    var builder = Kernel.Builder
    //        .WithLogger(logger)
    //        .WithConfiguration(config);

    //    builder.WithAzureChatCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential, true, "gpt-35-turbo");

    //    //if (completionType == CompletionTypes.Chat)
    //    //{
    //    //    builder.WithAzureChatCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential);
    //    //}
    //    //else if (completionType == CompletionTypes.Text)
    //    //{
    //    //    builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, _azureCredential);
    //    //}

    //    var kernel = builder.Build();
    //    return kernel;
    //}

    //public static IKernel GetKernel<T>(ILogger<T> logger)
    //{
    //    return GetKernel(logger, CompletionTypes.Text);
    //}

}

