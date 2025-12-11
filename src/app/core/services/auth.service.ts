import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, tap } from 'rxjs';
import {environment} from '../../../environments/environment';

export interface User {
  email: string;
  username: string;
  role: 'USER' | 'MODERATOR' | 'ADMIN';
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);

  private userSource = new BehaviorSubject<User | null>(null);
  user$ = this.userSource.asObservable();

  isLoggedIn(): boolean {
    return !!this.userSource.value;
  }

  isModeratorOrAdmin(): boolean {
    const user = this.userSource.value;
    return user?.role === 'MODERATOR' || user?.role === 'ADMIN';
  }

  isAdmin(): boolean {
    return this.userSource.value?.role === 'ADMIN';
  }

  getCurrentUser(): User | null {
    return this.userSource.value;
  }

  constructor() {
    const token = localStorage.getItem('mycine_jwt');
    if (token) {
      this.decodeAndSetUser(token);
    }
  }

  login(credentials: { email: string; password: string }) {
    return this.http.post<any>(`${environment.apiUrl}/auth/login`, credentials).pipe(
      tap(res => {
        localStorage.setItem('mycine_jwt', res.token);
        this.decodeAndSetUser(res.token);
      })
    );
  }



register(data: { username: string; email: string; password: string }) {
  return this.http.post(`${environment.apiUrl}/auth/register`, data);
}

logout() {
  localStorage.removeItem('mycine_jwt');
  this.userSource.next(null);
}

private decodeAndSetUser(token: string) {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    this.userSource.next({
      email: payload.sub,
      username: payload.username,
      role: payload.role
    });
  } catch (e) {
    this.logout();
  }
}
}
