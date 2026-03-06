import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';

@Component({
  selector: 'app-admin-analytics',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="analytics-page">
      <div class="page-header">
        <h1>📊 Analytics</h1>
        <p>Platform-wide learning statistics</p>
      </div>

      <div class="analytics-grid" *ngIf="report()">
        <!-- Daily Activity -->
        <div class="analytics-card wide">
          <h3>Daily Activity (Last 14 Days)</h3>
          <div class="activity-chart">
            <div class="bar-group" *ngFor="let d of report()?.dailyActivity">
              <div class="activity-bar" [style.height.px]="getBarHeight(d.activeUsers)">
                <span class="bar-tooltip">{{ d.activeUsers }}</span>
              </div>
              <span class="bar-date">{{ d.date | date:'d/M' }}</span>
            </div>
          </div>
        </div>

        <!-- Key Metrics -->
        <div class="analytics-card">
          <h3>Learning Outcomes</h3>
          <div class="metric-list">
            <div class="metric-row">
              <span>Avg Quiz Pass Rate</span>
              <strong>{{ report()?.avgPassRate | number:'1.0-0' }}%</strong>
            </div>
            <div class="metric-row">
              <span>Avg Session Length</span>
              <strong>{{ report()?.avgSessionMinutes | number:'1.0-0' }} min</strong>
            </div>
            <div class="metric-row">
              <span>Topics Mastered (total)</span>
              <strong>{{ report()?.topicsMastered | number }}</strong>
            </div>
            <div class="metric-row">
              <span>Active Learners (7d)</span>
              <strong>{{ report()?.activeLearners7Days | number }}</strong>
            </div>
          </div>
        </div>

        <div class="analytics-card">
          <h3>AI Usage</h3>
          <div class="metric-list">
            <div class="metric-row">
              <span>AI Requests (30d)</span>
              <strong>{{ report()?.aiRequests30Days | number }}</strong>
            </div>
            <div class="metric-row">
              <span>Rule-Based Fallbacks</span>
              <strong>{{ report()?.ruleBasedFallbacks | number }}</strong>
            </div>
            <div class="metric-row">
              <span>Avg AI Response (ms)</span>
              <strong>{{ report()?.avgAiResponseMs | number }}</strong>
            </div>
            <div class="metric-row">
              <span>AI Success Rate</span>
              <strong>{{ report()?.aiSuccessRate | number:'1.0-0' }}%</strong>
            </div>
          </div>
        </div>
      </div>

      <div class="loading" *ngIf="!report()">📊 Loading analytics…</div>
    </div>
  `,
  styleUrls: ['./admin-analytics.component.scss']
})
export class AdminAnalyticsComponent implements OnInit {
  private http = inject(HttpClient);
  report = signal<any>(null);

  ngOnInit() {
    this.http.get<ApiResponse<any>>(`${environment.apiUrl}/admin/analytics`).subscribe({
      next: res => { if (res.success) this.report.set(res.data); }
    });
  }

  getBarHeight(value: number): number {
    const max = Math.max(...(this.report()?.dailyActivity?.map((d: any) => d.activeUsers) ?? [1]));
    return max > 0 ? Math.max(4, (value / max) * 120) : 4;
  }
}
