using MonApiTMDB.Models;

namespace MonApiTMDB.Services
{
    public interface ITmdbService
    {
        // ... Vos méthodes existantes ...
        Task<TmdbResponse?> GetPopularMoviesAsync(string language = "fr-FR");
        Task<TmdbResponse?> DiscoverMoviesAsync(int? genreId, int? year, int? companyId, int page = 1, string language = "fr-FR");
        Task<TmdbResponse?> GetNowPlayingMoviesAsync(string language = "fr-FR", int page = 1);
        Task<IEnumerable<Genre>> GetGenresAsync(string language = "fr-FR");
        
        Task<CollectionResponse?> SearchCollectionAsync(string query, int page = 1, string language = "fr-FR");
        Task<CollectionDetail?> GetCollectionDetailsAsync(int collectionId, string language = "fr-FR");
        
        Task<PersonResponse?> SearchPersonAsync(string query, int page = 1, string language = "fr-FR");
        Task<PersonDetail?> GetPersonDetailAsync(int personId, string language = "fr-FR");
        
        Task<Movie?> GetMovieDetailAsync(int id, string language = "fr-FR"); // Celui qu'on a fait juste avant
        
        // --- NOUVELLE MÉTHODE : Chercher un film par nom ---
        Task<TmdbResponse?> SearchMovieAsync(string query, int page = 1, string language = "fr-FR");
        
        // Auth & Rating
        Task<string?> CreateGuestSessionAsync(); // Crée une session invité et retourne son ID
        Task<TmdbStatusResponse?> RateMovieAsync(int movieId, double rating, string guestSessionId);
        
      
        
        // --- NOUVELLE MÉTHODE : Supprimer la note ---
        Task<TmdbStatusResponse?> DeleteMovieRatingAsync(int movieId, string guestSessionId);
        
        Task<TvShowResponse?> SearchTvShowAsync(string query, int page = 1, string language = "fr-FR");

        // --- NOUVELLE MÉTHODE : Détails complets d'une série (par ID) ---
        Task<TvShowDetail?> GetTvShowDetailAsync(int id, string language = "fr-FR");
        
        public interface ITmdbService
        {
            // ... (Autres méthodes) ...
            Task<TvShowResponse?> SearchTvShowAsync(string query, int page = 1, string language = "fr-FR");

            // --- NOUVELLE MÉTHODE : Détails complets d'une série (par ID) ---
            Task<TvShowDetail?> GetTvShowDetailAsync(int id, string language = "fr-FR");
        }
    }
}