import {Component, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {TmdbMovie, TmdbService} from '../../core/services/tmdb.service';
import {CommonModule} from '@angular/common';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';
import {SafeUrlPipe} from '../../core/pipes/safe-url-pipe';
import {take} from 'rxjs';

@Component({
  selector: 'app-movie-detail',
  imports: [CommonModule, LoadingSpinner, SafeUrlPipe, RouterLink],
  templateUrl: './movie-detail.html',
  styleUrl: './movie-detail.css',
})
export class MovieDetail implements OnInit {
  movie: TmdbMovie | null = null;
  loading = signal(true);
  trailerUrl: string | null = null;

  private route = inject(ActivatedRoute);
  protected tmdb = inject(TmdbService);

  ngOnInit() {
    const id = +(this.route.snapshot.paramMap.get('id')!);

    this.tmdb.getMovie(id).pipe(take(1)).subscribe({
      next: (data) => {
        console.log('Movie data', data);
        this.movie = data;
        this.trailerUrl = this.getTrailerUrl();
        this.loading.set(false);
      },
      error: (err) => {
        console.error('TMDB error', err);
        this.movie = null;
        this.loading.set(false);
      }
    });
  }

    getTrailerUrl(): string | null {
    const videos = this.movie?.videos?.results;
    if (!videos) return null;
    const trailer = videos.find(v => v.site === 'YouTube' && v.type === 'Trailer');
    return trailer ? `https://www.youtube.com/embed/${trailer.key}` : null;
  }

  get director(): string | null {
    const crew = this.movie?.credits?.crew;
    if (!crew) return null;
    const director = crew.find(c => c.job === 'Director');
    return director ? director.name : null;
  }
}

