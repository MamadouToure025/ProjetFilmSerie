using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class TvShowResponse
    {
        [JsonPropertyName("results")]
        public List<TvShow> Results { get; set; } = new();
    }
}