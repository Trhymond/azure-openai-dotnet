
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
        // step 1
        // use llm to get query
        var question = context.Variables["question"];
        var chatHistory = context.Variables["chat_history"];
        var query = await GetQuery(question, chatHistory);
       
        // step 2
        // use query to search related docs
        var additionalData = await GetAdditionalData(query);
        
        // step 3
        // use llm to get answer
        var followUpQuestionsPrompt = context.Variables["follow_up_questions_prompt"];
        var injectedPrompt = context.Variables["injected_prompt"];
        var answer = await GetAnswer(query, chatHistory, additionalData, followUpQuestionsPrompt, injectedPrompt);

        context.Variables["query"] = query;
        context.Variables["answer"] = answer;
        context.Variables["data"] = additionalData;
        
        return context;
    }

    private async Task<string> GetQuery(string question, string chatHistory) {
        var context = _kernel.CreateNewContext();
        context.Variables["query"] = question;
        context.Variables["chat_history"] = chatHistory;
       
        var getQuery = _kernel.Skills.GetFunction("ChatPlugin", "QueryGenerator");
        SKContext results = await getQuery.InvokeAsync(context);
        Console.WriteLine("results = " + results);

        return results.Variables["input"];
    }

    private async Task<string> GetAdditionalData(string question) {

        var queryDocuments = _kernel.Skills.GetFunction("RetrieveRelatedDocumentsPlugin", "QueryAsync");

        SKContext results = await queryDocuments.InvokeAsync(question);
        return results.Variables["input"];
    }

    private async Task<string> GetAnswer(string query, string? chatHistory, string? additionalData, 
        string? followUpQuestionsPrompt, string? injectedPrompt) {
        var context = _kernel.CreateNewContext();
        context.Variables["follow_up_questions_prompt"] =  followUpQuestionsPrompt ?? "";
        context.Variables["injected_prompt"] = injectedPrompt ?? "";
        context.Variables["sources"] = additionalData ?? "";
        context.Variables["chat_history"] = chatHistory ?? "";
        context.Variables["question"] = query;

        var getResponse = _kernel.Skills.GetFunction("ChatPlugin", "AnswerPromptGenerator");
        SKContext results = await getResponse.InvokeAsync(context);
        return results.Variables["input"];
    }
}