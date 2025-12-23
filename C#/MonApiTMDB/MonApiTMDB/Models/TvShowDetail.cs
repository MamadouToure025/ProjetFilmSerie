using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    // ==========================================
    // 1. LA CLASSE PRINCIPALE
    // ==========================================
    public class TvShowDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("original_name")]
        public string OriginalName { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("tagline")]
        public string Tagline { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("first_air_date")]
        public string FirstAirDate { get; set; }

        [JsonPropertyName("last_air_date")]
        public string LastAirDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } 

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("number_of_episodes")]
        public int NumberOfEpisodes { get; set; }

        [JsonPropertyName("number_of_seasons")]
        public int NumberOfSeasons { get; set; }

        [JsonPropertyName("episode_run_time")]
        public List<int> EpisodeRunTime { get; set; }

        // --- LISTES IMBRIQUÉES ---

        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new();

        [JsonPropertyName("created_by")]
        public List<Creator> CreatedBy { get; set; } = new();

        [JsonPropertyName("networks")]
        public List<Network> Networks { get; set; } = new();

        [JsonPropertyName("seasons")]
        public List<Season> Seasons { get; set; } = new();

        [JsonPropertyName("last_episode_to_air")]
        public EpisodeInfo? LastEpisodeToAir { get; set; }

        [JsonPropertyName("next_episode_to_air")]
        public EpisodeInfo? NextEpisodeToAir { get; set; }

        // --- EXTENSIONS (APPEND_TO_RESPONSE) ---

        // 1. CRÉDITS (Acteurs)
        // Note: Assurez-vous d'avoir la classe TvShowCredits (si elle est dans un autre fichier, c'est parfait)
        [JsonPropertyName("credits")]
        public TvShowCredits? Credits { get; set; }

        // 2. VIDÉOS (Trailers)
        [JsonPropertyName("videos")]
        public TvShowVideoResponse? Videos { get; set; }

        // 3. IMAGES (Posters & Backdrops)
        [JsonPropertyName("images")]
        public TvShowImages? Images { get; set; }
    }

    // ==========================================
    // 2. CLASSES DE SUPPORT
    // ==========================================

    // Si vous n'avez pas de fichier Genre.cs, décommentez ceci :
    /*
    public class Genre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
    */

    public class Creator
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }
    }

    public class Network
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("logo_path")]
        public string? LogoPath { get; set; }
    }

    public class Season
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("overview")]
        public string Overview { get; set; }
        [JsonPropertyName("season_number")]
        public int SeasonNumber { get; set; }
        [JsonPropertyName("episode_count")]
        public int EpisodeCount { get; set; }
        [JsonPropertyName("air_date")]
        public string? AirDate { get; set; }
        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }
    }

    public class EpisodeInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("overview")]
        public string Overview { get; set; }
        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
        [JsonPropertyName("air_date")]
        public string AirDate { get; set; }
        [JsonPropertyName("episode_number")]
        public int EpisodeNumber { get; set; }
        [JsonPropertyName("season_number")]
        public int SeasonNumber { get; set; }
        [JsonPropertyName("still_path")]
        public string? StillPath { get; set; }
    }

    // ==========================================
    // 3. CLASSES POUR LES VIDÉOS
    // ==========================================

    public class TvShowVideoResponse
    {
        [JsonPropertyName("results")]
        public List<TvShowVideo> Results { get; set; } = new();
    }

    public class TvShowVideo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; } 
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("site")]
        public string Site { get; set; } 
        [JsonPropertyName("type")]
        public string Type { get; set; } 
        [JsonPropertyName("official")]
        public bool Official { get; set; }
        
        // Helper
        public string YouTubeUrl => Site == "YouTube" ? $"https://www.youtube.com/watch?v={Key}" : "";
    }

    // ==========================================
    // 4. CLASSES POUR LES IMAGES
    // ==========================================

    public class TvShowImages
    {
        [JsonPropertyName("backdrops")]
        public List<ImageFile> Backdrops { get; set; } = new();

        [JsonPropertyName("posters")]
        public List<ImageFile> Posters { get; set; } = new();
        
        [JsonPropertyName("logos")]
        public List<ImageFile> Logos { get; set; } = new();
    }

    public class ImageFile
    {
        [JsonPropertyName("file_path")]
        public string FilePath { get; set; }

        [JsonPropertyName("aspect_ratio")]
        public double AspectRatio { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("iso_639_1")]
        public string? LanguageCode { get; set; } // Mappe "iso_639_1" vers LanguageCode

        // Helper pour URL complète
        public string FullPath => string.IsNullOrEmpty(FilePath) 
            ? "" 
            : $"https://image.tmdb.org/t/p/original{FilePath}";
    }
}