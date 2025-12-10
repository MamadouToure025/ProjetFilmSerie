using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class Person
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("original_name")]
        public string? OriginalName { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }

        [JsonPropertyName("known_for_department")]
        public string? KnownForDepartment { get; set; }

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }
    }
}