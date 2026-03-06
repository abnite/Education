import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';

@Component({
  selector: 'app-child-progress',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="child-progress-page" *ngIf="data(); else loading">
      <div class="page-header">
        <button class="btn btn-secondary" (click)="router.navigate(['/parent/children'])">← Back</button>
        <div class="child-title">
          <span class="avatar">{{ data()?.avatarCode || '🧒🏾' }}</span>
          <div>
            <h1>{{ data()?.displayName }}</h1>
            <span class="grade">Grade {{ data()?.gradeLevel }} · Age {{ data()?.age }}</span>
          </div>
        </div>
      </div>

      <!-- Stats Row -->
      <div class="stats-row">
        <div class="stat-box"><span>⭐</span><b>{{ data()?.totalPoints | number }}</b><small>Points</small></div>
        <div class="stat-box"><span>🔥</span><b>{{ data()?.currentStreak }}</b><small>Day Streak</small></div>
        <div class="stat-box"><span>📚</span><b>{{ data()?.lessonsCompleted }}</b><small>Lessons</small></div>
        <div class="stat-box"><span>🎯</span><b>{{ data()?.quizzesCompleted }}</b><small>Quizzes</small></div>
      </div>

      <!-- Subject Mastery -->
      <section class="mastery-section">
        <h2>Subject Mastery</h2>
        <div class="subject-bars">
          <div class="subject-bar-row" *ngFor="let s of data()?.subjectMasteries">
            <div class="subject-label">
              <span>{{ getIcon(s.subjectCode) }}</span>
              <span>{{ s.subjectName }}</span>
            </div>
            <div class="bar-track">
              <div class="bar-fill" [style.width.%]="s.averageMasteryScore"></div>
            </div>
            <span class="bar-pct">{{ s.averageMasteryScore | number:'1.0-0' }}%</span>
          </div>
        </div>
      </section>

      <!-- Weak Areas -->
      <section class="weak-areas" *ngIf="data()?.weakTopics?.length">
        <h2>⚠️ Weak Areas</h2>
        <div class="weak-list">
          <div class="weak-item" *ngFor="let w of data()?.weakTopics">
            <span class="weak-icon">📉</span>
            <div>
              <strong>{{ w.topicName }}</strong>
              <p>{{ w.subjectName }} · {{ w.failedAttempts }} failed attempts</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Strong Areas -->
      <section class="strong-areas" *ngIf="data()?.strongTopics?.length">
        <h2>🌟 Strengths</h2>
        <div class="strong-list">
          <div class="strong-item" *ngFor="let s of data()?.strongTopics">
            <span>🌟</span>
            <div>
              <strong>{{ s.topicName }}</strong>
              <p>{{ s.subjectName }} · {{ s.masteryScore | number:'1.0-0' }}% mastery</p>
            </div>
          </div>
        </div>
      </section>
    </div>

    <ng-template #loading>
      <div class="loading">📊 Loading progress…</div>
    </ng-template>
  `,
  styleUrls: ['./child-progress.component.scss']
})
export class ChildProgressComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);
  router = inject(Router);

  data = signal<any>(null);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.http.get<ApiResponse<any>>(`${environment.apiUrl}/parents/children/${id}`).subscribe({
      next: res => { if (res.success) this.data.set(res.data); }
    });
  }

  getIcon(code: string): string {
    const m: Record<string, string> = { MATHS: '🔢', ENGLISH: '📖', SCIENCE: '🔬', CODING: '💻' };
    return m[code] || '📚';
  }
}
