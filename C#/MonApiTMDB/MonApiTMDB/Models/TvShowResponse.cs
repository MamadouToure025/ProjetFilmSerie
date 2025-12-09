using System.Text.Json.Serialization;
using TMDbLib.Objects.TvShows;

namespace MonApiTMDB.Models;

public class TvShowResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<TvShow> Results { get; set; } = new();

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }
}