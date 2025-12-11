import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {form} from '@angular/forms/signals';
import {AuthService} from '../../core/services/auth.service';
import {TmdbService} from '../../core/services/tmdb.service';
import {ApiService} from '../../core/services/api.service';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-add-film',
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './add-film.html',
  styleUrl: './add-film.css',
})

export class AddFilm implements OnInit {
  form :any;

  loading = false;
  success = false;

  constructor(
    private tmdb: TmdbService,
    private api: ApiService,
    public auth: AuthService
  ) {
    const fb = inject(FormBuilder);

    this.form = fb.group({
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

  ngOnInit() {}

  fetchFromTmdb() {
    const id = this.form.get('tmdbId')?.value;
    if (!id) return;

    this.loading = true;
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
        this.loading = false;
      },
      error: () => {
        alert('Film non trouvé sur TMDB');
        this.loading = false;
      }
    });
  }

  submit() {
    if (this.form.invalid) return;

    this.api.post('/films', this.form.value).subscribe({
      next: () => {
        this.success = true;
        setTimeout(() => this.success = false, 3000);
        this.form.reset();
      },
      error: (err) => alert('Erreur lors de l\'ajout : ' + err.error)
    });
  }
}
