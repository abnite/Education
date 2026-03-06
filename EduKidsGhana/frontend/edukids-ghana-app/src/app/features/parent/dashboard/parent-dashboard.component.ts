import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';

@Component({
  selector: 'app-parent-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="parent-dashboard">
      <div class="page-header">
        <h1>Welcome back! 👋</h1>
        <p>Here's how your children are doing</p>
      </div>

      <!-- Summary Cards -->
      <div class="summary-cards" *ngIf="dashboard()">
        <div class="sum-card">
          <span class="sum-icon">👧🏾</span>
          <span class="sum-val">{{ dashboard()?.totalChildren }}</span>
          <span class="sum-lbl">Children</span>
        </div>
        <div class="sum-card">
          <span class="sum-icon">📚</span>
          <span class="sum-val">{{ dashboard()?.totalLessonsCompleted }}</span>
          <span class="sum-lbl">Lessons Done</span>
        </div>
        <div class="sum-card">
          <span class="sum-icon">⭐</span>
          <span class="sum-val">{{ dashboard()?.totalPointsEarned | number }}</span>
          <span class="sum-lbl">Points Earned</span>
        </div>
        <div class="sum-card">
          <span class="sum-icon">🏆</span>
          <span class="sum-val">{{ dashboard()?.totalBadgesEarned }}</span>
          <span class="sum-lbl">Badges Earned</span>
        </div>
      </div>

      <!-- Children List -->
      <section class="children-section">
        <div class="section-header">
          <h2>My Children</h2>
          <a routerLink="/parent/add-child" class="btn btn-primary btn-sm">+ Add Child</a>
        </div>

        <div class="children-grid" *ngIf="dashboard()?.children?.length; else noChildren">
          <div class="child-card" *ngFor="let child of dashboard()?.children"
            (click)="viewChild(child.learnerId)">
            <div class="child-avatar">{{ child.avatarCode || '🧒🏾' }}</div>
            <div class="child-info">
              <h3>{{ child.displayName }}</h3>
              <span class="grade-badge">Grade {{ child.gradeLevel }}</span>
            </div>
            <div class="child-stats">
              <div class="mini-stat">
                <span>🔥</span>{{ child.currentStreak }}d
              </div>
              <div class="mini-stat">
                <span>⭐</span>{{ child.totalPoints | number }}
              </div>
            </div>
            <div class="mastery-preview">
              <div class="mastery-bar">
                <div class="mastery-fill" [style.width.%]="child.overallMasteryPercent"></div>
              </div>
              <span>{{ child.overallMasteryPercent | number:'1.0-0' }}% mastery</span>
            </div>
            <button class="btn btn-secondary btn-sm view-btn">View Progress →</button>
          </div>
        </div>

        <ng-template #noChildren>
          <div class="empty-children">
            <span>👧🏾</span>
            <p>No children linked yet.</p>
            <a routerLink="/parent/add-child" class="btn btn-primary">Add Your First Child</a>
          </div>
        </ng-template>
      </section>

      <!-- Weak Areas Alert -->
      <section class="alerts-section" *ngIf="dashboard()?.weakAreaAlerts?.length">
        <h2>⚠️ Needs Attention</h2>
        <div class="alert-card" *ngFor="let alert of dashboard()?.weakAreaAlerts">
          <span class="alert-icon">📉</span>
          <div class="alert-info">
            <strong>{{ alert.childName }}</strong> is struggling with
            <strong>{{ alert.topicName }}</strong> in {{ alert.subjectName }}
          </div>
          <button class="btn btn-secondary btn-sm" (click)="viewChild(alert.learnerId)">View</button>
        </div>
      </section>
    </div>
  `,
  styleUrls: ['./parent-dashboard.component.scss']
})
export class ParentDashboardComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);

  dashboard = signal<any>(null);

  ngOnInit() {
    this.http.get<ApiResponse<any>>(`${environment.apiUrl}/parents/dashboard`).subscribe({
      next: res => { if (res.success) this.dashboard.set(res.data); }
    });
  }

  viewChild(learnerId: string) {
    this.router.navigate(['/parent/child', learnerId]);
  }
}
