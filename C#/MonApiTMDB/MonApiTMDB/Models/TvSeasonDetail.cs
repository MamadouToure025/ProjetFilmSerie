using System.Text.Json.Serialization;
using MonApiTMDB.Models.Dtos; // <--- AJOUT IMPORTANT : Si vos classes TvShowCast sont dans Dtos

namespace MonApiTMDB.Models
{
    // 1. La Saison (Objet Principal)
    public class TvSeasonDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("air_date")]
        public string AirDate { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("season_number")]
        public int SeasonNumber { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("episodes")]
        public List<Episode> Episodes { get; set; }
    }

    // 2. L'Épisode
    public class Episode
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("air_date")]
        public string AirDate { get; set; }

        [JsonPropertyName("episode_number")]
        public int EpisodeNumber { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("production_code")]
        public string ProductionCode { get; set; }

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; }

        [JsonPropertyName("still_path")]
        public string StillPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        // On utilise ici vos classes existantes (TvShowCast et TvShowCrew)
        // Elles ne sont PAS redéfinies ici pour éviter les conflits.
        [JsonPropertyName("guest_stars")]
        public List<TvShowCast> GuestStars { get; set; }

        [JsonPropertyName("crew")]
        public List<TvShowCrew> Crew { get; set; }
    }
}