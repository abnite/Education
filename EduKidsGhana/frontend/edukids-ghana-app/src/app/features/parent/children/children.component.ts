import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';

@Component({
  selector: 'app-children',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="children-page">
      <div class="page-header">
        <h1>👧🏾 My Children</h1>
        <a routerLink="/parent/add-child" class="btn btn-primary">+ Add Child</a>
      </div>

      <div class="children-list" *ngIf="children()?.length; else empty">
        <div class="child-row" *ngFor="let c of children()">
          <div class="child-avatar-big">{{ c.avatarCode || '🧒🏾' }}</div>
          <div class="child-details">
            <h3>{{ c.displayName }}</h3>
            <div class="child-meta">
              <span>Grade {{ c.gradeLevel }}</span>
              <span>Age {{ c.age }}</span>
              <span>🔥 {{ c.currentStreak }}d streak</span>
              <span>⭐ {{ c.totalPoints | number }} pts</span>
            </div>
          </div>
          <button class="btn btn-primary" (click)="router.navigate(['/parent/child', c.learnerId])">
            View Progress →
          </button>
        </div>
      </div>

      <ng-template #empty>
        <div class="empty-state">
          <span>👧🏾</span>
          <h3>No children yet</h3>
          <p>Add your child's profile to start tracking their progress.</p>
          <a routerLink="/parent/add-child" class="btn btn-primary btn-lg">Add First Child</a>
        </div>
      </ng-template>
    </div>
  `,
  styleUrls: ['./children.component.scss']
})
export class ChildrenComponent implements OnInit {
  private http = inject(HttpClient);
  router = inject(Router);
  children = signal<any[]>([]);

  ngOnInit() {
    this.http.get<ApiResponse<any[]>>(`${environment.apiUrl}/parents/children`).subscribe({
      next: res => { if (res.success) this.children.set(res.data); }
    });
  }
}
