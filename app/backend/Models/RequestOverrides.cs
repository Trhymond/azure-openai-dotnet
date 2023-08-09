using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Rhymond.OpenAI.Models;

public record RequestOverrides
{
    [JsonProperty("semantic_ranker")]
    public bool SemanticRanker { get; set; } = true;
    [JsonProperty("semantic_captions")]
    public bool? SemanticCaptions { get; set; } = true;
    [JsonProperty("exclude_category")]
    public string? ExcludeCategory { get; set; } = "";
    [JsonProperty("top")]
    public int? Top { get; set; } = 3;
    [JsonProperty("temerature")]
    public int? Temperature { get; set; }
    [JsonProperty("prompt_template")]
    public string? PromptTemplate { get; set; } = "";
    [JsonProperty("prompt_template_prefix")]
    public string? PromptTemplatePrefix { get; set; } = "";
    [JsonProperty("prompt_template_suffix")]
    public string? PromptTemplateSuffix { get; set; } = "";
    [JsonProperty("suggest_followup_questions")]
    public bool SuggestFollowupQuestions { get; set; } = true;
}
