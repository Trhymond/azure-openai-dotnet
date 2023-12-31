namespace Rhymond.OpenAI.Services;

internal sealed class RetrieveThenReadApproachService : IApproachBasedService
{
    private readonly ILogger<RetrieveThenReadApproachService> _logger;
    private readonly IKernel _kernel;
    private readonly string _pluginsDirectory;
    private readonly SearchClient _searchClient;

    public RetrieveThenReadApproachService(ILoggerFactory loggerFactory,  SearchClient searchClient)
    {
        _logger = loggerFactory.CreateLogger<RetrieveThenReadApproachService>();
        _searchClient = searchClient;

        _kernel  = SemanticKernelFactory.GetKernel<RetrieveThenReadApproachService>(_logger); 
        // var builder = new KernelBuilder();
        // builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey);
        // _kernel = builder.Build();

        _pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
    }

    public Approach Approach => Approach.RetrieveThenRead;

    public async Task<ApproachResponse> ReplyAsync(string question, RequestOverrides? overrides = null, CancellationToken cancellationToken = default)
    {
        _kernel.ImportSemanticSkillFromDirectory(_pluginsDirectory, "RetrieveThenReadPlugin");
        var retrieveRelatedDocumentsPlugin = _kernel.ImportSkill(new RetrieveRelatedDocumentsPlugin(_searchClient, overrides), "RetrieveRelatedDocumentsPlugin");

        var queryContext = _kernel.CreateNewContext();
        queryContext.Variables["question"] = question;
        var text = await retrieveRelatedDocumentsPlugin["QueryAsync"].InvokeAsync(queryContext, cancellationToken: CancellationToken.None);

        var context = _kernel.CreateNewContext();
        context.Variables["retrieve"] = text.Result;
        context.Variables["question"] = question;

        var getAnswer = _kernel.Skills.GetFunction("RetrieveThenReadPlugin", "AnswerGenerator");
        var answer = await getAnswer.InvokeAsync(context);
        // string asnwer = context["input"].Trim();

        return new ApproachResponse {
            DataPoints = text.Result.Split('\r'),
            Answer = answer.Result.ToString(),
            Thoughts = $"Question: {question} \r Prompt: {context.Variables}",
            CitationBaseUrl = AppSettings.CitationBaseUrl
        };

    }
}