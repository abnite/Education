import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  // Public routes
  {
    path: '',
    loadComponent: () => import('./features/public/landing/landing.component').then(m => m.LandingComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./features/public/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/public/register/register.component').then(m => m.RegisterComponent)
  },

  // Learner routes
  {
    path: 'learner',
    canActivate: [authGuard],
    loadChildren: () => import('./features/learner/learner.routes').then(m => m.LEARNER_ROUTES)
  },

  // Parent routes
  {
    path: 'parent',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Parent', 'Admin'] },
    loadChildren: () => import('./features/parent/parent.routes').then(m => m.PARENT_ROUTES)
  },

  // Admin routes
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] },
    loadChildren: () => import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES)
  },

  { path: '**', redirectTo: '' }
];
