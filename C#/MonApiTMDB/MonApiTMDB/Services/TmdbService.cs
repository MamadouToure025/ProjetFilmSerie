using System.Text;
using MonApiTMDB.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

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
            // Récupère la clé API. Assurez-vous qu'elle est dans appsettings.Development.json sous "TMDB:ApiKey"
            //
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
        // 2. RECHERCHE AVANCÉE (Discover)
        //
        // ---------------------------------------------------------
        public async Task<TmdbResponse?> DiscoverMoviesAsync(int? genreId, int? year, int? companyId, int page = 1, string language = "fr-FR")
        {
            // Trie par popularité décroissante par défaut
            var url = $"{_baseUrl}/discover/movie?api_key={_apiKey}&language={language}&sort_by=popularity.desc&page={page}";

            if (genreId.HasValue) url += $"&with_genres={genreId.Value}";
            if (year.HasValue) url += $"&primary_release_year={year.Value}";
            if (companyId.HasValue) url += $"&with_companies={companyId.Value}";

            return await SendRequestAsync<TmdbResponse>(url);
        }

        // ---------------------------------------------------------
        // 3. FILMS EN SALLE (Now Playing)
        // ---------------------------------------------------------
        public async Task<TmdbResponse?> GetNowPlayingMoviesAsync(string language = "fr-FR", int page = 1)
        {
            var url = $"{_baseUrl}/movie/now_playing?api_key={_apiKey}&language={language}&page={page}";
            return await SendRequestAsync<TmdbResponse>(url);
        }

        // ---------------------------------------------------------
        // 4. LISTE DES GENRES
        // ---------------------------------------------------------
        public async Task<IEnumerable<Genre>> GetGenresAsync(string language = "fr-FR")
        {
            var url = $"{_baseUrl}/genre/movie/list?api_key={_apiKey}&language={language}";
            // Utilise le wrapper GenreListResponse car l'API renvoie { "genres": [...] }
            var response = await SendRequestAsync<GenreListResponse>(url);
            return response?.Genres ?? Enumerable.Empty<Genre>();
        }

        public Task<CollectionResponse?> SearchCollectionAsync(string query, int page = 1)
        {
            throw new NotImplementedException();
        }

        // ---------------------------------------------------------
        // 5. RECHERCHE DE COLLECTIONS (Sagas)
        // ---------------------------------------------------------
        public async Task<CollectionResponse?> SearchCollectionAsync(string query, int page = 1, string language = "fr-FR")
        {
            var encodedQuery = Uri.EscapeDataString(query);
            var url = $"{_baseUrl}/search/collection?api_key={_apiKey}&language={language}&query={encodedQuery}&page={page}";
            return await SendRequestAsync<CollectionResponse>(url);
        }

        public Task<CollectionDetail?> GetCollectionDetailsAsync(int collectionId, string language = "fr-FR")
        {
            throw new NotImplementedException();
        }

