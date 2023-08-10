
namespace Rhymond.OpenAI.Plugins;

public sealed class ChatPlugin {
    private readonly IKernel _kernel;

    public ChatPlugin(IKernel kernel)
    {
        _kernel = kernel;
    }

    [SKFunction, Description("Reply to users chat"), SKName("ReplyAsync")]
    public async Task<SKContext> ReplyAsync(SKContext context)
    {
        var question = context.Variables["question"];
        var chatHistory = context.Variables["chat_history"];
        var followUpQuestionsPrompt = context.Variables["follow_up_questions_prompt"];
        var injectedPrompt = context.Variables["injected_prompt"];

        // step 1
        // use llm to get query
        var queryContext = _kernel.CreateNewContext();
        queryContext.Variables["query"] = question;
        queryContext.Variables["chat_history"] = chatHistory;
        var queryResult = await _kernel.Skills.GetFunction("ChatPlugin", "QueryGenerator").InvokeAsync(queryContext);
        var query =  queryResult.Variables["input"];
       
        // step 2
        // use query to search related docs
        var documentResult = await _kernel.Skills.GetFunction("RetrieveRelatedDocumentsPlugin", "QueryAsync").InvokeAsync(query);
        var documents =  documentResult.Variables["input"];
        
        // step 3
        // use llm to get answer
        var answerContext = _kernel.CreateNewContext();
        answerContext.Variables["follow_up_questions_prompt"] =  followUpQuestionsPrompt ?? "";
        answerContext.Variables["injected_prompt"] = injectedPrompt ?? "";
        answerContext.Variables["sources"] = documents ?? "";
        answerContext.Variables["chat_history"] = chatHistory ?? "";
        answerContext.Variables["question"] = query;
        var answerResult = await _kernel.Skills.GetFunction("ChatPlugin", "AnswerPromptGenerator").InvokeAsync(answerContext);
        
        // Reeturn context
        context.Variables["query"] = query;
        context.Variables["answer"] = answerResult.Variables["input"];
        context.Variables["data"] = documents ?? "";
        
        return context;
    }
}