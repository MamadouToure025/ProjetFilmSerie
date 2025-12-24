using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class PersonCredit
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; } // Titre (si c'est un film)

        [JsonPropertyName("name")]
        public string? Name { get; set; }  // Titre (si c'est une série)

        [JsonPropertyName("media_type")]
        public string? MediaType { get; set; } // "movie" ou "tv"

        [JsonPropertyName("character")]
        public string? Character { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("first_air_date")]
        public string? FirstAirDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        // Petit bonus pour simplifier l'affichage (Date et Titre unifiés)
        public string DisplayTitle => !string.IsNullOrEmpty(Title) ? Title : Name ?? "Titre inconnu";
        public string DisplayDate => !string.IsNullOrEmpty(ReleaseDate) ? ReleaseDate : FirstAirDate ?? "";
    }

    // Petite classe pour recevoir la réponse brute de TMDB (Cast + Crew)
    public class PersonCreditsResponse
    {
        [JsonPropertyName("cast")]
        public List<PersonCredit> Cast { get; set; } = new();
    }
}