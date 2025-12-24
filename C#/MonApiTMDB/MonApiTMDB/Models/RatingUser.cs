using System.ComponentModel.DataAnnotations;

namespace MonApiTMDB.Models
{
    public class RatingUser
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MediaId { get; set; }
        public string MediaType { get; set; } // "movie" ou "tv"
        
        public int RatingValue { get; set; } // La note de l'utilisateur (ex: 8)

        // Données en cache pour l'affichage (comme pour les Favoris)
        public string? Title { get; set; }
        public string? Overview { get; set; }
        public string? PosterPath { get; set; }
        public string? BackdropPath { get; set; }
        public double VoteAverage { get; set; }
        public string? ReleaseDate { get; set; } // Date de sortie

        public DateTime RatedAt { get; set; } = DateTime.UtcNow;
    }
}