using System.Text.Json.Serialization;

namespace MonApiTMDB.Models.Dtos
{
    public class FavoriteDto
    {
        [JsonPropertyName("mediaId")]
        public int MediaId { get; set; }

        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; } // "movie" ou "tv"
    }
}