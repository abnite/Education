import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { LearnerService } from '../../../core/services/learner.service';
import { CurriculumService } from '../../../core/services/curriculum.service';
import { LearnerDashboard } from '../../../core/models/learner.models';
import { Subject } from '../../../core/models/curriculum.models';

@Component({
  selector: 'app-learner-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <!-- Welcome Banner -->
      <div class="welcome-banner" *ngIf="dashboard()">
        <div class="welcome-text">
          <h1>Hello, {{ dashboard()?.displayName }}! 👋</h1>
          <p>Ready to learn something amazing today?</p>
        </div>
        <div class="streak-card">
          <span class="streak-fire">🔥</span>
          <span class="streak-num">{{ dashboard()?.currentStreak }}</span>
          <span class="streak-label">day streak</span>
        </div>
      </div>

      <!-- Quick Stats -->
      <div class="stats-row" *ngIf="dashboard()">
        <div class="stat-card">
          <span class="stat-icon">⭐</span>
          <span class="stat-val">{{ dashboard()?.totalPoints | number }}</span>
          <span class="stat-lbl">Total Points</span>
        </div>
        <div class="stat-card">
          <span class="stat-icon">📚</span>
          <span class="stat-val">{{ dashboard()?.lessonsCompleted }}</span>
          <span class="stat-lbl">Lessons Done</span>
        </div>
        <div class="stat-card">
          <span class="stat-icon">🎯</span>
          <span class="stat-val">{{ dashboard()?.quizzesTaken }}</span>
          <span class="stat-lbl">Quizzes Taken</span>
        </div>
        <div class="stat-card">
          <span class="stat-icon">🏆</span>
          <span class="stat-val">{{ dashboard()?.badgesEarned }}</span>
          <span class="stat-lbl">Badges Earned</span>
        </div>
      </div>

      <!-- Next Lesson Recommendation -->
      <div class="recommendation-card" *ngIf="dashboard()?.nextLesson">
        <div class="rec-label">📖 Recommended Next</div>
        <div class="rec-content">
          <h3>{{ dashboard()?.nextLesson?.title }}</h3>
          <p>{{ dashboard()?.nextLesson?.reason }}</p>
          <button class="btn btn-primary" (click)="startLesson(dashboard()?.nextLesson?.lessonId!)">
            Start Lesson →
          </button>
        </div>
      </div>

      <!-- Today's Challenge -->
      <div class="challenge-card" *ngIf="dashboard()?.dailyChallenge">
        <div class="challenge-header">
          <span class="challenge-icon">⚡</span>
          <span>Today's Challenge</span>
          <span class="challenge-pts">+{{ dashboard()?.dailyChallenge?.rewardPoints }} pts</span>
        </div>
        <p>{{ dashboard()?.dailyChallenge?.description }}</p>
        <div class="challenge-progress">
          <div class="progress-bar">
            <div class="progress-fill"
              [style.width.%]="getChallengeProgress()"></div>
          </div>
          <span>{{ dashboard()?.dailyChallenge?.currentValue }} / {{ dashboard()?.dailyChallenge?.targetValue }}</span>
        </div>
      </div>

      <!-- Subject Progress -->
      <section class="subjects-section">
        <h2>Your Subjects</h2>
        <div class="subjects-grid">
          <div class="subject-tile" *ngFor="let s of dashboard()?.subjectProgress"
            [class]="'subject-' + s.subjectCode.toLowerCase()"
            (click)="goToSubject(s)">
            <div class="subject-icon">{{ getSubjectIcon(s.subjectCode) }}</div>
            <div class="subject-info">
              <h4>{{ s.subjectName }}</h4>
              <div class="mastery-bar">
                <div class="mastery-fill" [style.width.%]="s.masteryScore"></div>
              </div>
              <span class="mastery-label">{{ s.masteryLevel }}</span>
            </div>
          </div>
        </div>
      </section>

      <!-- Recent Activity -->
      <section class="activity-section" *ngIf="dashboard()?.recentActivity?.length">
        <h2>Recent Activity</h2>
        <ul class="activity-list">
          <li *ngFor="let act of dashboard()?.recentActivity">
            <span class="act-icon">{{ getActivityIcon(act.type) }}</span>
            <div class="act-details">
              <span class="act-title">{{ act.title }}</span>
              <span class="act-time">{{ act.timestamp | date:'short' }}</span>
            </div>
            <span class="act-pts" *ngIf="act.pointsEarned">+{{ act.pointsEarned }} pts</span>
          </li>
        </ul>
      </section>

      <!-- New Badges -->
      <section class="badges-section" *ngIf="dashboard()?.newBadges?.length">
        <h2>🎉 New Badges!</h2>
        <div class="badges-row">
          <div class="badge-item" *ngFor="let b of dashboard()?.newBadges">
            <span class="badge-icon">{{ b.icon }}</span>
            <span class="badge-name">{{ b.name }}</span>
          </div>
        </div>
      </section>

      <div class="loading-overlay" *ngIf="loading()">
        <div class="spinner-text">📚 Loading your dashboard…</div>
      </div>
    </div>
  `,
  styleUrls: ['./learner-dashboard.component.scss']
})
export class LearnerDashboardComponent implements OnInit {
  private learnerService = inject(LearnerService);
  private router = inject(Router);

  dashboard = signal<LearnerDashboard | null>(null);
  loading = signal(true);

  ngOnInit() {
    this.learnerService.getDashboard().subscribe({
      next: res => {
        if (res.success) this.dashboard.set(res.data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  startLesson(lessonId: string) {
    this.router.navigate(['/learner/lesson', lessonId]);
  }

  goToSubject(subject: any) {
    this.router.navigate(['/learner/progress'], { queryParams: { subject: subject.subjectCode } });
  }

  getChallengeProgress(): number {
    const ch = this.dashboard()?.dailyChallenge;
    if (!ch || ch.targetValue === 0) return 0;
    return Math.min(100, (ch.currentValue / ch.targetValue) * 100);
  }

  getSubjectIcon(code: string): string {
    const icons: Record<string, string> = { MATHS: '🔢', ENGLISH: '📖', SCIENCE: '🔬', CODING: '💻' };
    return icons[code] || '📚';
  }

  getActivityIcon(type: string): string {
    const icons: Record<string, string> = { lesson: '📖', quiz: '🎯', badge: '🏆', streak: '🔥' };
    return icons[type] || '✅';
  }
}
