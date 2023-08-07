// namespace Rhymond.OpenAI.Extensions;

public class SemanticKernelFactory {

    public static IKernel GetKernel<T>(ILogger<T> logger, CompletionTypes completionType, IMemoryStore memoryStore){

            var config = new KernelConfig()
                .SetDefaultHttpRetryConfig(new Microsoft.SemanticKernel.Reliability.HttpRetryConfig{
                    MaxRetryCount = 3,
                    UseExponentialBackoff = true
                });

            var builder = Kernel.Builder
                .WithLogger(logger)
                .WithConfiguration(config)
                .WithAzureTextEmbeddingGenerationService(AppSettings.AzureOpenAiEmbeddingDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey)
                .WithMemoryStorage(memoryStore);
            
            if(completionType == CompletionTypes.Chat) {
                builder.WithAzureChatCompletionService(AppSettings.AzureOpenAiChatGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey);
            }
            else if(completionType == CompletionTypes.Text) {
                builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey);
            }

            var kernel = builder.Build();
            return kernel;
    }

    public static IKernel GetKernel<T>(ILogger<T> logger, CompletionTypes completionType){

        var config = new KernelConfig()
            .SetDefaultHttpRetryConfig(new Microsoft.SemanticKernel.Reliability.HttpRetryConfig{
                MaxRetryCount = 3,
                UseExponentialBackoff = true
            });

        var builder = Kernel.Builder
            .WithLogger(logger)
            .WithConfiguration(config);
  
        if(completionType == CompletionTypes.Chat) {
            builder.WithAzureChatCompletionService(AppSettings.AzureOpenAiChatGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey);
        }
        else if(completionType == CompletionTypes.Text) {
            builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey);
        }

        var kernel = builder.Build();
        return kernel;
    }

    public static IKernel GetKernel<T>(ILogger<T> logger){
        return GetKernel(logger, CompletionTypes.Text);
    }

}

 