import {Component, inject} from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, switchMap, startWith } from 'rxjs';
import {MovieCard} from '../../components/movie-card/movie-card';
import {TmdbService} from '../../core/services/tmdb.service';


@Component({
  selector: 'app-advanced-search',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MovieCard],
  templateUrl: './advanced-search.html',
  styleUrl: './advanced-search.css'
})

export class AdvancedSearch {
  private fb = inject(FormBuilder);
  private tmdb = inject(TmdbService);

  searchForm = this.fb.group({
    query: [''],
    year: [''],
    genre: ['']
  });

  results$ = this.searchForm.valueChanges.pipe(
    startWith({ query: '', year: '', genre: '' }),
    debounceTime(400),
    distinctUntilChanged(),
    switchMap(() => {
      const v = this.searchForm.getRawValue();
      return this.tmdb.search({
        query: v.query || undefined,
        year: v.year || undefined,
        genre: v.genre || undefined
      });
    })
  );

  clear() {
    this.searchForm.reset();
  }
}
