using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class Movie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; }

        // --- NOUVEAUX CHAMPS ---

        [JsonPropertyName("budget")]
        public long Budget { get; set; }

        [JsonPropertyName("revenue")]
        public long Revenue { get; set; } // Recette mondiale

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; } // Date par défaut (souvent US)

        // Date spécifique pour la Belgique
        public string? ReleaseDateBE { get; set; } 

        // Nom de la collection (ex: "The Avengers Collection")
        public string? CollectionName { get; set; } 

        // Lien vers la vidéo Youtube
        public string? TrailerUrl { get; set; } 

        // Champs remplis manuellement (String simple pour l'affichage)
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public string? Actors { get; set; }      // Distribution
        public string? ProductionCompanies { get; set; }
    }
}