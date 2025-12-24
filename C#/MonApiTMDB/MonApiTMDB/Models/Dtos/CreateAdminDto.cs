using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MonApiTMDB.Models.Dtos
{
    public class CreateAdminDto
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; } // <--- Doit être écrit exactement comme ça

        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}