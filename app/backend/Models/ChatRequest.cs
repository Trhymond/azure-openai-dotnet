//namespace Rhymond.OpenAI.Models;
//public record class ChatRequest(
//    ChatTurn[] History,
//    Approach Approach,
//    RequestOverrides? Overrides = null) : ApproachRequest(Approach)
//{
//    public string? LastUserQuestion => History?.LastOrDefault()?.User;
//}


public record class ChatRequest
{
    [JsonProperty("history")]
    public required ChatTurn[] History { get; set; }
    [JsonProperty("approach")]
    public Approach Approach { get; set; }
    [JsonProperty("overrides")]
    public RequestOverrides? Overrides { get; set; }

    [IgnoreDataMember]
    public string? LastUserQuestion => History?.LastOrDefault()?.User;
}