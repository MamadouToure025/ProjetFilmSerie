using System.Text.Json.Serialization;

namespace MonApiTMDB.Models.Dtos;

public class RatingDto
{
    [JsonPropertyName("value")]
    public double Value { get; set; } // Note entre 0.5 et 10

    [JsonPropertyName("guest_session_id")]
    public string GuestSessionId { get; set; } // L'ID de session TMDB
}