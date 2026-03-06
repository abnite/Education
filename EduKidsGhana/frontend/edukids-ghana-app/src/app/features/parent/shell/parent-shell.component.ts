import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-parent-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="parent-shell">
      <nav class="sidebar" [class.open]="sidebarOpen()">
        <div class="sidebar-header">
          <span class="logo-icon">🌟</span>
          <span class="logo-text">EduKids Ghana</span>
        </div>

        <div class="user-badge">
          <span class="user-icon">👨‍👩‍👧</span>
          <span class="user-label">Parent Dashboard</span>
        </div>

        <ul class="nav-menu">
          <li><a routerLink="dashboard" routerLinkActive="active" (click)="closeSidebar()"><span>🏠</span> Dashboard</a></li>
          <li><a routerLink="children"  routerLinkActive="active" (click)="closeSidebar()"><span>👧🏾</span> My Children</a></li>
          <li><a routerLink="add-child" routerLinkActive="active" (click)="closeSidebar()"><span>➕</span> Add Child</a></li>
          <li><a routerLink="settings"  routerLinkActive="active" (click)="closeSidebar()"><span>⚙️</span> Settings</a></li>
        </ul>

        <button class="logout-btn" (click)="logout()">🚪 Sign Out</button>
      </nav>

      <div class="overlay" *ngIf="sidebarOpen()" (click)="closeSidebar()"></div>

      <main class="main-content">
        <header class="topbar">
          <button class="menu-btn" (click)="sidebarOpen.set(!sidebarOpen())">☰</button>
          <span class="topbar-title">Parent Portal</span>
        </header>
        <div class="page-content">
          <router-outlet />
        </div>
      </main>
    </div>
  `,
  styleUrls: ['./parent-shell.component.scss']
})
export class ParentShellComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  sidebarOpen = signal(false);
  closeSidebar() { this.sidebarOpen.set(false); }
  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}
