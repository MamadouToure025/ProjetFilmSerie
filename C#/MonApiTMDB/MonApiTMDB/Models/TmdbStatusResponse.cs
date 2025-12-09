using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    // pour noter un film
    // Modèle pour lire la réponse: { "status_code": 1, "status_message": "Success." }
    public class TmdbStatusResponse
    {
        [JsonPropertyName("status_code")]
        public int StatusCode { get; set; }

        [JsonPropertyName("status_message")]
        public string? StatusMessage { get; set; }
    }
}