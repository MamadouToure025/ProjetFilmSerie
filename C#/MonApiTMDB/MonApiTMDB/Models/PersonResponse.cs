using System.Text.Json.Serialization;

namespace MonApiTMDB.Models;

public class PersonResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<Person> Results { get; set; } = new();

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }
}