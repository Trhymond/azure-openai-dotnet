namespace Rhymond.OpenAI.Models;

//public record class AskRequest(
//    [JsonProperty("question")] string Question,
//    [JsonProperty("approach")] Approach Approach,
//    [JsonProperty("overrides")] RequestOverrides? Overrides = null) : ApproachRequest(Approach);

public record class AskRequest
{
    [JsonProperty("question")]
    public required string Question { get; set; }
    [JsonProperty("approach")]
    public Approach Approach { get; set; }
    [JsonProperty("overrides")]
    public RequestOverrides? Overrides { get; set; }
}