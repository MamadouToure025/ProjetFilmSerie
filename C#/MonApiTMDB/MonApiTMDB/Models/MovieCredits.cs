using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    // La réponse globale contenant le casting et l'équipe
    public class MovieCredits
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("cast")]
        public List<CastMember> Cast { get; set; } = new();

        [JsonPropertyName("crew")]
        public List<CrewMember> Crew { get; set; } = new();
    }

    // Un acteur
    public class CastMember
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("character")]
        public string? Character { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; } // Ordre d'importance
        
        [JsonPropertyName("known_for_department")]
        public string? KnownForDepartment { get; set; }
    }

    // Un membre de l'équipe technique
    public class CrewMember
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("job")]
        public string? Job { get; set; } // ex: Director, Screenplay

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }
    }
}