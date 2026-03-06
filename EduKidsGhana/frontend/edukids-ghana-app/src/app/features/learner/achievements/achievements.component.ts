import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LearnerService } from '../../../core/services/learner.service';

@Component({
  selector: 'app-achievements',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="achievements-page">
      <div class="page-header">
        <h1>🏆 Achievements</h1>
        <p>{{ earned() }} of {{ total() }} badges earned</p>
        <div class="progress-bar">
          <div class="progress-fill" [style.width.%]="earnedPercent()"></div>
        </div>
      </div>

      <div class="achievements-grid">
        <div class="achievement-card" *ngFor="let a of achievements()"
          [class.earned]="a.isEarned"
          [class.locked]="!a.isEarned">
          <div class="badge-icon">{{ a.icon || '🏅' }}</div>
          <h4>{{ a.name }}</h4>
          <p>{{ a.description }}</p>
          <div class="earned-info" *ngIf="a.isEarned">
            <span class="earned-label">✓ Earned {{ a.earnedAt | date:'mediumDate' }}</span>
          </div>
          <div class="locked-info" *ngIf="!a.isEarned">
            <span class="locked-label">🔒 Locked</span>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./achievements.component.scss']
})
export class AchievementsComponent implements OnInit {
  private learnerService = inject(LearnerService);

  achievements = signal<any[]>([]);

  earned = () => this.achievements().filter(a => a.isEarned).length;
  total  = () => this.achievements().length;
  earnedPercent = () => this.total() ? (this.earned() / this.total()) * 100 : 0;

  ngOnInit() {
    this.learnerService.getAchievements().subscribe({
      next: res => { if (res.success) this.achievements.set(res.data); }
    });
  }
}
