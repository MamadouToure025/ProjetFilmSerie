import {Component, OnInit} from '@angular/core';
import {AuthService} from '../../core/services/auth.service';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-profile',
  imports: [
    RouterLink
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  constructor(public auth: AuthService) {}

  getRoleLabel(): string {
    const role = this.auth.getCurrentUser()?.role;
    switch (role) {
      case 'ADMIN': return 'Administrateur';
      case 'MODERATOR': return 'Modérateur';
      case 'USER': return 'Membre';
      default: return 'Inconnu';
    }
  }
}
