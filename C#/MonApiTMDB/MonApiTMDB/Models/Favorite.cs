using System.ComponentModel.DataAnnotations;

namespace MonApiTMDB.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        // IMPORTANT : int car votre User.Id est un int
        [Required]
        public int UserId { get; set; } 

        [Required]
        public int MediaId { get; set; }

        [Required]
        public string MediaType { get; set; } // "movie" ou "tv"

        public string? Title { get; set; }
        public string? PosterPath { get; set; }
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}