namespace Rhymond.OpenAI.Services;

internal sealed class ReadRetrieveReadApproachService : IApproachBasedService
{
    private readonly ILogger<ReadRetrieveReadApproachService> _logger;
    private readonly IKernel _kernel;
    private readonly string _pluginsDirectory;
    private readonly SearchClient _searchClient;
    private const string PlanPrompt = """
        Do the following steps:
         - Search information for $question and save result to $knowledge
         - Answer the $question based on the knowledge you have and save result to $answer.
        """;

    public ReadRetrieveReadApproachService(ILoggerFactory loggerFactory, SearchClient searchClient)
    {
        _logger = loggerFactory.CreateLogger<ReadRetrieveReadApproachService>();
        _searchClient = searchClient;
        _pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "plugins");

        _kernel  = SemanticKernelFactory.GetKernel<ReadRetrieveReadApproachService>(_logger, CompletionTypes.Text); 
        // var builder = new KernelBuilder();
        // builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey);
        // _kernel = builder.Build();
    }

    public Approach Approach => Approach.ReadRetrieveRead;

    public async Task<ApproachResponse> ReplyAsync(string question, RequestOverrides? overrides = null, CancellationToken cancellationToken = default)
    {
        _kernel.ImportSemanticSkillFromDirectory(_pluginsDirectory, "ReadRetrieveReadPlugin");
        _kernel.ImportSkill(new RetrieveRelatedDocumentsPlugin(_searchClient, overrides), "RetrieveRelatedDocumentsPlugin");

        var planner = new SequentialPlanner(_kernel, new SequentialPlannerConfig
        {
            RelevancyThreshold = 0.7,
        });

        var plan = await planner.CreatePlanAsync(PlanPrompt);
        plan.State["question"] = question;
        plan.State["knowledge"] = string.Empty;

        var sb = new StringBuilder();
        var step  = 1;

         _logger.LogInformation("{Plan}", PlanToString(plan));

         do {
            plan = await _kernel.StepAsync(plan, cancellationToken: cancellationToken);  
            sb.AppendLine($"Step {step++} - Execution results:\n");
            sb.AppendLine(plan.State + "\n");

         } while (plan.HasNextStep);

        return new ApproachResponse(
            DataPoints: plan.State["knowledge"].ToString().Split('\r'),
            Answer: plan.State["answer"],
            Thoughts: sb.ToString().Replace("\n", "<br>"),
            CitationBaseUrl: AppSettings.CitationBaseUrl);
    }

    private static string PlanToString(Plan originalPlan)
    {
        return $"Goal: {originalPlan.Description}\n\nSteps:\n" + string.Join("\n", originalPlan.Steps.Select(
            s =>
                $"- {s.SkillName}.{s.Name} {string.Join(" ", s.Parameters.Select(p => $"{p.Key}='{p.Value}'"))}{" => " + string.Join(" ", s.Outputs)}"
        ));
    }
}