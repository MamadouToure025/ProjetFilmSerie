import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';
import {ActivatedRoute, Router} from '@angular/router';
import {TmdbService} from '../../core/services/tmdb.service';
import {ApiService} from '../../core/services/api.service';
import {AuthService} from '../../core/services/auth.service';
import {SafeUrlPipe} from '../../core/pipes/safe-url-pipe';

@Component({
  selector: 'app-edit-serie',
  imports: [
    ReactiveFormsModule,
    LoadingSpinner,
    SafeUrlPipe
  ],
  templateUrl: './edit-serie.html',
  styleUrl: './edit-serie.css',
})
export class EditSerie implements OnInit {
  serieId!: number;
  loading = true;
  loadingTmdb = false;
  success = false;

  form:any;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    public tmdb: TmdbService,
    private api: ApiService,
    public auth: AuthService
  ) {
    const fb = inject(FormBuilder);

    this.form = fb.group({
      id: [null],
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

  ngOnInit(): void {
    this.serieId = +this.route.snapshot.paramMap.get('id')!;
    this.loadSerie();
  }

  loadSerie(): void {
    this.loading = true;
    this.api.get<any>(`/series/${this.serieId}`).subscribe({
      next: (serie) => {
        this.form.patchValue({
          id: serie.id,
          tmdbId: serie.tmdbId,
          name: serie.name,
          originalName: serie.originalName,
          overview: serie.overview,
          firstAirDate: serie.firstAirDate,
          lastAirDate: serie.lastAirDate,
          numberOfSeasons: serie.numberOfSeasons,
          numberOfEpisodes: serie.numberOfEpisodes,
          episodeRuntime: serie.episodeRuntime,
          posterPath: serie.posterPath,
          backdropPath: serie.backdropPath,
          voteAverage: serie.voteAverage,
          genres: serie.genres || [],
          createdBy: serie.createdBy,
          trailerUrl: serie.trailerUrl
        });
        this.loading = false;
      },
      error: () => {
        alert('Série non trouvée');
        this.router.navigate(['/']);
      }
    });
  }

  fetchFromTmdb(): void {
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

  submit(): void {
    if (this.form.invalid) return;

    this.api.put(`/series/${this.serieId}`, this.form.value).subscribe({
      next: () => {
        this.success = true;
        setTimeout(() => {
          this.success = false;
          this.router.navigate(['/serie', this.serieId]);
        }, 2000);
      },
      error: (err) => {
        alert('Erreur lors de la sauvegarde : ' + (err.error?.message || 'Inconnue'));
      }
    });
  }

  // Méthode privée pour extraire la clé YouTube (corrige TS7006 & TS2339)
  private getYoutubeTrailerKey(videos: any[] | undefined): string | null {
    if (!videos || videos.length === 0) return null;
    const trailer = videos.find((v: any) => v.site === 'YouTube' && v.type === 'Trailer');
    return trailer?.key || null;
  }
}
