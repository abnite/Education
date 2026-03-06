import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="admin-dashboard">
      <div class="page-header">
        <h1>Admin Dashboard</h1>
        <p>EduKids Ghana — Platform Overview</p>
      </div>

      <div class="kpi-grid" *ngIf="data()">
        <div class="kpi-card">
          <span class="kpi-icon">👨‍👩‍👧</span>
          <span class="kpi-val">{{ data()?.totalUsers | number }}</span>
          <span class="kpi-lbl">Total Users</span>
        </div>
        <div class="kpi-card">
          <span class="kpi-icon">🧒🏾</span>
          <span class="kpi-val">{{ data()?.totalLearners | number }}</span>
          <span class="kpi-lbl">Learners</span>
        </div>
        <div class="kpi-card">
          <span class="kpi-icon">📚</span>
          <span class="kpi-val">{{ data()?.totalLessons | number }}</span>
          <span class="kpi-lbl">Lessons</span>
        </div>
        <div class="kpi-card">
          <span class="kpi-icon">🎯</span>
          <span class="kpi-val">{{ data()?.totalQuizAttempts | number }}</span>
          <span class="kpi-lbl">Quiz Attempts</span>
        </div>
        <div class="kpi-card">
          <span class="kpi-icon">🤖</span>
          <span class="kpi-val">{{ data()?.aiRequestsToday | number }}</span>
          <span class="kpi-lbl">AI Requests Today</span>
        </div>
        <div class="kpi-card">
          <span class="kpi-icon">⭐</span>
          <span class="kpi-val">{{ data()?.totalPointsAwarded | number }}</span>
          <span class="kpi-lbl">Points Awarded</span>
        </div>
      </div>

      <!-- Quick Links -->
      <div class="quick-links">
        <a routerLink="/admin/users"       class="qlink"><span>👥</span> Manage Users</a>
        <a routerLink="/admin/subjects"    class="qlink"><span>📚</span> Manage Subjects</a>
        <a routerLink="/admin/ai-settings" class="qlink"><span>🤖</span> AI Settings</a>
        <a routerLink="/admin/analytics"   class="qlink"><span>📊</span> Analytics</a>
      </div>

      <!-- Recent Registrations -->
      <section class="recent-section" *ngIf="data()?.recentRegistrations?.length">
        <h2>Recent Registrations</h2>
        <table class="data-table">
          <thead>
            <tr><th>Name</th><th>Email</th><th>Role</th><th>Joined</th></tr>
          </thead>
          <tbody>
            <tr *ngFor="let u of data()?.recentRegistrations">
              <td>{{ u.firstName }} {{ u.lastName }}</td>
              <td>{{ u.email }}</td>
              <td><span class="role-badge">{{ u.role }}</span></td>
              <td>{{ u.createdAt | date:'shortDate' }}</td>
            </tr>
          </tbody>
        </table>
      </section>

      <!-- Subject Usage -->
      <section class="usage-section" *ngIf="data()?.subjectUsage?.length">
        <h2>Subject Usage</h2>
        <div class="usage-bars">
          <div class="usage-row" *ngFor="let s of data()?.subjectUsage">
            <span class="usage-name">{{ s.subjectName }}</span>
            <div class="usage-track">
              <div class="usage-fill" [style.width.%]="s.usagePercent"></div>
            </div>
            <span class="usage-pct">{{ s.quizAttempts }} attempts</span>
          </div>
        </div>
      </section>
    </div>
  `,
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  private http = inject(HttpClient);
  data = signal<any>(null);

  ngOnInit() {
    this.http.get<ApiResponse<any>>(`${environment.apiUrl}/admin/dashboard`).subscribe({
      next: res => { if (res.success) this.data.set(res.data); }
    });
  }
}
