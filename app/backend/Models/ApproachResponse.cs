namespace Rhymond.OpenAI.Models;

public record ApproachResponse(
    string Answer,
    string? Thoughts,
    string[] DataPoints,
    string CitationBaseUrl,
    string? Error = null);
