using System.Text.Json.Serialization;

namespace MonApiTMDB.Models
{
    // ==========================================
    // LA CLASSE PRINCIPALE (FILM & DÉTAILS)
    // ==========================================
    public class Movie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("original_title")]
        public string? OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("tagline")]
        public string? Tagline { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; }

        // --- CHAMPS DÉTAILLÉS (Budget, Revenus...) ---

        [JsonPropertyName("budget")]
        public long Budget { get; set; }

        [JsonPropertyName("revenue")]
        public long Revenue { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; } // ex: "Released"

        // --- LISTES ET OBJETS IMBRIQUÉS ---
        
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new();

        // 1. CRÉDITS (Acteurs/Réalisateurs)
        [JsonPropertyName("credits")]
        public MovieCredits? Credits { get; set; }

        // 2. VIDÉOS (Bandes-annonces)
        [JsonPropertyName("videos")]
        public MovieVideoResponse? Videos { get; set; }

        // 3. IMAGES (Posters & Backdrops) - Utilise ImageFileMovie
        [JsonPropertyName("images")]
        public MovieImages? Images { get; set; }

        // --- CHAMPS UTILITAIRES / CALCULÉS ---

        // Récupère automatiquement le lien YouTube du premier trailer
        public string? TrailerUrl => Videos?.Results
            .FirstOrDefault(v => v.Site == "YouTube" && v.Type == "Trailer")?.YouTubeUrl;

        // URL complète pour l'affiche
        public string FullPosterPath => string.IsNullOrEmpty(PosterPath) 
            ? "" : $"https://image.tmdb.org/t/p/w500{PosterPath}";

        // URL complète pour le fond d'écran
        public string FullBackdropPath => string.IsNullOrEmpty(BackdropPath) 
            ? "" : $"https://image.tmdb.org/t/p/original{BackdropPath}";
            
        // Vos champs personnalisés supplémentaires
        public string? ReleaseDateBE { get; set; } 
        public string? CollectionName { get; set; } 
        
        // Champs "String simple" pour affichage facile (optionnels, à remplir manuellement si besoin)
        public string? GenreDisplay { get; set; }
        public string? Director { get; set; }
        public string? ActorsDisplay { get; set; }
    }

    // ==========================================
    // CLASSES POUR LES IMAGES (SPÉCIFIQUE FILMS)
    // ==========================================
    public class MovieImages
    {
        // On utilise ici ImageFileMovie au lieu de ImageFile
        [JsonPropertyName("backdrops")]
        public List<ImageFileMovie> Backdrops { get; set; } = new();

        [JsonPropertyName("posters")]
        public List<ImageFileMovie> Posters { get; set; } = new();
        
        [JsonPropertyName("logos")]
        public List<ImageFileMovie> Logos { get; set; } = new();
    }

    // Renommée pour éviter le conflit avec TvShowDetail.cs qui contient déjà "ImageFile"
    public class ImageFileMovie
    {
        [JsonPropertyName("file_path")]
        public string FilePath { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("iso_639_1")]
        public string? Iso6391 { get; set; }

        public string FullPath => string.IsNullOrEmpty(FilePath) ? "" : $"https://image.tmdb.org/t/p/original{FilePath}";
    }

    // ==========================================
    // CLASSES POUR LES VIDÉOS
    // ==========================================
    public class MovieVideoResponse
    {
        [JsonPropertyName("results")]
        public List<MovieVideo> Results { get; set; } = new();
    }

    public class MovieVideo
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
        
        public string YouTubeUrl => Site == "YouTube" ? $"https://www.youtube.com/watch?v={Key}" : "";
    }



    public class MovieCast
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("character")]
        public string Character { get; set; }
        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }
        [JsonPropertyName("order")]
        public int Order { get; set; }
        
        public string FullProfilePath => string.IsNullOrEmpty(ProfilePath) ? "" : $"https://image.tmdb.org/t/p/w200{ProfilePath}";
    }

    public class MovieCrew
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("job")]
        public string Job { get; set; }
        [JsonPropertyName("department")]
        public string Department { get; set; }
        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }
    }

    // Si Genre est déjà défini dans TvShowDetail.cs ou ailleurs dans le même namespace,
    // vous pouvez supprimer cette classe ici. Sinon, gardez-la.
    /*
    public class Genre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
    */
}