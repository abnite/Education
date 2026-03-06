import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const requiredRoles: string[] = route.data?.['roles'] ?? [];

  if (!requiredRoles.length) return true;

  const user = authService.currentUser();
  if (!user) { router.navigate(['/login']); return false; }

  const hasRole = requiredRoles.some(role => user.roles.includes(role));
  if (!hasRole) { router.navigate(['/']); return false; }

  return true;
};
