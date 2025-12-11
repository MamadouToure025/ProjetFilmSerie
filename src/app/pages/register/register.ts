import {Component, inject} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {AuthService} from '../../core/services/auth.service';
import {Router} from '@angular/router';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  form: any;

  error = '';
  success = false;

  constructor(
    private auth: AuthService,
    private router: Router
  ) {
    const fb = inject(FormBuilder);
    this.form = fb.group({
      username: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, {validators: this.passwordMatchValidator});
  }

  passwordMatchValidator(form: any) {
    return form.get('password')?.value === form.get('confirmPassword')?.value
      ? null : {mismatch: true};
  }

  register() {
    if (this.form.invalid) return;

    const { username, email, password} = this.form.value;

    this.auth.register({username, email, password}).subscribe({
      next: () => {
        this.success = true;
        setTimeout(() => this.router.navigate(['/login']), 3000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Erreur lors de l’inscription';
      }
    });
  }
}
