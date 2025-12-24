using System.Text.Json.Serialization;

namespace MonApiTMDB.Models.Dtos
{
    public class UserUpdateDto
    {
        // Optionnel : L'utilisateur n'est pas obligé de tout changer
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}