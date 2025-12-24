using System.Text.Json.Serialization;

namespace MonApiTMDB.Models.Dtos
{
    // 1. L'objet principal : La Saison
    public class SeasonDetailDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } // ex: "Season 1"

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("season_number")]
        public int SeasonNumber { get; set; }

        [JsonPropertyName("air_date")]
        public string AirDate { get; set; }

        // La liste des épisodes
        [JsonPropertyName("episodes")]
        public List<EpisodeDto> Episodes { get; set; }
    }

    // 2. L'objet enfant : L'Épisode
    public class EpisodeDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("episode_number")]
        public int EpisodeNumber { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } // ex: "Winter Is Coming"

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("still_path")]
        public string StillPath { get; set; } // L'image de l'épisode

        [JsonPropertyName("air_date")]
        public string AirDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
        
        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }
    }
}