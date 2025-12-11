import {Component, OnInit} from '@angular/core';
import {MovieCard} from '../../components/movie-card/movie-card';
import {TmdbService} from '../../core/services/tmdb.service';
import {CommonModule, SlicePipe} from '@angular/common';
import {RouterLink} from '@angular/router';
import {Observable} from 'rxjs';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';

interface MovieList {
  results: any[];
}

@Component({
  selector: 'app-home',
  imports: [
    MovieCard,
    CommonModule,
    LoadingSpinner
  ],
  templateUrl: './home.html',
  styleUrl: './home.css',
})

export class Home implements OnInit {
  upcoming$!: Observable<MovieList>;
  topRated$!: Observable<MovieList>;

  constructor(private tmdb: TmdbService) {}

  ngOnInit() {
    this.upcoming$ = this.tmdb.getUpcomingMovies();
    this.topRated$ = this.tmdb.getTopRatedMovies();
  }
}
