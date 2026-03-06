import { Routes } from '@angular/router';

export const LEARNER_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./shell/learner-shell.component').then(m => m.LearnerShellComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard',   loadComponent: () => import('./dashboard/learner-dashboard.component').then(m => m.LearnerDashboardComponent) },
      { path: 'lesson/:id',  loadComponent: () => import('./lesson-player/lesson-player.component').then(m => m.LessonPlayerComponent) },
      { path: 'quiz/:id',    loadComponent: () => import('./quiz-player/quiz-player.component').then(m => m.QuizPlayerComponent) },
      { path: 'revision',    loadComponent: () => import('./revision/revision.component').then(m => m.RevisionComponent) },
      { path: 'achievements',loadComponent: () => import('./achievements/achievements.component').then(m => m.AchievementsComponent) },
      { path: 'progress',    loadComponent: () => import('./progress/learner-progress.component').then(m => m.LearnerProgressComponent) },
      { path: 'profile',     loadComponent: () => import('./profile/learner-profile.component').then(m => m.LearnerProfileComponent) },
    ]
  }
];
