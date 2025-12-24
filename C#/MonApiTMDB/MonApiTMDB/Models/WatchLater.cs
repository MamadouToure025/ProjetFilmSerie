using System.ComponentModel.DataAnnotations;

namespace MonApiTMDB.Models
{
    public class WatchLater
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int MediaId { get; set; }
        public string MediaType { get; set; } // "movie" ou "tv"

        // Infos Cache
        public string? Title { get; set; }
        public string? Overview { get; set; }
        public string? PosterPath { get; set; }
        public string? BackdropPath { get; set; }
        public double VoteAverage { get; set; }
        public string? ReleaseDate { get; set; }

        // --- C'EST CETTE LIGNE QUI MANQUAIT ---
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}