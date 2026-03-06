import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { LearnerService } from '../../../core/services/learner.service';
import { LearnerProfile } from '../../../core/models/learner.models';

@Component({
  selector: 'app-learner-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="learner-shell">
      <!-- Sidebar -->
      <nav class="sidebar" [class.open]="sidebarOpen()">
        <div class="sidebar-header">
          <span class="logo-icon">🌟</span>
          <span class="logo-text">EduKids Ghana</span>
        </div>

        <div class="learner-info" *ngIf="profile()">
          <div class="avatar">{{ profile()?.avatarCode || '🧒🏾' }}</div>
          <div class="learner-meta">
            <div class="learner-name">{{ profile()?.displayName }}</div>
            <div class="learner-grade">Grade {{ profile()?.gradeLevel }}</div>
          </div>
        </div>

        <div class="points-bar">
          <span class="points-label">⭐ Points</span>
          <span class="points-value">{{ profile()?.totalPoints || 0 }}</span>
        </div>

        <ul class="nav-menu">
          <li>
            <a routerLink="dashboard" routerLinkActive="active" (click)="closeSidebar()">
              <span class="nav-icon">🏠</span> Dashboard
            </a>
          </li>
          <li>
            <a routerLink="revision" routerLinkActive="active" (click)="closeSidebar()">
              <span class="nav-icon">🔄</span> Revision Queue
            </a>
          </li>
          <li>
            <a routerLink="achievements" routerLinkActive="active" (click)="closeSidebar()">
              <span class="nav-icon">🏆</span> Achievements
            </a>
          </li>
          <li>
            <a routerLink="progress" routerLinkActive="active" (click)="closeSidebar()">
              <span class="nav-icon">📊</span> My Progress
            </a>
          </li>
          <li>
            <a routerLink="profile" routerLinkActive="active" (click)="closeSidebar()">
              <span class="nav-icon">👤</span> Profile
            </a>
          </li>
        </ul>

        <button class="btn btn-secondary logout-btn" (click)="logout()">🚪 Sign Out</button>
      </nav>

      <!-- Overlay for mobile -->
      <div class="overlay" *ngIf="sidebarOpen()" (click)="closeSidebar()"></div>

      <!-- Main content -->
      <main class="main-content">
        <header class="topbar">
          <button class="menu-btn" (click)="sidebarOpen.set(!sidebarOpen())">☰</button>
          <div class="streak-display" *ngIf="profile()">
            🔥 {{ profile()?.currentStreak }} day streak
          </div>
        </header>
        <div class="page-content">
          <router-outlet />
        </div>
      </main>
    </div>
  `,
  styleUrls: ['./learner-shell.component.scss']
})
export class LearnerShellComponent implements OnInit {
  private authService = inject(AuthService);
  private learnerService = inject(LearnerService);
  private router = inject(Router);

  profile = signal<LearnerProfile | null>(null);
  sidebarOpen = signal(false);

  ngOnInit() {
    this.learnerService.getProfile().subscribe({
      next: res => { if (res.success) this.profile.set(res.data); }
    });
  }

  closeSidebar() { this.sidebarOpen.set(false); }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
