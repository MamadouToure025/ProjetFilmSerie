import {Component, inject} from '@angular/core';
import {TmdbService} from '../../core/services/tmdb.service';
import {ApiService} from '../../core/services/api.service';
import {AuthService} from '../../core/services/auth.service';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-add-serie',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-serie.html',
  styleUrl: './add-serie.css',
})

export class AddSerie {
  loadingTmdb = false;
  success = false;

  form:any;

  constructor(
    public tmdb: TmdbService,
    private api: ApiService,
    public auth: AuthService
  ) {
    const fb = inject(FormBuilder);

    this.form = fb.group({
      tmdbId: ['', [Validators.required, Validators.pattern('^[0-9]+$')]],
      name: ['', Validators.required],
      originalName: [''],
      overview: [''],
      firstAirDate: [''],
      lastAirDate: [''],
      numberOfSeasons: [null],
      numberOfEpisodes: [null],
      episodeRuntime: [null],
      posterPath: [''],
      backdropPath: [''],
      voteAverage: [0],
      genres: [[]],
      createdBy: [''],
      trailerUrl: ['']
    });
  }

  fetchFromTmdb() {
    const id = this.form.get('tmdbId')?.value;
    if (!id) return;

    this.loadingTmdb = true;
    this.tmdb.getSerie(+id).subscribe({
      next: (serie) => {
        this.form.patchValue({
          name: serie.name,
          originalName: serie.original_name,
          overview: serie.overview,
          firstAirDate: serie.first_air_date,
          lastAirDate: serie.last_air_date,
          numberOfSeasons: serie.number_of_seasons,
          numberOfEpisodes: serie.number_of_episodes,
          episodeRuntime: serie.episode_run_time?.[0] || null,
          posterPath: serie.poster_path,
          backdropPath: serie.backdrop_path,
          voteAverage: serie.vote_average,
          genres: serie.genres?.map((g: any) => g.name) || [],
          createdBy: serie.created_by?.[0]?.name || '',
          trailerUrl: this.getYoutubeTrailerKey(serie.videos?.results) || ''
        });
        this.loadingTmdb = false;
      },
      error: () => {
        alert('Série non trouvée sur TMDB');
        this.loadingTmdb = false;
      }
    });
  }

  private getYoutubeTrailerKey(videos: any[] | undefined): string | null {
    if (!videos || videos.length === 0) return null;

    const trailer = videos.find(
      (video: any) => video.site === 'YouTube' && video.type === 'Trailer'
    );

    return trailer?.key || null;
  }

  submit() {
    if (this.form.invalid) return;

    this.api.post('/series', this.form.value).subscribe({
      next: () => {
        this.success = true;
        setTimeout(() => {
          this.success = false;
          this.form.reset();
        }, 3000);
      },
      error: (err) => alert('Erreur : ' + (err.error?.message || 'Inconnue'))
    });


  }
}
