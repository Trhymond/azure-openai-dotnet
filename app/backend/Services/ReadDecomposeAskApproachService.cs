namespace Rhymond.OpenAI.Services;

internal sealed class ReadDecomposeAskApproachService : IApproachBasedService
{
    private readonly ILogger<ReadDecomposeAskApproachService> _logger;
    private readonly IKernel _kernel;
    private readonly string _pluginsDirectory;
    private readonly SearchClient _searchClient;
    private const string PlannerPrefix = """
        do the following steps:
         - explain what you need to know to answer the $question.
         - generating $keywords from explanation.
         - use $keywords to lookup or search information.
         - update information to $knowledge.
         - summarize the entire process and update $summary.
         - answer the question based on the knowledge you have.
        """;

    public ReadDecomposeAskApproachService(ILoggerFactory loggerFactory, SearchClient searchClient)
    {
        _logger = loggerFactory.CreateLogger<ReadDecomposeAskApproachService>();
        _searchClient = searchClient;
        _pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");

        _kernel = SemanticKernelFactory.GetKernel<ReadDecomposeAskApproachService>(_logger);
        // var builder = new KernelBuilder();
        // builder.WithAzureTextCompletionService(AppSettings.AzureOpenAiGptDeployment, AppSettings.AzureOpenAiServiceEndpoint, AppSettings.AzureOpenAiKey);
        // _kernel = builder.Build();

    }

    public Approach Approach => Approach.ReadDecomposeAsk;

    public async Task<ApproachResponse> ReplyAsync(string question, RequestOverrides? overrides = null, CancellationToken cancellationToken = default)
    {
        _kernel.ImportSemanticSkillFromDirectory(_pluginsDirectory, "ReadDecomposePlugin");
        _kernel.ImportSkill(new RetrieveRelatedDocumentsPlugin(_searchClient, overrides), "RetrieveRelatedDocumentsPlugin");

        var planner = new SequentialPlanner(_kernel, new SequentialPlannerConfig
        {
            RelevancyThreshold = 0.7,
        });

        var planInstruction = $"{PlannerPrefix}";
        var plan = await planner.CreatePlanAsync(planInstruction);
        plan.State["question"] = question;

        var sb = new StringBuilder();
        var step = 1;

        _logger.LogInformation("{Plan}", PlanToString(plan));

        do
        {
            plan = await _kernel.StepAsync(question, plan, cancellationToken: cancellationToken);
            sb.AppendLine($"Step {step++} - Execution results:\n");
            sb.AppendLine(plan.State + "\n");

        } while (plan.HasNextStep);

        return new ApproachResponse {
            DataPoints = plan.State["knowledge"].ToString().Split('\r'),
            Answer = plan.State["Answer"],
            Thoughts = plan.State["SUMMARY"].Replace("\n", "<br>"),
            CitationBaseUrl = AppSettings.CitationBaseUrl
        };

    }

    private static string PlanToString(Plan originalPlan)
    {
        return $"Goal: {originalPlan.Description}\n\nSteps:\n" + string.Join("\n", originalPlan.Steps.Select(
            s =>
                $"- {s.SkillName}.{s.Name} {string.Join(" ", s.Parameters.Select(p => $"{p.Key}='{p.Value}'"))}{" => " + string.Join(" ", s.Outputs)}"
        ));
    }
}