
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
        var charPlugin = _kernel.ImportSemanticSkillFromDirectory(_pluginsDirectory, "ChatPlugin");
        var retrieveRelatedDocumentsPlugin = _kernel.ImportSkill(new RetrieveRelatedDocumentsPlugin(_searchClient, overrides), "RetrieveRelatedDocumentsPlugin");

        var chatHistory = history.GetChatHistoryAsText(includeLastTurn: true);
        var question = (history.LastOrDefault()?.User is { } userQuestion)
            ? userQuestion
            : throw new InvalidOperationException("User question is null");
        var prompt = "";
        var injectedPrompt = "";
        if (overrides is null or { PromptTemplate: null })
        {
            injectedPrompt = string.Empty;
            prompt = FollowUpQuestionsPrompt;
        }
        else if (overrides is not null && overrides.PromptTemplate.StartsWith(">>>"))
        {
            injectedPrompt= overrides.PromptTemplate[3..];
        }
        else if (overrides?.PromptTemplate is string promptTemplate)
        {
            injectedPrompt = promptTemplate;
            prompt = promptTemplate;
        }

        // step 1
        // use llm to get query
        var queryContext = _kernel.CreateNewContext();
        queryContext.Variables["chat_history"] = chatHistory;
        queryContext.Variables["question"] = question;
        var queryResult = await charPlugin["QueryGenerator"].InvokeAsync(queryContext);
        var query = queryResult.Variables["input"];

        // step 2
        // use query to search related docs
        var documentsResult = await retrieveRelatedDocumentsPlugin["QueryAsync"].InvokeAsync(query);
        var documents = documentsResult.Variables["input"];

        // step 3
        // use llm to get answer
        var answerContext = _kernel.CreateNewContext();
        answerContext.Variables["follow_up_questions_prompt"] =  (overrides?.SuggestFollowupQuestions is true) ? FollowUpQuestionsPrompt : string.Empty;
        answerContext.Variables["injected_prompt"] = injectedPrompt ?? "";
        answerContext.Variables["sources"] = documents ?? "";
        answerContext.Variables["chat_history"] = chatHistory ?? "";
        answerContext.Variables["question"] = query;
        var answerResult = await charPlugin["AnswerPromptGenerator"].InvokeAsync(answerContext);
        var answer = answerResult.Variables["input"];

        var promptContext = _kernel.CreateNewContext();
        promptContext.Variables["input"] = prompt;
        prompt = await _kernel.PromptTemplateEngine.RenderAsync(FollowUpQuestionsPrompt, promptContext);

        return new ChatResponse(
                    DataPoints: (documents ?? "").Split('\r'),
                    Answer: answer,
                    Thoughts: $"Searched for:<br>{query}<br><br>Prompt:<br>{prompt.Replace("\n", "<br>")}",
                    CitationBaseUrl: AppSettings.CitationBaseUrl);
    }
}
