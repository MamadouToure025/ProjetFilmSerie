import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import {LoadingSpinner} from '../../components/loading-spinner/loading-spinner';
import {ApiService} from '../../core/services/api.service';

interface User {
  id: number;
  email: string;
  username: string;
  role: 'USER' | 'MODERATOR' | 'ADMIN';
}

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [CommonModule, LoadingSpinner],
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.css'
})
export class AdminPanel implements OnInit {
  users: User[] = [];
  loading = true;

  constructor(
    private api: ApiService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.loading = true;
    this.api.get<User[]>('/admin/users').subscribe({
      next: (data) => {
        this.users = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  changeRole(userId: number, newRole: 'USER' | 'MODERATOR' | 'ADMIN') {
    this.api.put(`/admin/users/${userId}/role`, { role: newRole }).subscribe({
      next: () => this.loadUsers(),
      error: (err) => alert('Erreur : ' + err.error?.message)
    });
  }

  deleteUser(userId: number) {
    if (confirm('Supprimer cet utilisateur ?')) {
      this.api.delete(`/admin/users/${userId}`).subscribe(() => {
        this.loadUsers();
      });
    }
  }
}
