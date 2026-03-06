import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';

@Component({
  selector: 'app-ai-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="ai-settings-page">
      <div class="page-header">
        <h1>🤖 AI Settings</h1>
        <p>Configure AI provider for question generation and explanations</p>
      </div>

      <div class="settings-card" *ngIf="settings()">
        <div class="provider-toggle">
          <div class="provider-option" [class.selected]="settings()?.providerType === 'OpenAI'"
            (click)="setProvider('OpenAI')">
            <span class="provider-logo">🔷</span>
            <span>OpenAI (GPT)</span>
          </div>
          <div class="provider-option" [class.selected]="settings()?.providerType === 'Anthropic'"
            (click)="setProvider('Anthropic')">
            <span class="provider-logo">🟠</span>
            <span>Anthropic (Claude)</span>
          </div>
          <div class="provider-option" [class.selected]="settings()?.providerType === 'None'"
            (click)="setProvider('None')">
            <span class="provider-logo">🔧</span>
            <span>Rule-Based Only</span>
          </div>
        </div>

        <div class="form-group" *ngIf="settings()?.providerType !== 'None'">
          <label>API Key</label>
          <input type="password" [(ngModel)]="settings()!.apiKey" placeholder="sk-…" />
        </div>

        <div class="form-group" *ngIf="settings()?.providerType !== 'None'">
          <label>Model Name</label>
          <input [(ngModel)]="settings()!.modelName"
            [placeholder]="settings()?.providerType === 'OpenAI' ? 'gpt-4o-mini' : 'claude-haiku-4-5-20251001'" />
        </div>

        <div class="form-row" *ngIf="settings()?.providerType !== 'None'">
          <div class="form-group">
            <label>Max Tokens</label>
            <input type="number" [(ngModel)]="settings()!.maxTokens" />
          </div>
          <div class="form-group">
            <label>Temperature (0–1)</label>
            <input type="number" [(ngModel)]="settings()!.temperature" min="0" max="1" step="0.1" />
          </div>
        </div>

        <div class="fallback-notice">
          <span>ℹ️</span>
          <p>If AI is unavailable or not configured, the system automatically uses the built-in
          rule-based question generator. No lessons or quizzes will be interrupted.</p>
        </div>

        <div class="save-row">
          <div class="success-msg" *ngIf="saved()">✅ Settings saved!</div>
          <button class="btn btn-primary" (click)="save()" [disabled]="saving()">
            {{ saving() ? '⏳ Saving…' : 'Save Settings' }}
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./ai-settings.component.scss']
})
export class AiSettingsComponent implements OnInit {
  private http = inject(HttpClient);
  settings = signal<any>(null);
  saving = signal(false);
  saved = signal(false);

  ngOnInit() {
    this.http.get<ApiResponse<any>>(`${environment.apiUrl}/admin/ai-settings`).subscribe({
      next: res => { if (res.success) this.settings.set(res.data); }
    });
  }

  setProvider(type: string) {
    this.settings.update(s => ({ ...s, providerType: type }));
  }

  save() {
    this.saving.set(true);
    this.http.put<ApiResponse<any>>(`${environment.apiUrl}/admin/ai-settings`, this.settings()).subscribe({
      next: () => {
        this.saved.set(true);
        this.saving.set(false);
        setTimeout(() => this.saved.set(false), 3000);
      },
      error: () => this.saving.set(false)
    });
  }
}
