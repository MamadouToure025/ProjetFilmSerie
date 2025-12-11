import { Routes } from '@angular/router';
import {RoleGuard} from './core/guards/role-guard';

export const routes: Routes = [
  // ======================
  // ROUTES PUBLIQUES
  // ======================
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home').then(m => m.Home)
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register').then(m => m.Register)
  },
  {
    path: 'search',
    loadComponent: () => import('./pages/advanced-search/advanced-search').then(m => m.AdvancedSearch)
  },
  {
    path: 'upcoming',
    loadComponent: () => import('./pages/upcoming/upcoming').then(m => m.Upcoming)
  },
  {
    path: 'top',
    loadComponent: () => import('./pages/top250/top250').then(m => m.Top250)
  },
  {
    path: 'movie/:id',
    loadComponent: () => import('./pages/movie-detail/movie-detail').then(m => m.MovieDetail)
  },
  {
    path: 'person/:id',
    loadComponent: () => import('./pages/person/person').then(m => m.Person)
  },

  // ======================
  // UTILISATEUR CONNECTÉ
  // ======================
  {
    path: 'profile',
    loadComponent: () => import('./pages/profile/profile').then(m => m.Profile),
    canActivate: [RoleGuard],
    data: { roles: ['USER', 'MODERATOR', 'ADMIN'] }
  },

  // ======================
  // MODÉRATEUR & ADMIN
  // ======================
  {
    path: 'add-movie',
    loadComponent: () => import('./films/add-film/add-film').then(m => m.AddFilm),
    canActivate: [RoleGuard],
    data: { roles: ['MODERATOR', 'ADMIN'] }
  },
  {
    path: 'edit-movie/:id',
    loadComponent: () => import('./films/edit-film/edit-film').then(m => m.EditFilm),
    canActivate: [RoleGuard],
    data: { roles: ['MODERATOR', 'ADMIN'] }
  },
  {
    path: 'delete-movie/:id',
    loadComponent: () => import('./films/delete-film/delete-film').then(m => m.DeleteFilm),
    canActivate: [RoleGuard],
    data: { roles: ['MODERATOR', 'ADMIN'] }
  },

  {
    path: 'add-serie',
    loadComponent: () => import('./series/add-serie/add-serie').then(m => m.AddSerie),
    canActivate: [RoleGuard],
    data: { roles: ['MODERATOR', 'ADMIN'] }
  },
  {
    path: 'edit-serie/:id',
    loadComponent: () => import('./series/edit-serie/edit-serie').then(m => m.EditSerie),
    canActivate: [RoleGuard],
    data: { roles: ['MODERATOR', 'ADMIN'] }
  },
  {
    path: 'delete-serie/:id',
    loadComponent: () => import('./series/delete-serie/delete-serie').then(m => m.DeleteSerie),
    canActivate: [RoleGuard],
    data: { roles: ['MODERATOR', 'ADMIN'] }
  },

  // ======================
  // ADMIN UNIQUEMENT
  // ======================
  {
    path: 'admin',
    loadComponent: () => import('./admin/admin-panel/admin-panel').then(m => m.AdminPanel),
    canActivate: [RoleGuard],
    data: { roles: ['ADMIN'] }
  },

  // ======================
  // 404 - Toujours en dernier !
  // ======================
  {
    path: '**',
    loadComponent: () => import('./pages/not-found/not-found').then(m => m.NotFound)
  }
];
