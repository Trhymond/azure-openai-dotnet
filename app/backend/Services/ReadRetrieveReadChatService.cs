
namespace Rhymond.OpenAI.Services;

public sealed class ReadRetrieveReadChatService
{
    private readonly ILogger<ReadRetrieveReadChatService> _logger;
    private readonly IKernel _kernel;
    private readonly SearchClient _searchClient;
    private readonly string _pluginsDirectory;
    private const string FollowUpQuestionsPrompt = """
        After answering question, also generate three very brief follow-up questions that the user would likely ask next.
        Use double angle brackets to reference the questions, e.g. <<Are there exclusions for prescriptions?>>.
        Try not to repeat questions that have already been asked.
        Only generate questions and do not generate any text before or after the questions, such as 'Next Questions'
        """;

    public ReadRetrieveReadChatService(ILoggerFactory loggerFactory, SearchClient searchClient, IMemoryStore memoryStore)
    {
        _logger = loggerFactory.CreateLogger<ReadRetrieveReadChatService>();
        _searchClient = searchClient;

        _kernel = SemanticKernelFactory.GetKernel<ReadRetrieveReadChatService>(_logger, CompletionTypes.Chat, memoryStore);

        _pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "plugins");
    }

    public async Task<ChatResponse> ReplyAsync(ChatTurn[] history,
                            RequestOverrides? overrides,
                            CancellationToken cancellationToken = default)
    {
        _kernel.ImportSemanticSkillFromDirectory(_pluginsDirectory, "ChatPlugin");
        _kernel.ImportSkill(new RetrieveRelatedDocumentsPlugin(_searchClient, overrides), "RetrieveRelatedDocumentsPlugin");

        var chatPlugin = _kernel.ImportSkill(new ChatPlugin(_kernel), "ChatPlugin");

        var context = _kernel.CreateNewContext(cancellationToken);
        context["chat_history"] = history.GetChatHistoryAsText(includeLastTurn: true);
        context["question"] = (history.LastOrDefault()?.User is { } userQuestion)
        ? userQuestion
        : throw new InvalidOperationException("User question is null");

        context["follow_up_questions_prompt"] = (overrides?.SuggestFollowupQuestions is true) ? FollowUpQuestionsPrompt : string.Empty;

        var prompt = "";
        if (overrides is null or { PromptTemplate: null })
        {
            context["injected_prompt"] = string.Empty;
            prompt = FollowUpQuestionsPrompt;
        }
        else if (overrides is not null && overrides.PromptTemplate.StartsWith(">>>"))
        {
            context["injected_prompt"] = overrides.PromptTemplate[3..];
        }
        else if (overrides?.PromptTemplate is string promptTemplate)
        {
            context["injected_prompt"] = promptTemplate;
            prompt = promptTemplate;
        }

        // string query, string data, string answer)
        SKContext result = await chatPlugin["Reply"].InvokeAsync(context);

        var promptContext = _kernel.CreateNewContext(cancellationToken);
        promptContext["input"] = prompt;
        prompt = await _kernel.PromptTemplateEngine.RenderAsync(FollowUpQuestionsPrompt, promptContext);

        return new ChatResponse(
                    DataPoints: result["data"].Split('\r'),
                    Answer: result["answer"],
                    Thoughts: $"Searched for:<br>{result["query"]}<br><br>Prompt:<br>{result["prompt"].Replace("\n", "<br>")}",
                    CitationBaseUrl: AppSettings.CitationBaseUrl);
    }
}
