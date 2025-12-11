import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const RoleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const requiredRoles = route.data['roles'] as Array<'USER' | 'MODERATOR' | 'ADMIN'>;

  const user = authService.getCurrentUser();

  if (!user) {
    router.navigate(['/login']);
    return false;
  }

  if (requiredRoles && !requiredRoles.includes(user.role)) {
    router.navigate(['/home']);
    return false;
  }

  return true;
};
