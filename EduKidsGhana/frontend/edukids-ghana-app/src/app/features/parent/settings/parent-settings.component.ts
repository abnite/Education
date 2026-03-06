import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-parent-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="settings-page">
      <div class="page-header">
        <h1>⚙️ Settings</h1>
      </div>

      <div class="settings-card">
        <h3>Account</h3>
        <div class="info-row">
          <span class="lbl">Email</span>
          <span class="val">{{ user()?.email }}</span>
        </div>
        <div class="info-row">
          <span class="lbl">Name</span>
          <span class="val">{{ user()?.firstName }} {{ user()?.lastName }}</span>
        </div>
        <div class="info-row">
          <span class="lbl">Role</span>
          <span class="val badge">{{ user()?.roles?.[0] }}</span>
        </div>
      </div>

      <div class="settings-card">
        <h3>Notifications</h3>
        <label class="toggle-row">
          <span>📧 Weekly Progress Emails</span>
          <div class="toggle" [class.on]="emailNotifs" (click)="emailNotifs = !emailNotifs">
            <div class="thumb"></div>
          </div>
        </label>
        <label class="toggle-row">
          <span>🏆 Achievement Alerts</span>
          <div class="toggle" [class.on]="achievementAlerts" (click)="achievementAlerts = !achievementAlerts">
            <div class="thumb"></div>
          </div>
        </label>
      </div>

      <div class="settings-card danger-zone">
        <h3>Account Actions</h3>
        <button class="btn btn-secondary" (click)="logout()">🚪 Sign Out</button>
      </div>
    </div>
  `,
  styleUrls: ['./parent-settings.component.scss']
})
export class ParentSettingsComponent {
  private authService = inject(AuthService);

  user = this.authService.currentUser;
  emailNotifs = true;
  achievementAlerts = true;

  logout() { this.authService.logout(); }
}
