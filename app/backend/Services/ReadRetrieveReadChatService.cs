
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

        _kernel = SemanticKernelFactory.GetKernel<ReadRetrieveReadChatService>(_logger, memoryStore);

        _pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
    }

    public async Task<ChatResponse> ReplyAsync(ChatTurn[] history, RequestOverrides? overrides)
    {
        _kernel.ImportSemanticSkillFromDirectory(_pluginsDirectory, "ChatPlugin");
        _kernel.ImportSkill(new RetrieveRelatedDocumentsPlugin(_searchClient, overrides), "RetrieveRelatedDocumentsPlugin");

        var chatPlugin = _kernel.ImportSkill(new ChatPlugin(_kernel), "ChatPlugin");

        var context = _kernel.CreateNewContext();
        context.Variables["chat_history"] = history.GetChatHistoryAsText(includeLastTurn: true);
        context.Variables["question"] = (history.LastOrDefault()?.User is { } userQuestion)
        ? userQuestion
        : throw new InvalidOperationException("User question is null");

        context.Variables["follow_up_questions_prompt"] = (overrides?.SuggestFollowupQuestions is true) ? FollowUpQuestionsPrompt : string.Empty;

        var prompt = "";
        if (overrides is null or { PromptTemplate: null })
        {
            context.Variables["injected_prompt"] = string.Empty;
            prompt = FollowUpQuestionsPrompt;
        }
        else if (overrides is not null && overrides.PromptTemplate.StartsWith(">>>"))
        {
            context.Variables["injected_prompt"] = overrides.PromptTemplate[3..];
        }
        else if (overrides?.PromptTemplate is string promptTemplate)
        {
            context.Variables["injected_prompt"] = promptTemplate;
            prompt = promptTemplate;
        }

        // string query, string data, string answer)
        SKContext result = await chatPlugin["ReplyAsync"].InvokeAsync(context);

        var promptContext = _kernel.CreateNewContext();
        promptContext.Variables["input"] = prompt;
        prompt = await _kernel.PromptTemplateEngine.RenderAsync(FollowUpQuestionsPrompt, promptContext);

        return new ChatResponse(
                    DataPoints: result.Variables["data"].Split('\r'),
                    Answer: result.Variables["answer"],
                    Thoughts: $"Searched for:<br>{result.Variables["query"]}<br><br>Prompt:<br>{prompt.Replace("\n", "<br>")}",
                    CitationBaseUrl: AppSettings.CitationBaseUrl);
    }
}
