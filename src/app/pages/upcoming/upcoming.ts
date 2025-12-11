import {Component, inject} from '@angular/core';
import {TmdbService} from '../../core/services/tmdb.service';
import { CommonModule} from '@angular/common';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';
import {MovieCard} from '../../components/movie-card/movie-card';

@Component({
  selector: 'app-upcoming',
  imports: [
    CommonModule,

    LoadingSpinner,
    MovieCard
  ],
  templateUrl: './upcoming.html',
  styleUrl: './upcoming.css',
})
export class Upcoming {
  private tmdb = inject(TmdbService);

  movies$ = this.tmdb.getUpcomingMovies();

  getDaysUntil(date: string): number {
    const today = new Date();
    const release = new Date(date);
    const diff = release.getTime() - today.getTime();
    return Math.ceil(diff / (1000 * 3600 * 24));
  }

  getCountdownText(date: string): string {
    const days = this.getDaysUntil(date);
    if (days === 0) return "Aujourd'hui !";
    if (days === 1) return "Demain !";
    if (days < 0) return "Sorti";
    return `Dans ${days} jour${days > 1 ? 's' : ''}`;
  }
}
