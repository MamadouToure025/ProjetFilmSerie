using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class TmdbResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        // Important : Doit être une List<Movie> pour correspondre à votre code
        [JsonPropertyName("results")]
        public List<Movie> Results { get; set; } = new();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }
}