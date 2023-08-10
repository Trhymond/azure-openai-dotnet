namespace Rhymond.OpenAI.Models;

// public record ChatResponse(
//     string Answer,
//     string? Thoughts,
//     string[] DataPoints,
//     string CitationBaseUrl,
//     string? Error = null);


public record class ChatResponse
{
    [JsonProperty("answer")]
    public required string Answer { get; set; }

    [JsonProperty("thoughts")]
    public string? Thoughts { get; set; }

    [JsonProperty("data_points")]
    public string[]? DataPoints { get; set; }

    [JsonProperty("citation_base_url")]
    public string? CitationBaseUrl { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; } = "";
}