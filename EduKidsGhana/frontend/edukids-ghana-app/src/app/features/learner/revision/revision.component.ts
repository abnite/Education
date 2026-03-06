import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { LearnerService } from '../../../core/services/learner.service';

@Component({
  selector: 'app-revision',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="revision-page">
      <div class="page-header">
        <h1>🔄 Revision Queue</h1>
        <p>Topics that need a little more practice</p>
      </div>

      <div class="revision-list" *ngIf="items()?.length; else empty">
        <div class="revision-card" *ngFor="let item of items()">
          <div class="revision-icon">{{ getSubjectIcon(item.subjectCode) }}</div>
          <div class="revision-info">
            <h3>{{ item.topicName }}</h3>
            <p>{{ item.subjectName }} — Grade {{ item.gradeLevel }}</p>
            <div class="revision-meta">
              <span class="fail-count">❌ {{ item.failureCount }} failed attempts</span>
              <span class="due-label" [class.overdue]="isOverdue(item.nextRevisionAt)">
                {{ isOverdue(item.nextRevisionAt) ? '⚠️ Overdue' : '📅 Due ' + (item.nextRevisionAt | date:'shortDate') }}
              </span>
            </div>
          </div>
          <button class="btn btn-primary" (click)="startRevision(item.lessonId)">
            Revise Now →
          </button>
        </div>
      </div>

      <ng-template #empty>
        <div class="empty-state">
          <span class="empty-icon">🎉</span>
          <h3>All caught up!</h3>
          <p>You have no topics in your revision queue. Great work!</p>
          <button class="btn btn-primary" (click)="router.navigate(['/learner/dashboard'])">
            Back to Dashboard
          </button>
        </div>
      </ng-template>
    </div>
  `,
  styleUrls: ['./revision.component.scss']
})
export class RevisionComponent implements OnInit {
  private learnerService = inject(LearnerService);
  router = inject(Router);

  items = signal<any[]>([]);

  ngOnInit() {
    this.learnerService.getRevisionQueue().subscribe({
      next: res => { if (res.success) this.items.set(res.data); }
    });
  }

  isOverdue(date: string): boolean {
    return new Date(date) < new Date();
  }

  getSubjectIcon(code: string): string {
    const icons: Record<string, string> = { MATHS: '🔢', ENGLISH: '📖', SCIENCE: '🔬', CODING: '💻' };
    return icons[code] || '📚';
  }

  startRevision(lessonId: string) {
    this.router.navigate(['/learner/lesson', lessonId]);
  }
}
