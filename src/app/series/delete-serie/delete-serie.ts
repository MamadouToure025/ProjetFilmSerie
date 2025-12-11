import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ApiService} from '../../core/services/api.service';
import {AuthService} from '../../core/services/auth.service';
import {TmdbService} from '../../core/services/tmdb.service';
import {CommonModule} from '@angular/common';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-delete-serie',
  imports: [CommonModule, LoadingSpinner],
  templateUrl: './delete-serie.html',
  styleUrl: './delete-serie.css',
})
export class DeleteSerie implements OnInit {
  serie: any;
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
    this.api.get<any>(`/series/${id}`).subscribe({
      next: (s) => { this.serie = s; this.loading = false; },
      error: () => this.router.navigate(['/'])
    });
  }

  confirmDelete() {
    if (confirm(`Supprimer "${this.serie.name}" ?`)) {
      this.api.delete(`/series/${this.serie.id}`).subscribe(() => {
        this.router.navigate(['/']);
      });
    }
  }
}

