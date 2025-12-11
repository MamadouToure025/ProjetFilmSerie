import {Component, OnInit} from '@angular/core';
import {TmdbListResponse, TmdbMovie, TmdbService} from '../../core/services/tmdb.service';
import {MovieCard} from '../../components/movie-card/movie-card';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';
import {forkJoin, map, Observable} from 'rxjs';
import {AsyncPipe} from '@angular/common';

@Component({
  selector: 'app-top250',
  imports: [
    MovieCard,
    LoadingSpinner,
    AsyncPipe
  ],
  templateUrl: './top250.html',
  styleUrl: './top250.css',
})
export class Top250 implements OnInit {
  movies$: Observable<{ results: TmdbMovie[] }> = new Observable();

  constructor(private tmdb: TmdbService) {
  }

  ngOnInit() {
    this.loadTop250();
  }

  loadTop250() {
    // On récupère 13 pages des mieux notés (260 films → on garde les 250 meilleurs)
    const pages = Array.from({length: 13}, (_, i) => i + 1);

    const requests = pages.map(page =>
      this.tmdb.getTopRatedMovies(page).pipe(
        map((response: TmdbListResponse<TmdbMovie>) => response.results)
      )
    );

    this.movies$ = forkJoin(requests).pipe(
      map((allPages: TmdbMovie[][]) => {
        const allMovies = allPages.flat();

        // Tri par note pondérée (vote_average × vote_count)
        const sorted = allMovies.sort((a, b) =>
          (b.vote_average * b.vote_count) - (a.vote_average * a.vote_count)
        );

        return {results: sorted.slice(0, 250)};
      })
    );
  }
}
