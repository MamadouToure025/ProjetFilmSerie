using System.Text.Json.Serialization;

namespace MonApiTMDB.Models.Dtos
{
    // C'est l'objet utilisé pour ENVOYER une note (POST)
    public class RatingDto
    {
        [JsonPropertyName("mediaId")]
        public int MediaId { get; set; } // <--- C'est ce champ qui manque et qui est rouge

        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; } // <--- Celui-ci aussi

        [JsonPropertyName("value")]
        public int Value { get; set; }
        
        //GuestSessionId
        [JsonPropertyName("guestSessionId")]
        public string GuestSessionId { get; set; }
    }
}