// ---------------------------------------------------------
// 6. DÉTAILS D'UN FILM (VERSION COMPLÈTE)
// ---------------------------------------------------------
public async Task<Movie?> GetMovieDetailAsync(int id, string language = "fr-FR")
{
    // --- APPEL 1 : INFOS GÉNÉRALES ---
    // Récupère : Titre, Résumé, Durée, Genres, Budget, Recette, Collection, Prod. Companies
    var urlDetails = $"{_baseUrl}/movie/{id}?api_key={_apiKey}&language={language}";
    var responseDetails = await _httpClient.GetAsync(urlDetails);
    if (!responseDetails.IsSuccessStatusCode) return null;

    using var doc = JsonDocument.Parse(await responseDetails.Content.ReadAsStringAsync());
    var root = doc.RootElement;

    // Création de l'objet de base
    var movie = new Movie
    {
        Id = root.GetProperty("id").GetInt32(),
        Title = root.GetProperty("title").GetString(),
        Overview = root.GetProperty("overview").GetString(),
        ReleaseDate = root.TryGetProperty("release_date", out var d) ? d.GetString() : null,
        VoteAverage = root.GetProperty("vote_average").GetDouble(),
        PosterPath = root.TryGetProperty("poster_path", out var p) ? p.GetString() : null,
        BackdropPath = root.TryGetProperty("backdrop_path", out var b) ? b.GetString() : null,
        Runtime = root.TryGetProperty("runtime", out var r) && r.ValueKind != JsonValueKind.Null ? r.GetInt32() : null,
        
        // Budget & Recette (Attention : ce sont des "long" car les montants peuvent être énormes)
        Budget = root.TryGetProperty("budget", out var bud) ? bud.GetInt64() : 0,
        Revenue = root.TryGetProperty("revenue", out var rev) ? rev.GetInt64() : 0
    };

    // Nom de la Collection (Saga)
    if (root.TryGetProperty("belongs_to_collection", out var coll) && coll.ValueKind != JsonValueKind.Null)
    {
        movie.CollectionName = coll.GetProperty("name").GetString();
    }

    // Genres (Liste -> String)
    if (root.TryGetProperty("genres", out var genresArray))
    {
        var list = genresArray.EnumerateArray()
                              .Select(g => g.GetProperty("name").GetString())
                              .Where(s => s != null);
        movie.Genre = string.Join(", ", list);
    }

    // Sociétés de Production (Liste -> String)
    if (root.TryGetProperty("production_companies", out var compsArray))
    {
        var list = compsArray.EnumerateArray()
                             .Select(c => c.GetProperty("name").GetString())
                             .Where(s => s != null);
        movie.ProductionCompanies = string.Join(", ", list);
    }

    // --- APPEL 2 : CASTING & RÉALISATEUR ---
    var urlCredits = $"{_baseUrl}/movie/{id}/credits?api_key={_apiKey}&language={language}";
    var responseCredits = await _httpClient.GetAsync(urlCredits);
    
    if (responseCredits.IsSuccessStatusCode)
    {
        using var docCredits = JsonDocument.Parse(await responseCredits.Content.ReadAsStringAsync());
        var rootCredits = docCredits.RootElement;

        // Réalisateur (Chercher le membre de l'équipe avec job = "Director")
        if (rootCredits.TryGetProperty("crew", out var crewArray))
        {
            var director = crewArray.EnumerateArray()
                                    .FirstOrDefault(c => c.GetProperty("job").GetString() == "Director");
            
            if (director.ValueKind != JsonValueKind.Undefined)
            {
                movie.Director = director.GetProperty("name").GetString();
            }
        }

        // Acteurs (Prendre les 5 premiers)
        if (rootCredits.TryGetProperty("cast", out var castArray))
        {
            var actors = castArray.EnumerateArray()
                                  .Take(5)
                                  .Select(a => a.GetProperty("name").GetString())
                                  .Where(s => s != null);
            movie.Actors = string.Join(", ", actors);
        }
    }

    // --- APPEL 3 : BANDE-ANNONCE (TRAILER) ---
    var urlVideos = $"{_baseUrl}/movie/{id}/videos?api_key={_apiKey}&language={language}"; 
    var responseVideos = await _httpClient.GetAsync(urlVideos);
    
    if (responseVideos.IsSuccessStatusCode)
    {
        using var docVideos = JsonDocument.Parse(await responseVideos.Content.ReadAsStringAsync());
        if (docVideos.RootElement.TryGetProperty("results", out var vidsArray))
        {
            // On cherche une vidéo Youtube de type "Trailer"
            var trailer = vidsArray.EnumerateArray()
                                   .FirstOrDefault(v => v.GetProperty("site").GetString() == "YouTube" 
                                                     && v.GetProperty("type").GetString() == "Trailer");

            if (trailer.ValueKind != JsonValueKind.Undefined)
            {
                var key = trailer.GetProperty("key").GetString();
                movie.TrailerUrl = $"https://www.youtube.com/watch?v={key}";
            }
        }
    }

    // --- APPEL 4 : DATE DE SORTIE BELGIQUE (BE) ---
    var urlDates = $"{_baseUrl}/movie/{id}/release_dates?api_key={_apiKey}";
    var responseDates = await _httpClient.GetAsync(urlDates);
    
    if (responseDates.IsSuccessStatusCode)
    {
        using var docDates = JsonDocument.Parse(await responseDates.Content.ReadAsStringAsync());
        if (docDates.RootElement.TryGetProperty("results", out var datesArray))
        {
            // On cherche l'entrée correspondant à la Belgique (iso_3166_1 = "BE")
            var beEntry = datesArray.EnumerateArray()
                                    .FirstOrDefault(d => d.GetProperty("iso_3166_1").GetString() == "BE");

            if (beEntry.ValueKind != JsonValueKind.Undefined 
                && beEntry.TryGetProperty("release_dates", out var releaseDatesList))
            {
                var firstRelease = releaseDatesList.EnumerateArray().FirstOrDefault();
                if (firstRelease.ValueKind != JsonValueKind.Undefined)
                {
                    // On récupère la date au format complet et on garde juste la partie YYYY-MM-DD
                    var fullDate = firstRelease.GetProperty("release_date").GetString();
                    if (!string.IsNullOrEmpty(fullDate))
                    {
                        movie.ReleaseDateBE = fullDate.Split('T')[0];
                    }
                }
            }
        }
    }

    return movie;
}

