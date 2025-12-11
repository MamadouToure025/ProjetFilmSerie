import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import {Router} from '@angular/router';

@Component({
  selector: 'app-movie-card',
  imports: [
    CommonModule
  ],
  templateUrl: './movie-card.html',
  styleUrl: './movie-card.css',
})
export class MovieCard {
  @Input() movie: any;
  @Input() rank?: number;

  get posterUrl(): string {
    return this.movie?.poster_path
      ? `https://image.tmdb.org/t/p/w500${this.movie.poster_path}`
      : 'https://via.placeholder.com/500x750/1a1a1a/ffffff?text=No+Image';
  }

  constructor(protected router: Router) {}
}
