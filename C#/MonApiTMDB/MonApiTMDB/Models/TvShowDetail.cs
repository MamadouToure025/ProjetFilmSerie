using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    public class TvShowDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("first_air_date")]
        public string? FirstAirDate { get; set; }

        [JsonPropertyName("last_air_date")]
        public string? LastAirDate { get; set; }

        [JsonPropertyName("number_of_episodes")]
        public int NumberOfEpisodes { get; set; }

        [JsonPropertyName("number_of_seasons")]
        public int NumberOfSeasons { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("tagline")]
        public string? Tagline { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        // --- Listes d'objets complexes ---

        [JsonPropertyName("created_by")]
        public List<Creator> CreatedBy { get; set; } = new();

        [JsonPropertyName("seasons")]
        public List<Season> Seasons { get; set; } = new();

        [JsonPropertyName("networks")]
        public List<Network> Networks { get; set; } = new();
        
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new();
    }

    public class Creator
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }
    }

    public class Season
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("overview")]
        public string? Overview { get; set; }
        [JsonPropertyName("season_number")]
        public int SeasonNumber { get; set; }
        [JsonPropertyName("episode_count")]
        public int EpisodeCount { get; set; }
        [JsonPropertyName("air_date")]
        public string? AirDate { get; set; }
        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }
    }

    public class Network
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("logo_path")]
        public string? LogoPath { get; set; }
    }
}