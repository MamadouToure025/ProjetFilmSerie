import { Component } from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {AsyncPipe} from '@angular/common';
import {AuthService} from '../../core/services/auth.service';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    AsyncPipe,
    RouterLinkActive
  ],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header{
  constructor(public auth: AuthService) {}
}
