
namespace Rhymond.OpenAI;

public static class AppSettings
{
    public static string AzureStorageAccountEndpoint =>
        Environment.GetEnvironmentVariable("AzureStorageAccountEndpoint") ?? throw new ArgumentNullException(nameof(AzureStorageAccountEndpoint));

    public static string AzureStorageContainer =>
      Environment.GetEnvironmentVariable("AzureStorageContainer") ?? throw new ArgumentNullException(nameof(AzureStorageContainer));

    public static string AzureSearchServiceEndpoint =>
       Environment.GetEnvironmentVariable("AzureSearchServiceEndpoint") ?? throw new ArgumentNullException(nameof(AzureSearchServiceEndpoint));

    public static string AzureSearchIndex =>
       Environment.GetEnvironmentVariable("AzureSearchIndex") ?? throw new ArgumentNullException(nameof(AzureSearchIndex));

    public static string AzureOpenAiServiceEndpoint =>
        Environment.GetEnvironmentVariable("AzureOpenAiServiceEndpoint") ?? throw new ArgumentNullException(nameof(AzureOpenAiServiceEndpoint));

    public static string AzureOpenAiGptDeployment =>
        Environment.GetEnvironmentVariable("AzureOpenAiGptDeployment") ?? throw new ArgumentNullException(nameof(AzureOpenAiGptDeployment));

    public static string AzureOpenAiEmbeddingDeployment =>
        Environment.GetEnvironmentVariable("AzureOpenAiEmbeddingDeployment") ?? throw new ArgumentNullException(nameof(AzureOpenAiEmbeddingDeployment));

    public static string AzureCosmosEndpoint =>
        Environment.GetEnvironmentVariable("AzureCosmosEndpoint") ?? throw new ArgumentNullException(nameof(AzureCosmosEndpoint));

    public static string AzureFormRecognizerServiceEndpoint =>
        Environment.GetEnvironmentVariable("AzureFormRecognizerServiceEndpoint") ?? throw new ArgumentNullException(nameof(AzureFormRecognizerServiceEndpoint));

    public static string CitationBaseUrl =>
        new UriBuilder(AzureStorageAccountEndpoint) { Path = AzureStorageContainer }.Uri.AbsoluteUri;
}

