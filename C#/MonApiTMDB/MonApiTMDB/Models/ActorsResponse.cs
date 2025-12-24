using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class ActorsResponse
    {
        [JsonPropertyName("results")]
        public List<PersonDetail> Results { get; set; } = new();
    }
}