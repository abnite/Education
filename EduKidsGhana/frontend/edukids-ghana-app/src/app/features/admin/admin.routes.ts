import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./shell/admin-shell.component').then(m => m.AdminShellComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',   loadComponent: () => import('./dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent) },
      { path: 'users',       loadComponent: () => import('./users/admin-users.component').then(m => m.AdminUsersComponent) },
      { path: 'subjects',    loadComponent: () => import('./subjects/admin-subjects.component').then(m => m.AdminSubjectsComponent) },
      { path: 'ai-settings', loadComponent: () => import('./ai-settings/ai-settings.component').then(m => m.AiSettingsComponent) },
      { path: 'analytics',   loadComponent: () => import('./analytics/admin-analytics.component').then(m => m.AdminAnalyticsComponent) },
    ]
  }
];
