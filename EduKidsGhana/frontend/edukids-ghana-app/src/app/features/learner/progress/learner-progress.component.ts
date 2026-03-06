import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LearnerService } from '../../../core/services/learner.service';

@Component({
  selector: 'app-learner-progress',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="progress-page">
      <div class="page-header">
        <h1>📊 My Progress</h1>
        <p>Track your learning journey across all subjects</p>
      </div>

      <div class="progress-content" *ngIf="summary()">
        <div class="overall-stats">
          <div class="stat-big">
            <span class="stat-number">{{ summary()?.overallMasteryPercent | number:'1.0-0' }}%</span>
            <span class="stat-label">Overall Mastery</span>
          </div>
          <div class="stat-big">
            <span class="stat-number">{{ summary()?.totalTopicsAttempted }}</span>
            <span class="stat-label">Topics Attempted</span>
          </div>
          <div class="stat-big">
            <span class="stat-number">{{ summary()?.topicsMastered }}</span>
            <span class="stat-label">Topics Mastered</span>
          </div>
        </div>

        <!-- Subject Mastery -->
        <section class="subject-mastery" *ngFor="let subj of summary()?.subjectMasteries">
          <div class="subject-header">
            <span class="subject-icon">{{ getIcon(subj.subjectCode) }}</span>
            <h3>{{ subj.subjectName }}</h3>
            <span class="subject-avg">{{ subj.averageMasteryScore | number:'1.0-0' }}% average</span>
          </div>

          <div class="topics-list">
            <div class="topic-row" *ngFor="let t of subj.topics">
              <div class="topic-name">{{ t.topicName }}</div>
              <div class="mastery-bar-container">
                <div class="mastery-bar">
                  <div class="mastery-fill" [style.width.%]="t.masteryScore"
                    [class]="getMasteryClass(t.masteryLevel)"></div>
                </div>
                <span class="mastery-value">{{ t.masteryScore | number:'1.0-0' }}%</span>
              </div>
              <span class="mastery-level-badge" [class]="'level-' + t.masteryLevel.toLowerCase()">
                {{ t.masteryLevel }}
              </span>
            </div>
          </div>
        </section>
      </div>

      <div class="loading" *ngIf="loading()">📊 Loading your progress…</div>
    </div>
  `,
  styleUrls: ['./learner-progress.component.scss']
})
export class LearnerProgressComponent implements OnInit {
  private learnerService = inject(LearnerService);

  summary = signal<any>(null);
  loading = signal(true);

  ngOnInit() {
    this.learnerService.getProgressSummary().subscribe({
      next: res => { if (res.success) this.summary.set(res.data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  getIcon(code: string): string {
    const m: Record<string, string> = { MATHS: '🔢', ENGLISH: '📖', SCIENCE: '🔬', CODING: '💻' };
    return m[code] || '📚';
  }

  getMasteryClass(level: string): string {
    const m: Record<string, string> = {
      NotStarted: 'fill-none', Emerging: 'fill-emerging',
      Developing: 'fill-developing', Proficient: 'fill-proficient', Mastered: 'fill-mastered'
    };
    return m[level] || 'fill-none';
  }
}
