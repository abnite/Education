import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CurriculumService } from '../../../core/services/curriculum.service';
import { LessonPlayerPayload, LessonSection } from '../../../core/models/curriculum.models';

@Component({
  selector: 'app-lesson-player',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="lesson-player" *ngIf="payload(); else loading">
      <!-- Header -->
      <div class="lesson-header">
        <button class="btn btn-secondary back-btn" (click)="goBack()">← Back</button>
        <div class="lesson-meta">
          <h1>{{ payload()?.lesson?.title }}</h1>
          <div class="meta-badges">
            <span class="badge subject-badge">{{ payload()?.lesson?.subjectName }}</span>
            <span class="badge grade-badge">Grade {{ payload()?.lesson?.gradeLevel }}</span>
            <span class="badge time-badge">⏱ {{ payload()?.lesson?.estimatedMinutes }} min</span>
          </div>
        </div>
        <div class="lesson-progress-indicator">
          <span>{{ currentSection() + 1 }} / {{ payload()?.lesson?.sections?.length }}</span>
        </div>
      </div>

      <!-- Objectives -->
      <div class="objectives-card" *ngIf="currentSection() === 0 && payload()?.lesson?.objectives?.length">
        <h3>🎯 What You'll Learn</h3>
        <ul>
          <li *ngFor="let obj of payload()?.lesson?.objectives">✓ {{ obj.description }}</li>
        </ul>
      </div>

      <!-- AI Enhancement -->
      <div class="ai-enhancement" *ngIf="payload()?.aiEnhancement">
        <span class="ai-label">🤖 AI Tutor Says</span>
        <p>{{ payload()?.aiEnhancement }}</p>
      </div>

      <!-- Section Content -->
      <div class="section-card" *ngIf="currentSectionData()">
        <h2>{{ currentSectionData()?.title }}</h2>
        <div class="section-content" [innerHTML]="currentSectionData()?.contentHtml || currentSectionData()?.content"></div>

        <!-- Audio Player -->
        <div class="audio-player" *ngIf="currentSectionData()?.audioUrl && audioEnabled()">
          <audio controls [src]="currentSectionData()?.audioUrl">
            Your browser does not support audio.
          </audio>
        </div>
      </div>

      <!-- Examples -->
      <div class="examples-section" *ngIf="shouldShowExamples()">
        <h3>📝 Examples</h3>
        <div class="example-card" *ngFor="let ex of payload()?.lesson?.examples">
          <div class="example-title">{{ ex.title }}</div>
          <div class="example-content">{{ ex.content }}</div>
          <div class="example-solution" *ngIf="showSolutions()">
            <strong>Solution:</strong> {{ ex.solution }}
            <p class="example-explanation">{{ ex.explanation }}</p>
          </div>
          <button class="btn btn-secondary btn-sm" *ngIf="!showSolutions()"
            (click)="showSolutions.set(true)">Show Solution 💡</button>
        </div>
      </div>

      <!-- Navigation -->
      <div class="lesson-nav">
        <button class="btn btn-secondary" (click)="prevSection()" [disabled]="currentSection() === 0">
          ← Previous
        </button>

        <div class="section-dots">
          <span class="dot" *ngFor="let s of payload()?.lesson?.sections; let i = index"
            [class.active]="i === currentSection()" (click)="currentSection.set(i)">
          </span>
        </div>

        <button class="btn btn-primary" (click)="nextSection()"
          *ngIf="currentSection() < (payload()?.lesson?.sections?.length ?? 1) - 1">
          Next →
        </button>

        <button class="btn btn-success" (click)="completeLesson()"
          *ngIf="currentSection() === (payload()?.lesson?.sections?.length ?? 1) - 1">
          Complete & Quiz! 🎯
        </button>
      </div>
    </div>

    <ng-template #loading>
      <div class="loading-screen">
        <div class="loading-mascot">📚</div>
        <p>Loading your lesson…</p>
      </div>
    </ng-template>
  `,
  styleUrls: ['./lesson-player.component.scss']
})
export class LessonPlayerComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private curriculumService = inject(CurriculumService);

  payload = signal<LessonPlayerPayload | null>(null);
  currentSection = signal(0);
  showSolutions = signal(false);
  audioEnabled = signal(true);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.curriculumService.getLessonPlayer(id).subscribe({
      next: res => { if (res.success) this.payload.set(res.data); }
    });
  }

  currentSectionData(): LessonSection | undefined {
    return this.payload()?.lesson?.sections?.[this.currentSection()];
  }

  shouldShowExamples(): boolean {
    const sections = this.payload()?.lesson?.sections ?? [];
    return this.currentSection() === sections.length - 1 && (this.payload()?.lesson?.examples?.length ?? 0) > 0;
  }

  prevSection() {
    if (this.currentSection() > 0) this.currentSection.update(n => n - 1);
  }

  nextSection() {
    const max = (this.payload()?.lesson?.sections?.length ?? 1) - 1;
    if (this.currentSection() < max) this.currentSection.update(n => n + 1);
  }

  goBack() { this.router.navigate(['/learner/dashboard']); }

  completeLesson() {
    const lessonId = this.payload()?.lesson?.id;
    this.router.navigate(['/learner/quiz', lessonId]);
  }
}
