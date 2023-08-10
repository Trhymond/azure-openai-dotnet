namespace Rhymond.OpenAI.Models;

// public record ApproachResponse(
//     string Answer,
//     string? Thoughts,
//     string[] DataPoints,
//     string CitationBaseUrl,
//     string? Error = null);


public record class ApproachResponse
{
    [JsonProperty("answer")]
    public required string Answer { get; set; }

    [JsonProperty("thoughts")]
    public Approach Thoughts { get; set; }

    [JsonProperty("data_points")]
    public string[] DataPoints { get; set; }

    [JsonProperty("citation_base_url")]
    public string CitationBaseUrl { get; set; }

    [JsonProperty("error")]
    string? error { get; set; } = ""
}