
namespace Rhymond.OpenAI.Models;

public record class Employee {

    [JsonProperty("id")]
    public required string Id { get; set;}

    [JsonProperty("first_name")]
    public required string FirstName { get; set;}

    [JsonProperty("last_name")]
    public required string LastName { get; set;}

    [JsonProperty("title")]
    public string? Title { get; set;}

    [JsonProperty("plan")]
    public required string Plan { get; set;}

    [JsonProperty("plan_type")]
    public required string PlanType { get; set;}

    [JsonProperty("display_name")]
    public required string DisplayName { get; set;}

    [JsonProperty("display_name")]
    public required int DependendCount { get; set;}

    [JsonProperty("has_hsa")]
    public required int HasHsa { get; set;}
}
