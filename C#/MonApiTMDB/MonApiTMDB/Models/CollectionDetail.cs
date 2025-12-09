using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class CollectionDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        // La liste des films qui composent la collection
        [JsonPropertyName("parts")]
        public List<Movie> Parts { get; set; } = new();
    }
}