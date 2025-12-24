using MonApiTMDB.Models;
using MonApiTMDB.Models.Dtos;

namespace MonApiTMDB.Services
{
    public interface ITmdbService
    {
        // --- FILMS ---
        Task<TmdbResponse?> GetPopularMoviesAsync(string language = "fr-FR");
        Task<TmdbResponse?> DiscoverMoviesAsync(int? genreId, int? year, int? companyId, int page = 1, string language = "fr-FR");// découverte de films avec filtres
        Task<TmdbResponse?> GetNowPlayingMoviesAsync(string language = "fr-FR", int page = 1);// films actuellement en salle
        Task<IEnumerable<Genre>> GetGenresAsync(string language = "fr-FR");// liste des genres
        Task<Movie?> GetMovieDetailAsync(int id, string language = "fr-FR");// détails d'un film
        Task<TmdbResponse?> SearchMovieAsync(string query, int page = 1, string language = "fr-FR");// recherche de films
        Task<TmdbResponse?>GetTopRatedMoviesAsync(string language = "fr-FR", int page = 1);// films les mieux notés
        Task<MovieCredits?> GetMovieCreditsAsync(int movieId, string language = "fr-FR");// credits d'un film
        // --- TENDANCES FILMS ---
        Task<TmdbResponse?> GetTrendingMoviesAsync(string language = "fr-FR");
        // --- TENDANCES  --- 
        // Vérifiez que cette ligne existe bien :
        Task<ActorsResponse?> GetTrendingPeopleAsync(string language = "fr-FR"); 
        // --- SERIES TV ---
        Task<TvShowResponse?> GetTrendingTvShowsAsync(string language = "fr-FR");
         
        Task<TvShowResponse?> SearchTvShowAsync(string query, int page = 1, string language = "fr-FR"); 
            Task<TvShowResponse?> DiscoverTvShowsAsync(int? genreId, int? year, string language = "fr-FR", int page = 1);
            Task<TvShowResponse?> GetAiringTodayTvShowsAsync(string language = "fr-FR", int page = 1);
            Task<TvShowResponse?> GetPopularTvShowsAsync(string language = "fr-FR", int page = 1);
            Task<TvShowDetail?> GetTvShowDetailAsync(int tvShowId, string language = "fr-FR");
            Task<TvShowCredits?> GetTvShowCreditsAsync(int tvShowId, string language = "fr-FR");
            
            // --- SAISONS TV ---
            Task<TvSeasonDetail?> GetTvSeasonAsync(int seriesId, int seasonNumber);
// --- AUTH & NOTES ---
            Task<string?> CreateGuestSessionAsync();

// FILMS
          
            Task<TmdbStatusResponse?> RateMovieAsync(int movieId, double rating, string guestSessionId);
            Task<TmdbStatusResponse?> DeleteMovieRatingAsync(int movieId, string guestSessionId);

// SÉRIES
      
            Task<TmdbStatusResponse?> RateTvShowAsync(int tvShowId, double rating, string guestSessionId);
            Task<TmdbStatusResponse?> DeleteTvShowRatingAsync(int tvShowId, string guestSessionId);
        

        // --- COLLECTIONS ---
        Task<CollectionResponse?> SearchCollectionAsync(string query, int page = 1, string language = "fr-FR");
        Task<CollectionDetail?> GetCollectionDetailsAsync(int collectionId, string language = "fr-FR");
        
        // --- PERSONNES ---
        Task<ActorsResponse?> SearchPersonAsync(string query, int page = 1, string language = "fr-FR");
        // Ajoutez le paramètre optionnel string language = "fr-FR"
        Task<PersonCreditsResponse?> GetPersonCreditsAsync(int personId, string language = "fr-FR");
        Task<ActorsResponse?> SearchActorsAsync(string query);
        Task<PersonDetail?> GetPersonDetailAsync(int personId);
       
        

        
        // --- COMPTE & FAVORIS ---
       
    }
}