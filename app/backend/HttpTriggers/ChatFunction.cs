
namespace Rhymond.OpenAI.HttpTriggers
{
    public class ChatFunction
    {
        private readonly ILogger _logger;
        private readonly ReadRetrieveReadChatService _chatService;

        public ChatFunction(ILoggerFactory loggerFactory, ReadRetrieveReadChatService chatService)
        {
            _logger = loggerFactory.CreateLogger<ChatFunction>();
            _chatService = chatService;
        }

        [Function("Chat")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "chat")] HttpRequestData req)
        {
            _logger.LogInformation("OpenAI Chat function");

            var cancellationToken = new CancellationToken();
            var chatRequest = await req.ReadFromJsonAsync<ChatRequest>();
            if (chatRequest is { History.Length: > 0 }) {
                var chatResponse = await _chatService.ReplyAsync(
                    chatRequest.History, chatRequest.Overrides, cancellationToken);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync<ChatResponse>(chatResponse).ConfigureAwait(false);
                return await Task.FromResult(response).ConfigureAwait(false);
            }

            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
