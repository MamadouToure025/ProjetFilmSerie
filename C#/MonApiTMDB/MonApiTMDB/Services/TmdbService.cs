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
            // --- APPEL 1 : INFOS GÉNÉRALES ---
            var urlDetails = $"{_baseUrl}/movie/{id}?api_key={_apiKey}&language={language}";
            var responseDetails = await _httpClient.GetAsync(urlDetails);
            if (!responseDetails.IsSuccessStatusCode) return null;

            using var doc = JsonDocument.Parse(await responseDetails.Content.ReadAsStringAsync());
            var root = doc.RootElement;

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
                Budget = root.TryGetProperty("budget", out var bud) ? bud.GetInt64() : 0,
                Revenue = root.TryGetProperty("revenue", out var rev) ? rev.GetInt64() : 0
            };

            // Collection
            if (root.TryGetProperty("belongs_to_collection", out var coll) && coll.ValueKind != JsonValueKind.Null)
            {
                movie.CollectionName = coll.GetProperty("name").GetString();
            }

            // Genres
            if (root.TryGetProperty("genres", out var genresArray))
            {
                var list = genresArray.EnumerateArray().Select(g => g.GetProperty("name").GetString()).Where(s => s != null);
                movie.Genre = string.Join(", ", list);
            }

            // Production Companies
            if (root.TryGetProperty("production_companies", out var compsArray))
            {
                var list = compsArray.EnumerateArray().Select(c => c.GetProperty("name").GetString()).Where(s => s != null);
                movie.ProductionCompanies = string.Join(", ", list);
            }

            // --- APPEL 2 : CASTING & RÉALISATEUR (Sommaire pour l'objet Movie simple) ---
            var urlCredits = $"{_baseUrl}/movie/{id}/credits?api_key={_apiKey}&language={language}";
            var responseCredits = await _httpClient.GetAsync(urlCredits);
            
            if (responseCredits.IsSuccessStatusCode)
            {
                using var docCredits = JsonDocument.Parse(await responseCredits.Content.ReadAsStringAsync());
                var rootCredits = docCredits.RootElement;

                if (rootCredits.TryGetProperty("crew", out var crewArray))
                {
                    var director = crewArray.EnumerateArray().FirstOrDefault(c => c.GetProperty("job").GetString() == "Director");
                    if (director.ValueKind != JsonValueKind.Undefined) movie.Director = director.GetProperty("name").GetString();
                }

                if (rootCredits.TryGetProperty("cast", out var castArray))
                {
                    var actors = castArray.EnumerateArray().Take(5).Select(a => a.GetProperty("name").GetString()).Where(s => s != null);
                    movie.Actors = string.Join(", ", actors);
                }
            }

            // --- APPEL 3 : TRAILER ---
            var urlVideos = $"{_baseUrl}/movie/{id}/videos?api_key={_apiKey}&language={language}"; 
            var responseVideos = await _httpClient.GetAsync(urlVideos);
            
            if (responseVideos.IsSuccessStatusCode)
            {
                using var docVideos = JsonDocument.Parse(await responseVideos.Content.ReadAsStringAsync());
                if (docVideos.RootElement.TryGetProperty("results", out var vidsArray))
                {
                    var trailer = vidsArray.EnumerateArray()
                                           .FirstOrDefault(v => v.GetProperty("site").GetString() == "YouTube" && v.GetProperty("type").GetString() == "Trailer");

                    if (trailer.ValueKind != JsonValueKind.Undefined)
                    {
                        movie.TrailerUrl = $"https://www.youtube.com/watch?v={trailer.GetProperty("key").GetString()}";
                    }
                }
            }

            // --- APPEL 4 : DATE DE SORTIE BELGIQUE ---
            var urlDates = $"{_baseUrl}/movie/{id}/release_dates?api_key={_apiKey}";
            var responseDates = await _httpClient.GetAsync(urlDates);
            
            if (responseDates.IsSuccessStatusCode)
            {
                using var docDates = JsonDocument.Parse(await responseDates.Content.ReadAsStringAsync());
                if (docDates.RootElement.TryGetProperty("results", out var datesArray))
                {
                    var beEntry = datesArray.EnumerateArray().FirstOrDefault(d => d.GetProperty("iso_3166_1").GetString() == "BE");
                    if (beEntry.ValueKind != JsonValueKind.Undefined && beEntry.TryGetProperty("release_dates", out var releaseDatesList))
                    {
                        var firstRelease = releaseDatesList.EnumerateArray().FirstOrDefault();
                        if (firstRelease.ValueKind != JsonValueKind.Undefined)
                        {
                            var fullDate = firstRelease.GetProperty("release_date").GetString();
                            if (!string.IsNullOrEmpty(fullDate)) movie.ReleaseDateBE = fullDate.Split('T')[0];
                        }
                    }
                }
            }

            return movie;
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
        // 14. AUTHENTIFICATION & NOTES
        // ---------------------------------------------------------
        public async Task<string?> CreateGuestSessionAsync()
        {
            var url = $"{_baseUrl}/authentication/guest_session/new?api_key={_apiKey}";
            var response = await SendRequestAsync<GuestSessionResponse>(url);
            return response?.GuestSessionId;
        }

        public async Task<TmdbStatusResponse?> RateMovieAsync(int movieId, double rating, string guestSessionId)
        {
            var url = $"{_baseUrl}/movie/{movieId}/rating?api_key={_apiKey}&guest_session_id={guestSessionId}";
            var bodyObj = new { value = rating };
            var jsonBody = JsonSerializer.Serialize(bodyObj);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TmdbStatusResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<TmdbStatusResponse?> DeleteMovieRatingAsync(int movieId, string guestSessionId)
        {
            var url = $"{_baseUrl}/movie/{movieId}/rating?api_key={_apiKey}&guest_session_id={guestSessionId}";
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TmdbStatusResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
        // MÉTHODE GÉNÉRIQUE (HELPER)
        // ---------------------------------------------------------
        private async Task<T?> SendRequestAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            return JsonSerializer.Deserialize<T>(jsonString, options);
        }
    }
}