import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {environment} from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TmdbService {
  private readonly key = environment.tmdbApiKey;
  private readonly base = environment.tmdbBaseUrl;
  private readonly imgBase = environment.tmdbImageBaseUrl;

  constructor(private http: HttpClient) {}

  // ===================================================================
  // FILMS
  // ===================================================================

  /** Détail complet d'un film avec crédits et vidéos */
  getMovie(id: number): Observable<TmdbMovie> {
    const params = new HttpParams()
      .set('api_key', this.key)
      .set('language', 'fr-FR')
      .set('append_to_response', 'credits,videos');

    return this.http.get<TmdbMovie>(`${this.base}/movie/${id}`, { params });
  }

  getPopularMovies(page = 1): Observable<TmdbListResponse<TmdbMovie>> {
    return this.getList('movie/popular', page);
  }

  getUpcomingMovies(): Observable<TmdbListResponse<TmdbMovie>> {
    return this.http.get<TmdbListResponse<TmdbMovie>>(`${this.base}/movie/upcoming`, {
      params: { api_key: this.key, language: 'fr-FR', region: 'FR' }
    });
  }

  getTopRatedMovies(page = 1): Observable<TmdbListResponse<TmdbMovie>> {
    return this.getList('movie/top_rated', page);
  }

  // ===================================================================
  // SÉRIES TV
  // ===================================================================

  /** Détail complet d'une série avec créateurs, crédits et vidéos */
  getSerie(id: number): Observable<TmdbSerie> {
    const params = new HttpParams()
      .set('api_key', this.key)
      .set('language', 'fr-FR')
      .set('append_to_response', 'credits,videos,created_by');

    return this.http.get<TmdbSerie>(`${this.base}/tv/${id}`, { params });
  }

  getPopularSeries(page = 1): Observable<TmdbListResponse<TmdbSerie>> {
    return this.getList('tv/popular', page);
  }

  // ===================================================================
  // PERSONNES (Acteurs / Réalisateurs)
  // ===================================================================

  getPerson(id: number): Observable<TmdbPerson> {
    const params = new HttpParams()
      .set('api_key', this.key)
      .set('language', 'fr-FR')
      .set('append_to_response', 'credits');

    return this.http.get<TmdbPerson>(`${this.base}/person/${id}`, { params });
  }

  // ===================================================================
  // RECHERCHE AVANCÉE
  // ===================================================================

  /** Recherche unifiée (films ou séries) */
  search(filters: {
    query?: string;
    year?: string;
    genre?: string;
    type?: 'movie' | 'tv';
  } = {}): Observable<TmdbSearchResponse> {
    let endpoint = '/search/movie';
    if (filters.type === 'tv') endpoint = '/search/tv';
    if (!filters.query) endpoint = filters.type === 'tv' ? '/discover/tv' : '/discover/movie';

    let params = new HttpParams()
      .set('api_key', this.key)
      .set('language', 'fr-FR')
      .set('include_adult', 'false');

    if (filters.query) params = params.set('query', filters.query);
    if (filters.year) {
      if (filters.type === 'tv') {
        params = params.set('first_air_date_year', filters.year);
      } else {
        params = params.set('primary_release_year', filters.year);
      }
    }
    if (filters.genre) params = params.set('with_genres', filters.genre);

    return this.http.get<TmdbSearchResponse>(`${this.base}${endpoint}`, { params });
  }

  // ===================================================================
  // UTILITAIRES
  // ===================================================================

  /** Retourne l'URL complète d'une image TMDB */
  getImage(path: string | null, size: 'w500' | 'w780' | 'w1280' | 'original' = 'w500'): string {
    return path ? `${this.imgBase}/${size}${path}` : '/assets/no-image.jpg';
  }

  /** Extrait la clé YouTube d'une bande-annonce */
  getYoutubeTrailer(videos: Array<{ key: string; site: string; type: string }> | undefined): string | null {
    if (!videos?.length) return null;
    const trailer = videos.find(v => v.site === 'YouTube' && v.type === 'Trailer');
    return trailer?.key ? `https://www.youtube.com/embed/${trailer.key}` : null;
  }

  // ===================================================================
  // MÉTHODE PRIVÉE GÉNÉRIQUE
  // ===================================================================

  private getList<T>(endpoint: string, page = 1): Observable<TmdbListResponse<T>> {
    return this.http.get<TmdbListResponse<T>>(`${this.base}/${endpoint}`, {
      params: { api_key: this.key, language: 'fr-FR', page }
    });
  }
}

// ===================================================================
// INTERFACES TYPÉES (à mettre dans un fichier séparé si tu veux)
// ===================================================================

export interface TmdbListResponse<T> {
  page: number;
  results: T[];
  total_results: number;
  total_pages: number;
}

export interface TmdbSearchResponse extends TmdbListResponse<any> {}

export interface TmdbMovie {
  id: number;
  title: string;
  original_title: string;
  overview: string;
  poster_path: string | null;
  backdrop_path: string | null;
  release_date: string;
  vote_average: number;
  vote_count: number;
  runtime: number | null;
  genres: Array<{ id: number; name: string }>;
  credits?: { cast: any[]; crew: any[] };
  videos?: { results: Array<{ key: string; site: string; type: string }> };
}

export interface TmdbSerie {
  id: number;
  name: string;
  original_name: string;
  overview: string;
  poster_path: string | null;
  backdrop_path: string | null;
  vote_average: number;
  first_air_date: string;
  last_air_date?: string;
  number_of_seasons: number;
  number_of_episodes: number;
  episode_run_time: number[];
  genres: Array<{ id: number; name: string }>;
  created_by?: Array<{ name: string }>;
  videos?: { results: Array<{ key: string; site: string; type: string }> };
}

export interface TmdbPerson {
  id: number;
  name: string;
  profile_path: string | null;
  biography: string;
  birthday: string | null;
  deathday: string | null;
  gender: number;
  place_of_birth: string | null;
  known_for_department: string;
  credits: {
    cast: Array<{ id: number; title: string; poster_path: string | null }>;
    crew: Array<{ id: number; title: string; job: string; poster_path: string | null }>;
  };
}
