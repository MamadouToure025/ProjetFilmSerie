using System.Text.Json.Serialization;

namespace MonApiTMDB.Models;

public class GenreListResponse
{
    [JsonPropertyName("genres")]
    public List<Genre> Genres { get; set; } = new();
}

