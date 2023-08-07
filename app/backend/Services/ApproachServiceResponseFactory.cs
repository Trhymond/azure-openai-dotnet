namespace Rhymond.OpenAI.Services;

public sealed class ApproachServiceResponseFactory {
    private readonly ILogger<ApproachServiceResponseFactory> _logger;
    private readonly IEnumerable<IApproachBasedService> _approachBasedServices;
    
    public ApproachServiceResponseFactory(
        ILogger<ApproachServiceResponseFactory> logger, 
        IEnumerable<IApproachBasedService> services) =>
        (_logger, _approachBasedServices) = (logger, services);

    internal async Task<ApproachResponse> GetApproachResponseAsync( Approach approach,
        string question,
        RequestOverrides? overrides = null,
        CancellationToken cancellationToken = default) {

        var service =
            _approachBasedServices.SingleOrDefault(service => service.Approach == approach)
            ?? throw new ArgumentOutOfRangeException(
                nameof(approach), $"Approach: {approach} value isn't supported.");

        var approachResponse =
            await service.ReplyAsync(question, overrides, cancellationToken)
            ?? throw new AIException(
                AIException.ErrorCodes.ServiceError,
                $"The approach response for '{approach}' was null.");

         return approachResponse;
    }

}