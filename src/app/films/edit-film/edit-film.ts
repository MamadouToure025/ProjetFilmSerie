import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { TmdbService } from '../../core/services/tmdb.service';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import {SafeUrlPipe} from '../../core/pipes/safe-url-pipe';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-edit-film',
  imports: [CommonModule, ReactiveFormsModule, SafeUrlPipe, LoadingSpinner],
  templateUrl: './edit-film.html',
  styleUrl: './edit-film.css'
})
export class EditFilm implements OnInit {
  filmId!: number;
  loading = true;
  loadingTmdb = false;
  success = false;

  form: any;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private api: ApiService,
    public auth: AuthService,
    public tmdb: TmdbService
  ) {
    const fb = inject(FormBuilder);

    this.form = fb.group({
      id: [null],
      tmdbId: ['', [Validators.required, Validators.pattern('^[0-9]+$')]],
      title: ['', Validators.required],
      originalTitle: [''],
      overview: [''],
      releaseDate: [''],
      runtime: [null],
      posterPath: [''],
      backdropPath: [''],
      voteAverage: [0],
      genres: [[]],
      director: [''],
      trailerUrl: ['']
    });
  }

  ngOnInit() {
    this.filmId = +this.route.snapshot.paramMap.get('id')!;
    this.loadFilm();
  }

  loadFilm() {
    this.loading = true;
    this.api.get<any>(`/films/${this.filmId}`).subscribe({
      next: (film) => {
        this.form.patchValue(film);
        this.loading = false;
      },
      error: () => this.router.navigate(['/'])
    });
  }

  fetchFromTmdb() {
    const id = this.form.get('tmdbId')?.value;
    if (!id) return;

    this.loadingTmdb = true;
    this.tmdb.getMovie(+id).subscribe({
      next: (movie) => {
        this.form.patchValue({
          title: movie.title,
          originalTitle: movie.original_title,
          overview: movie.overview,
          releaseDate: movie.release_date,
          runtime: movie.runtime,
          posterPath: movie.poster_path,
          backdropPath: movie.backdrop_path,
          voteAverage: movie.vote_average,
          genres: movie.genres?.map((g: any) => g.name) || [],
          director: movie.credits?.crew?.find((c: any) => c.job === 'Director')?.name || '',
          trailerUrl: movie.videos?.results?.find((v: any) => v.type === 'Trailer' && v.site === 'YouTube')?.key || ''
        });
        this.loadingTmdb = false;
      },
      error: () => {
        alert('Film non trouvé sur TMDB');
        this.loadingTmdb = false;
      }
    });
  }

  submit() {
    if (this.form.invalid) return;

    this.api.put(`/films/${this.filmId}`, this.form.value).subscribe({
      next: () => {
        this.success = true;
        setTimeout(() => {
          this.success = false;
          this.router.navigate(['/movie', this.filmId]);
        }, 2000);
      },
      error: (err) => alert('Erreur : ' + (err.error?.message || 'Inconnue'))
    });
  }
}
