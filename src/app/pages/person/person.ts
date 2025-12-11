import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {TmdbPerson, TmdbService} from '../../core/services/tmdb.service';
import {MovieCard} from '../../components/movie-card/movie-card';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';


@Component({
  selector: 'app-person',
  imports: [CommonModule, MovieCard, LoadingSpinner, RouterLink],
  templateUrl: './person.html',
  styleUrl: './person.css',
})

export class Person implements OnInit {
  person: TmdbPerson | null = null;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    protected tmdb: TmdbService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : null;

    if (!id || isNaN(id)) {
      this.loading = false;
      return;
    }

    this.tmdb.getPerson(id).subscribe({
      next: (data) => {
        this.person = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getAge(): string {
    if (!this.person?.birthday) return 'Âge inconnu';
    const birth = new Date(this.person.birthday);
    const end = this.person.deathday ? new Date(this.person.deathday) : new Date();
    const age = end.getFullYear() - birth.getFullYear();
    return `${age} ans`;
  }

  getCastCredits(): any[] {
    return this.person?.credits?.cast ?? [];
  }

  getDirectorCredits(): any[] {
    return this.person?.credits?.crew?.filter(c => c.job === 'Director') ?? [];
  }
}
