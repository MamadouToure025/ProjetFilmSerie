using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    // Représente un acteur ou une personnalité
    public class Person
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("name")] public string? Name { get; set; }

        [JsonPropertyName("profile_path")] public string? ProfilePath { get; set; }

        [JsonPropertyName("known_for_department")]
        public string? KnownForDepartment { get; set; }

        [JsonPropertyName("popularity")] public double Popularity { get; set; }

        // Liste des films pour lesquels cette personne est connue
        [JsonPropertyName("known_for")] public List<Movie>? KnownFor { get; set; }
    }

} 