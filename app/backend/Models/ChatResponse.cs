namespace Rhymond.OpenAI.Models;

public record ChatResponse(
    string Answer,
    string? Thoughts,
    string[] DataPoints,
    string CitationBaseUrl,
    string? Error = null);
