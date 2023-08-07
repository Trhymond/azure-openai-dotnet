
//namespace Rhymond.OpenAI;
public static class AppSettings {

    public static string AzureOpenAiKey =>
        Environment.GetEnvironmentVariable("AzureOpenAiKey") ?? throw new ArgumentNullException(nameof(AzureOpenAiKey));
    // AZURE_OPENAI_CHAT_GPT_DEPLOYMENT
    public static string AzureOpenAiChatGptDeployment =>
        Environment.GetEnvironmentVariable("AzureOpenAiChatGptDeployment") ?? throw new ArgumentNullException(nameof(AzureOpenAiChatGptDeployment));

     // AZURE_OPENAI_GPT_DEPLOYMENT
    public static string AzureOpenAiGptDeployment =>
        Environment.GetEnvironmentVariable("AzureOpenAiGptDeployment") ?? throw new ArgumentNullException(nameof(AzureOpenAiGptDeployment));

     // AZURE_OPENAI_EMBEDDING_DEPLOYMENT
     // "text-embedding-ada-002"
    public static string AzureOpenAiEmbeddingDeployment =>
        Environment.GetEnvironmentVariable("AzureOpenAiEmbeddingDeployment") ?? throw new ArgumentNullException(nameof(AzureOpenAiEmbeddingDeployment));


    public static string AzureStorageAccountEndpoint =>
        Environment.GetEnvironmentVariable("AzureStorageAccountEndpoint") ?? throw new ArgumentNullException(nameof(AzureStorageAccountEndpoint));
    
    public static string AzureStorageContainer =>
        Environment.GetEnvironmentVariable("AzureStorageContainer") ?? throw new ArgumentNullException(nameof(AzureStorageContainer));
    
    // AZURE_KEY_VAULT_ENDPOINT
    public static string AzureKeyVaultEndpoint =>
        Environment.GetEnvironmentVariable("AzureKeyVaultEndpoint") ?? throw new ArgumentNullException(nameof(AzureKeyVaultEndpoint));
    
    public static string AzureSearchServiceEndpoint =>
        Environment.GetEnvironmentVariable("AzureSearchServiceEndpoint") ?? throw new ArgumentNullException(nameof(AzureSearchServiceEndpoint));
    
    public static string AzureSearchIndex =>
        Environment.GetEnvironmentVariable("AzureSearchIndex") ?? throw new ArgumentNullException(nameof(AzureSearchIndex));

    public static string AzureOpenAiServiceEndpoint =>
        Environment.GetEnvironmentVariable("AzureOpenAiServiceEndpoint") ?? throw new ArgumentNullException(nameof(AzureOpenAiServiceEndpoint));

    public static string CitationBaseUrl => 
        new UriBuilder(AzureStorageAccountEndpoint){ Path = AzureStorageContainer }.Uri.AbsoluteUri;
}

