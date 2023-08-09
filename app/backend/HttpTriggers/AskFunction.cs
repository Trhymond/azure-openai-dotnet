
using Rhymond.OpenAI.Extensions;

namespace Rhymond.OpenAI.HttpTriggers
{
    public class AskFunction
    {
        private readonly ILogger _logger;
        private readonly ApproachServiceResponseFactory _factory;

        public AskFunction(ILoggerFactory loggerFactory, ApproachServiceResponseFactory factory)
        {
            _logger = loggerFactory.CreateLogger<AskFunction>();
            _factory = factory;
        }

        [Function("Ask")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ask")] HttpRequestData req)
        {
            _logger.LogInformation("OpenAI Ask function");

            var askRequest = req.ReadFromJson<AskRequest>();

            if (askRequest is { Question.Length: > 0 }) {

                var approachResponse = await _factory.GetApproachResponseAsync(
                    askRequest.Approach, askRequest.Question, askRequest.Overrides);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync<ApproachResponse>(approachResponse).ConfigureAwait(false);
                return await Task.FromResult(response).ConfigureAwait(false);
            }

            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
