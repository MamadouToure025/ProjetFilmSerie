import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {ApiService} from '../../core/services/api.service';
import {AuthService} from '../../core/services/auth.service';
import {CommonModule} from '@angular/common';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';
import {TmdbService} from '../../core/services/tmdb.service';

@Component({
  selector: 'app-delete-film',
  imports: [CommonModule, LoadingSpinner],
  templateUrl: './delete-film.html',
  styleUrl: './delete-film.css',
})

export class DeleteFilm implements OnInit {
  film: any;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private api: ApiService,
    public auth: AuthService,
    public tmdb: TmdbService
  ) {}

  ngOnInit() {
    const id = +this.route.snapshot.paramMap.get('id')!;
    this.api.get<any>(`/films/${id}`).subscribe({
      next: (f) => { this.film = f; this.loading = false; },
      error: () => this.router.navigate(['/'])
    });
  }

  confirmDelete() {
    if (confirm(`Supprimer définitivement "${this.film.title}" ?`)) {
      this.api.delete(`/films/${this.film.id}`).subscribe({
        next: () => this.router.navigate(['/']),
        error: () => alert('Erreur lors de la suppression')
      });
    }
  }
}
