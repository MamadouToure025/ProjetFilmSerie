using System.Text;
using MonApiTMDB.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using MonApiTMDB.Models.Dtos;

namespace MonApiTMDB.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Récupération de la clé API
            _apiKey = configuration["TMDB:ApiKey"] 
                      ?? throw new InvalidOperationException("La configuration 'TMDB:ApiKey' est introuvable.");
        }

        // ---------------------------------------------------------
        // 1. FILMS POPULAIRES
        // ---------------------------------------------------------
        public async Task<TmdbResponse?> GetPopularMoviesAsync(string language = "fr-FR")
        {
            var url = $"{_baseUrl}/movie/popular?api_key={_apiKey}&language={language}";
            return await SendRequestAsync<TmdbResponse>(url);
        }

        // ---------------------------------------------------------
        // 2. TENDANCES FILMS (Trending Movies)
        // ---------------------------------------------------------
        public async Task<TmdbResponse?> GetTrendingMoviesAsync(string language = "fr-FR")
        {
            var url = $"{_baseUrl}/trending/movie/week?api_key={_apiKey}&language={language}";
            return await SendRequestAsync<TmdbResponse>(url);
        }

        // ---------------------------------------------------------
        // 3. TENDANCES SÉRIES TV (Trending TV Shows)
        // ---------------------------------------------------------
        public async Task<TvShowResponse?> GetTrendingTvShowsAsync(string language = "fr-FR")
        {
            var url = $"{_baseUrl}/trending/tv/week?api_key={_apiKey}&language={language}";
            return await SendRequestAsync<TvShowResponse>(url);
        }

        // ---------------------------------------------------------
// 4. TENDANCES PERSONNES (Trending People)
// ---------------------------------------------------------

        public async Task<ActorsResponse?> GetTrendingPeopleAsync(string language = "fr-FR")
        {
            var url = $"{_baseUrl}/trending/person/week?api_key={_apiKey}&language={language}";
            // L'API renvoie un JSON correspondant à ActorsResponse
            return await SendRequestAsync<ActorsResponse>(url);
        }

        // ---------------------------------------------------------
        // 5. FILMS EN SALLE (Now Playing)
        // ---------------------------------------------------------
        public async Task<TmdbResponse?> GetNowPlayingMoviesAsync(string language = "fr-FR", int page = 1)
        {
            var url = $"{_baseUrl}/movie/now_playing?api_key={_apiKey}&language={language}&page={page}";
            return await SendRequestAsync<TmdbResponse>(url);
        }

        // ---------------------------------------------------------
        // 6. FILMS LES MIEUX NOTÉS (Top Rated)
        // ---------------------------------------------------------
        public async Task<TmdbResponse?> GetTopRatedMoviesAsync(string language = "fr-FR", int page = 1)
        {
            var url = $"{_baseUrl}/movie/top_rated?api_key={_apiKey}&language={language}&page={page}";
            return await SendRequestAsync<TmdbResponse>(url);
        }

        // ---------------------------------------------------------
        // 7. RECHERCHE AVANCÉE (Discover)
        // ---------------------------------------------------------
        public async Task<TmdbResponse?> DiscoverMoviesAsync(int? genreId, int? year, int? companyId, int page = 1, string language = "fr-FR")
        {
            var url = $"{_baseUrl}/discover/movie?api_key={_apiKey}&language={language}&sort_by=popularity.desc&page={page}";

            if (genreId.HasValue) url += $"&with_genres={genreId.Value}";
            if (year.HasValue) url += $"&primary_release_year={year.Value}";
            if (companyId.HasValue) url += $"&with_companies={companyId.Value}";

            return await SendRequestAsync<TmdbResponse>(url);
        }

        // ---------------------------------------------------------
        // 8. LISTE DES GENRES
        // ---------------------------------------------------------
        public async Task<IEnumerable<Genre>> GetGenresAsync(string language = "fr-FR")
        {
            var url = $"{_baseUrl}/genre/movie/list?api_key={_apiKey}&language={language}";
            var response = await SendRequestAsync<GenreListResponse>(url);
            return response?.Genres ?? Enumerable.Empty<Genre>();
        }

        // ---------------------------------------------------------
        // 9. RECHERCHE DE FILMS PAR NOM
        // ---------------------------------------------------------
        public async Task<TmdbResponse?> SearchMovieAsync(string query, int page = 1, string language = "fr-FR")
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"{_baseUrl}/search/movie?api_key={_apiKey}&language={language}&query={encodedQuery}&page={page}";
            return await SendRequestAsync<TmdbResponse>(url);
        }

        // ---------------------------------------------------------
        // 10. DÉTAILS D'UN FILM (COMPLET)
        // ---------------------------------------------------------
     
        public async Task<Movie?> GetMovieDetailAsync(int id, string language = "fr-FR")
        {
            // On ajoute "credits,videos,images" 
            var url = $"{_baseUrl}/movie/{id}?api_key={_apiKey}&language={language}&append_to_response=credits,videos,images&include_image_language=null,fr,en";
    
            // Utilisez votre méthode générique (FetchDataAsync ou SendRequestAsync)
            return await FetchDataAsync<Movie>(url);
        }
        // ---------------------------------------------------------
        // 11. CRÉDITS COMPLETS (Distribution - Cast & Crew)
        // ---------------------------------------------------------
        public async Task<MovieCredits?> GetMovieCreditsAsync(int movieId, string language = "fr-FR")
        {
            var url = $"{_baseUrl}/movie/{movieId}/credits?api_key={_apiKey}&language={language}";
            return await SendRequestAsync<MovieCredits>(url);
        }
        

        // ---------------------------------------------------------
        // 12. RECHERCHE DE PERSONNE
        // ---------------------------------------------------------
        public async Task<ActorsResponse?> SearchPersonAsync(string query, int page = 1, string language = "fr-FR")
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"{_baseUrl}/search/person?api_key={_apiKey}&language={language}&query={encodedQuery}&page={page}";
            return await SendRequestAsync<ActorsResponse>(url);
        }

        // ---------------------------------------------------------
        // 13. DÉTAILS D'UNE PERSONNE
        // ---------------------------------------------------------
        public async Task<PersonDetail?> GetPersonDetailAsync(int personId, string language = "fr-FR")
        {
            var urlInfo = $"{_baseUrl}/person/{personId}?api_key={_apiKey}&language={language}";
            var person = await SendRequestAsync<PersonDetail>(urlInfo);
            if (person == null) return null;

            var urlCredits = $"{_baseUrl}/person/{personId}/movie_credits?api_key={_apiKey}&language={language}";
            var response = await _httpClient.GetAsync(urlCredits);
            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                if (doc.RootElement.TryGetProperty("cast", out var castArray))
                {
                    foreach (var item in castArray.EnumerateArray())
                    {
                        person.Credits.Add(new Movie
                        {
                            Id = item.GetProperty("id").GetInt32(),
                            Title = item.TryGetProperty("title", out var t) ? t.GetString() : null,
                            PosterPath = item.TryGetProperty("poster_path", out var p) ? p.GetString() : null,
                            ReleaseDate = item.TryGetProperty("release_date", out var r) ? r.GetString() : null
                        });
                    }
                }
            }
            return person;
        }

   
        // ---------------------------------------------------------
        // 15. COLLECTIONS
        // ---------------------------------------------------------
        public async Task<CollectionResponse?> SearchCollectionAsync(string query, int page = 1, string language = "fr-FR")
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"{_baseUrl}/search/collection?api_key={_apiKey}&language={language}&query={encodedQuery}&page={page}";
            return await SendRequestAsync<CollectionResponse>(url);
        }

        public Task<CollectionDetail?> GetCollectionDetailsAsync(int collectionId, string language = "fr-FR")
        {
            // Implémentation basique si nécessaire
            throw new NotImplementedException();
        }
