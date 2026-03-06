import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-admin-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="admin-shell">
      <nav class="sidebar" [class.open]="sidebarOpen()">
        <div class="sidebar-header">
          <span>🌟</span>
          <span class="logo">EduKids Ghana</span>
        </div>
        <div class="admin-badge">
          <span>🛡️</span>
          <span>Admin Panel</span>
        </div>
        <ul class="nav-menu">
          <li><a routerLink="dashboard"   routerLinkActive="active" (click)="closeSidebar()"><span>🏠</span> Dashboard</a></li>
          <li><a routerLink="users"       routerLinkActive="active" (click)="closeSidebar()"><span>👥</span> Users</a></li>
          <li><a routerLink="subjects"    routerLinkActive="active" (click)="closeSidebar()"><span>📚</span> Subjects</a></li>
          <li><a routerLink="ai-settings" routerLinkActive="active" (click)="closeSidebar()"><span>🤖</span> AI Settings</a></li>
          <li><a routerLink="analytics"   routerLinkActive="active" (click)="closeSidebar()"><span>📊</span> Analytics</a></li>
        </ul>
        <button class="logout-btn" (click)="logout()">🚪 Sign Out</button>
      </nav>
      <div class="overlay" *ngIf="sidebarOpen()" (click)="closeSidebar()"></div>
      <main class="main-content">
        <header class="topbar">
          <button class="menu-btn" (click)="sidebarOpen.set(!sidebarOpen())">☰</button>
          <span class="topbar-title">Admin Panel</span>
        </header>
        <div class="page-content"><router-outlet /></div>
      </main>
    </div>
  `,
  styleUrls: ['./admin-shell.component.scss']
})
export class AdminShellComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  sidebarOpen = signal(false);
  closeSidebar() { this.sidebarOpen.set(false); }
  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
