﻿namespace Rhymond.OpenAI.Models;

public record RequestOverrides
{
    public bool SemanticRanker { get; set; } = true;
    public bool? SemanticCaptions { get; set; } = true;
    public string? ExcludeCategory { get; set; } = "";
    public int? Top { get; set; } = 3;
    public int? Temperature { get; set; }
    public string? PromptTemplate { get; set; } = "";
    public string? PromptTemplatePrefix { get; set; } = "";
    public string? PromptTemplateSuffix { get; set; } = "";
    public bool SuggestFollowupQuestions { get; set; } = true;
}
