import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';

@Component({
  selector: 'app-admin-subjects',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="admin-subjects">
      <div class="page-header">
        <h1>📚 Subjects & Lessons</h1>
      </div>

      <div class="subjects-grid" *ngIf="subjects()?.length">
        <div class="subject-card" *ngFor="let s of subjects()">
          <div class="subject-header" [style.background]="s.colourHex || '#FF6B35'">
            <h3>{{ s.name }}</h3>
            <span class="code-badge">{{ s.code }}</span>
          </div>
          <div class="subject-body">
            <div class="stat-row">
              <span>📖 {{ s.topicCount }} topics</span>
              <span>📝 {{ s.lessonCount }} lessons</span>
            </div>
            <div class="stat-row">
              <span>🎯 {{ s.quizCount }} quizzes taken</span>
              <span>👥 {{ s.learnerCount }} learners</span>
            </div>
          </div>
        </div>
      </div>

      <div class="loading" *ngIf="!subjects()?.length">📚 Loading subjects…</div>
    </div>
  `,
  styleUrls: ['./admin-subjects.component.scss']
})
export class AdminSubjectsComponent implements OnInit {
  private http = inject(HttpClient);
  subjects = signal<any[]>([]);

  ngOnInit() {
    this.http.get<ApiResponse<any[]>>(`${environment.apiUrl}/subjects`).subscribe({
      next: res => { if (res.success) this.subjects.set(res.data); }
    });
  }
}
