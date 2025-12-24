using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class PersonDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("biography")]
        public string? Biography { get; set; }

        [JsonPropertyName("birthday")]
        public string? Birthday { get; set; }

        [JsonPropertyName("place_of_birth")]
        public string? PlaceOfBirth { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }

        // C'est ici qu'on stocke la filmographie
        [JsonPropertyName("credits")]
        public List<PersonCredit> Credits { get; set; } = new();
    }
}