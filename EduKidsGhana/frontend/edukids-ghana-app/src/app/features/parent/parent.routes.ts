import { Routes } from '@angular/router';

export const PARENT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./shell/parent-shell.component').then(m => m.ParentShellComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',      loadComponent: () => import('./dashboard/parent-dashboard.component').then(m => m.ParentDashboardComponent) },
      { path: 'children',       loadComponent: () => import('./children/children.component').then(m => m.ChildrenComponent) },
      { path: 'child/:id',      loadComponent: () => import('./child-progress/child-progress.component').then(m => m.ChildProgressComponent) },
      { path: 'add-child',      loadComponent: () => import('./add-child/add-child.component').then(m => m.AddChildComponent) },
      { path: 'settings',       loadComponent: () => import('./settings/parent-settings.component').then(m => m.ParentSettingsComponent) },
    ]
  }
];
