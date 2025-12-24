using System.Text.Json.Serialization;

namespace MonApiTMDB.Models.Dtos
{
    public class RatingDtoTvUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")] // Spécifique aux séries
        public string Name { get; set; }

        [JsonPropertyName("original_name")]
        public string OriginalName { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("first_air_date")] // Spécifique aux séries
        public string FirstAirDate { get; set; }

        [JsonPropertyName("origin_country")] // Demandé dans votre JSON
        public List<string> OriginCountry { get; set; } = new List<string> { "US" }; // Valeur par défaut

        [JsonPropertyName("rating")]
        public int Rating { get; set; }
    }
}