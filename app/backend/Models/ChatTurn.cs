namespace Rhymond.OpenAI.Models;

//public record ChatTurn(string User, string? Bot = null);
public record class ChatTurn
{
    [JsonProperty("user")]
    public required string User { get; set; }
    [JsonProperty("bot")]
    public string? Bot { get; set; }
}
