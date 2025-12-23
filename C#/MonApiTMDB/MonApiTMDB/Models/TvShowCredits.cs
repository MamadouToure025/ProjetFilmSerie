using System.Text.Json.Serialization;

namespace MonApiTMDB.Models;

public class TvShowCredits
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("cast")]
    public List<TvShowCast> Cast { get; set; } = new();

    [JsonPropertyName("crew")]
    public List<TvShowCrew> Crew { get; set; } = new();
}

public class TvShowCast
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("character")]
    public string Character { get; set; } // Le nom du personnage (ex: Jon Snow)

    [JsonPropertyName("profile_path")]
    public string ProfilePath { get; set; } // La photo de l'acteur

    [JsonPropertyName("known_for_department")]
    public string KnownForDepartment { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; } // Pour trier les acteurs principaux
}

public class TvShowCrew
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("department")]
    public string Department { get; set; } // ex: Production, Writing

    [JsonPropertyName("job")]
    public string Job { get; set; } // ex: Executive Producer

    [JsonPropertyName("profile_path")]
    public string ProfilePath { get; set; }
}