// ---------------------------------------------------------
// RECHERCHE DE SÉRIES TV
// ---------------------------------------------------------
        public async Task<TvShowResponse?> SearchTvShowAsync(string query, int page = 1, string language = "fr-FR")
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"{_baseUrl}/search/tv?api_key={_apiKey}&language={language}&query={encodedQuery}&page={page}";
    
            // On réutilise votre méthode générique SendRequestAsync
            return await SendRequestAsync<TvShowResponse>(url);
        }
        
        // ---------------------------------------------------------
// DÉCOUVRIR DES SÉRIES (FILTRES)
// ---------------------------------------------------------
        public async Task<TvShowResponse?> DiscoverTvShowsAsync(int? genreId, int? year, string language = "fr-FR", int page = 1)
        {
            // On trie par popularité décroissante par défaut
            var url = $"{_baseUrl}/discover/tv?api_key={_apiKey}&language={language}&sort_by=popularity.desc&page={page}";

            // Filtre par Genre
            if (genreId.HasValue) 
            {
                url += $"&with_genres={genreId.Value}";
            }

            // Filtre par Année de sortie (first_air_date_year pour les séries)
            if (year.HasValue) 
            {
                url += $"&first_air_date_year={year.Value}";
            }

            return await SendRequestAsync<TvShowResponse>(url);
        } 
        
        // ---------------------------------------------------------
// SÉRIES DIFFUSÉES AUJOURD'HUI (Airing Today)
// ---------------------------------------------------------
        public async Task<TvShowResponse?> GetAiringTodayTvShowsAsync(string language = "fr-FR", int page = 1)
        {
            // L'endpoint est /tv/airing_today
            // On peut ajouter &timezone=Europe/Brussels si on veut être précis sur le fuseau horaire
            var url = $"{_baseUrl}/tv/airing_today?api_key={_apiKey}&language={language}&page={page}";
    
            return await SendRequestAsync<TvShowResponse>(url);
        }
        
        // ---------------------------------------------------------
        // SÉRIES POPULAIRES
        // ---------------------------------------------------------
        public async Task<TvShowResponse?> GetPopularTvShowsAsync(string language = "fr-FR", int page = 1)
        {
            var url = $"{_baseUrl}/tv/popular?api_key={_apiKey}&language={language}&page={page}";
            return await SendRequestAsync<TvShowResponse>(url);
        }
        
        // ---------------------------------------------------------
        // SÉRIES DETAILS
        // --------------------------------------------------------- 
        // 2. LA MÉTHODE POUR LES DÉTAILS DE LA SÉRIE
