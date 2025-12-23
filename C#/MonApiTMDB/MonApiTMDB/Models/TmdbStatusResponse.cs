using System.Text.Json.Serialization;

namespace MonApiTMDB.Models;

public class TmdbStatusResponse
{
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("status_message")]
    public string StatusMessage { get; set; }
    
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}