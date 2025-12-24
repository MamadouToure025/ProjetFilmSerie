using System.Text.Json.Serialization;

namespace MonApiTMDB.Models.Dtos
{
    public class RatingDtoUser
    {
        // L'ID du film (ex: 550)
        [JsonPropertyName("id")]
        public int Id { get; set; } 

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        // --- LE CHAMP SPÉCIAL ---
        // La note donnée par l'utilisateur (ex: 8)
        [JsonPropertyName("rating")]
        public int Rating { get; set; }
    }
}