namespace Rhymond.OpenAI.Services;

public sealed class ApproachServiceResponseFactory
{
    private readonly ILogger<ApproachServiceResponseFactory> _logger;
    private readonly IEnumerable<IApproachBasedService> _approachBasedServices;
    private readonly IDistributedCache _cache;

    public ApproachServiceResponseFactory(
        ILogger<ApproachServiceResponseFactory> logger,
        IEnumerable<IApproachBasedService> services, IDistributedCache cache) =>
        (_logger, _approachBasedServices, _cache) = (logger, services, cache);

    internal async Task<ApproachResponse> GetApproachResponseAsync(
        Approach approach,
        string question,
        RequestOverrides? overrides = null,
        CancellationToken cancellationToken = default)
    {

        var service =
            _approachBasedServices.SingleOrDefault(service => service.Approach == approach)
            ?? throw new ArgumentOutOfRangeException(
                nameof(approach), $"Approach: {approach} value isn't supported.");

        var key = new CacheKey(approach, question, overrides)
             .ToCacheKeyString();

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
        if (cachedValue is { Length: > 0 } && JsonSerializer.Deserialize<ApproachResponse>(cachedValue, options) is ApproachResponse cachedResponse)
        {
            _logger.LogDebug(
                "Return cached value for key ({Key}): {Appproach}\n{Response}",
                key, approach, cachedResponse);

            return cachedResponse;
        }

        var approachResponse =
            await service.ReplyAsync(question, overrides, cancellationToken)
            ?? throw new AIException(
                AIException.ErrorCodes.ServiceError,
                $"The approach response for '{approach}' was null.");


        var json = JsonSerializer.Serialize(approachResponse, options);
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        await _cache.SetStringAsync(key, json, entryOptions, cancellationToken);
        _logger.LogDebug(
            "Return cached value for key ({Key}): {Appproach}\n{Response}",
            key, approach, approachResponse);


        return approachResponse;
    }

    internal readonly record struct CacheKey(Approach Approach, string Question, RequestOverrides? Overrides)
    {
        internal string ToCacheKeyString()
        {
            var (approach, question, overrides) = this;

            string? overrideString = null;
            if (overrides is { } o)
            {
                static string Bit(bool value) => value ? "1" : "0";
                var bits = $"""
                    {Bit(o.SemanticCaptions.GetValueOrDefault())}.{Bit(o.SemanticRanker)}.{Bit(o.SuggestFollowupQuestions)}
                    """;

                overrideString =
                    $":{o.SemanticCaptions}-{o.PromptTemplate}-{o.PromptTemplatePrefix}-{o.PromptTemplateSuffix}-{bits}";
            }

            return $"""
                {approach}:{question}{overrideString}
              """;
        }
    }
}