// Elle appelle SendRequestAsync avec les paramètres pour Images, Vidéos et Crédits
        public async Task<TvShowDetail?> GetTvShowDetailAsync(int tvShowId, string language = "fr-FR")
        {
            var url = $"{_baseUrl}/tv/{tvShowId}?api_key={_apiKey}&language={language}&append_to_response=credits,videos,images&include_image_language=null,fr,en";
    
            // On utilise bien le nom "SendRequestAsync" ici
            return await SendRequestAsync<TvShowDetail>(url);
        
        }

        // ---------------------------------------------------------
        // SÉRIES CREDITS
        // --------------------------------------------------------- 
        
        public async Task<TvShowCredits?> GetTvShowCreditsAsync(int tvShowId, string language = "fr-FR")
        {
            var url = $"{_baseUrl}/tv/{tvShowId}/credits?api_key={_apiKey}&language={language}";
            return await _httpClient.GetFromJsonAsync<TvShowCredits>(url);
        }

        
        // ==============================================================
    // 1. GESTION DES SESSIONS (AUTH)
    // ==============================================================
    public async Task<string?> CreateGuestSessionAsync()
    {
        var url = $"{_baseUrl}/authentication/guest_session/new?api_key={_apiKey}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("guest_session_id", out var idElement))
        {
            return idElement.GetString();
        }
        return null;
    }

    // ==============================================================
    // 2. GESTION DES NOTES (FILMS)
    // ==============================================================

    public async Task<TmdbStatusResponse?> RateMovieAsync(int movieId, double rating, string guestSessionId)
    {
        var url = $"{_baseUrl}/movie/{movieId}/rating?api_key={_apiKey}&guest_session_id={guestSessionId}";
        
        // Construction du Body JSON strict : { "value": 8.5 }
        var payload = new { value = rating };
        var jsonBody = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        
        // On lit la réponse PEU IMPORTE le code (Succès ou Erreur 401/404)
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TmdbStatusResponse>(jsonResponse);
    }

    public async Task<TmdbStatusResponse?> DeleteMovieRatingAsync(int movieId, string guestSessionId)
    {
        var url = $"{_baseUrl}/movie/{movieId}/rating?api_key={_apiKey}&guest_session_id={guestSessionId}";
        
        var response = await _httpClient.DeleteAsync(url);
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TmdbStatusResponse>(jsonResponse);
    }

 

    // ==============================================================
    // 3. GESTION DES NOTES (SÉRIES)
    // ==============================================================

    public async Task<TmdbStatusResponse?> RateTvShowAsync(int tvShowId, double rating, string guestSessionId)
    {
        var url = $"{_baseUrl}/tv/{tvShowId}/rating?api_key={_apiKey}&guest_session_id={guestSessionId}";
        
        var payload = new { value = rating };
        var jsonBody = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TmdbStatusResponse>(jsonResponse);
    }

    public async Task<TmdbStatusResponse?> DeleteTvShowRatingAsync(int tvShowId, string guestSessionId)
    {
        var url = $"{_baseUrl}/tv/{tvShowId}/rating?api_key={_apiKey}&guest_session_id={guestSessionId}";
        
        var response = await _httpClient.DeleteAsync(url);
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TmdbStatusResponse>(jsonResponse);
    }

 
        
 
        
        // ---------------------------------------------------------
        // MÉTHODE GÉNÉRIQUE (HELPER)
        // ---------------------------------------------------------
        // 1. LA MÉTHODE GÉNÉRIQUE (HELPER)
// Elle gère l'appel HTTP et la désérialisation JSON
        private async Task<T?> SendRequestAsync<T>(string url)
        {
            try 
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return default;

                var json = await response.Content.ReadAsStringAsync();
        
                // Options pour éviter les erreurs de majuscules/minuscules
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                };
        
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch 
            { 
                return default; 
            }
        }

// ==============================================================
// METHODE PRIVÉE (HELPER) INDISPENSABLE
// C'est elle qui fait le travail technique pour GetMovieDetailAsync, etc.
// ==============================================================
        private async Task<T?> FetchDataAsync<T>(string url)
        {
            try
            {
                // 1. On envoie la requête
                var response = await _httpClient.GetAsync(url);
        
                // 2. Si l'API répond une erreur (404, 500...), on renvoie null/vide
                if (!response.IsSuccessStatusCode) return default;

                // 3. On lit le JSON
                var json = await response.Content.ReadAsStringAsync();
        
                // 4. On configure les options (pour ignorer les majuscules/minuscules)
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                };

                // 5. On transforme le JSON en objet C# (Movie, TvShowDetail, etc.)
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch
            {
                // En cas de crash réseau ou autre, on renvoie null pour ne pas planter l'API
                return default;
            }
        }
        
 
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}