// ---------------------------------------------------------
// 8. RECHERCHE DE PERSONNE (Par Nom)
// ---------------------------------------------------------
public async Task<ActorsResponse?> SearchPersonAsync(string query, int page = 1, string language = "fr-FR")
{
    var encodedQuery = Uri.EscapeDataString(query);
    var url = $"{_baseUrl}/search/person?api_key={_apiKey}&language={language}&query={encodedQuery}&page={page}";
    return await SendRequestAsync<ActorsResponse>(url);
}

// ---------------------------------------------------------
// 9. DÉTAILS D'UNE PERSONNE (Par ID)
// ---------------------------------------------------------
public async Task<PersonDetail?> GetPersonDetailAsync(int personId, string language = "fr-FR")
{
    // Appel 1 : Infos Bio
    var urlInfo = $"{_baseUrl}/person/{personId}?api_key={_apiKey}&language={language}";
    var person = await SendRequestAsync<PersonDetail>(urlInfo);
    if (person == null) return null;

    // Appel 2 : Filmographie
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
// 10. AUTHENTIFICATION (Session Invité)
// ---------------------------------------------------------
public async Task<string?> CreateGuestSessionAsync()
{
    var url = $"{_baseUrl}/authentication/guest_session/new?api_key={_apiKey}";
    var response = await SendRequestAsync<GuestSessionResponse>(url);
    return response?.GuestSessionId;
}

// ---------------------------------------------------------
// 11. NOTER UN FILM (POST)
// ---------------------------------------------------------
public async Task<TmdbStatusResponse?> RateMovieAsync(int movieId, double rating, string guestSessionId)
{
    // URL : On doit inclure la guest_session_id en plus de l'api_key
    var url = $"{_baseUrl}/movie/{movieId}/rating?api_key={_apiKey}&guest_session_id={guestSessionId}";

    // Corps de la requête : { "value": 8.5 }
    var bodyObj = new { value = rating };
    var jsonBody = JsonSerializer.Serialize(bodyObj);
    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

    // Envoi en POST
    var response = await _httpClient.PostAsync(url, content);
    response.EnsureSuccessStatusCode();

    var jsonString = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<TmdbStatusResponse>(jsonString, 
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
}
// ...

// ---------------------------------------------------------
// 13. SUPPRIMER UNE NOTE (DELETE)
// ---------------------------------------------------------
public async Task<TmdbStatusResponse?> DeleteMovieRatingAsync(int movieId, string guestSessionId)
{
    // L'URL est la même que pour noter, mais la méthode HTTP change
    var url = $"{_baseUrl}/movie/{movieId}/rating?api_key={_apiKey}&guest_session_id={guestSessionId}";

    var response = await _httpClient.DeleteAsync(url);
    response.EnsureSuccessStatusCode();

    var jsonString = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<TmdbStatusResponse>(jsonString, 
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
}


// ---------------------------------------------------------
// 12. RECHERCHER UN FILM PAR NOM
// ---------------------------------------------------------
public async Task<TmdbResponse?> SearchMovieAsync(string query, int page = 1, string language = "fr-FR")
{
    var encodedQuery = Uri.EscapeDataString(query);
    var url = $"{_baseUrl}/search/movie?api_key={_apiKey}&language={language}&query={encodedQuery}&page={page}";
            
    return await SendRequestAsync<TmdbResponse>(url);
}

// ---------------------------------------------------------
// 15. FILMS LES MIEUX NOTÉS (Top Rated)
// ---------------------------------------------------------
public async Task<TmdbResponse?> GetTopRatedMoviesAsync(string language = "fr-FR", int page = 1)
{
    var url = $"{_baseUrl}/movie/top_rated?api_key={_apiKey}&language={language}&page={page}";
    return await SendRequestAsync<TmdbResponse>(url);
}

// ---------------------------------------------------------
// 15. CRÉDITS D'UN FILM
// ---------------------------------------------------------

        // Dans TmdbService.cs

        public async Task<MovieCredits?> GetMovieCreditsAsync(int movieId, string language = "fr-FR")
        {
            // Appel direct à l'endpoint /credits de TMDB
            var url = $"{_baseUrl}/movie/{movieId}/credits?api_key={_apiKey}&language={language}";
            return await SendRequestAsync<MovieCredits>(url);
        }   
        
        
        // ---------------------------------------------------------
        // HELPER PRIVÉ (Générique)
        // ---------------------------------------------------------
        private async Task<T?> SendRequestAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            
            // Ignore la casse (Title vs title) lors de la désérialisation
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            return JsonSerializer.Deserialize<T>(jsonString, options);
        }
    }
}