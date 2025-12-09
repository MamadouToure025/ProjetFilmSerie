using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class GuestSessionResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("guest_session_id")]
        public string? GuestSessionId { get; set; }
    }
}