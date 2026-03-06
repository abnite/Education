import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LearnerService } from '../../../core/services/learner.service';
import { LearnerProfile, Avatar } from '../../../core/models/learner.models';

@Component({
  selector: 'app-learner-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="profile-page">
      <div class="page-header">
        <h1>👤 My Profile</h1>
      </div>

      <div class="profile-content" *ngIf="profile()">
        <!-- Avatar Selection -->
        <div class="avatar-section">
          <h3>Choose Your Avatar</h3>
          <div class="avatar-grid">
            <div class="avatar-option" *ngFor="let av of avatars()"
              [class.selected]="profile()?.avatarCode === av.code"
              (click)="setAvatar(av.code)">
              <span class="avatar-emoji">{{ av.code }}</span>
              <span class="avatar-name">{{ av.name }}</span>
            </div>
          </div>
        </div>

        <!-- Profile Info -->
        <div class="profile-info-card">
          <h3>Your Details</h3>
          <div class="info-row">
            <span class="info-label">Name</span>
            <span class="info-value">{{ profile()?.displayName }}</span>
          </div>
          <div class="info-row">
            <span class="info-label">Grade</span>
            <span class="info-value">Grade {{ profile()?.gradeLevel }}</span>
          </div>
          <div class="info-row">
            <span class="info-label">Age</span>
            <span class="info-value">{{ profile()?.age }} years old</span>
          </div>
          <div class="info-row">
            <span class="info-label">Total Points</span>
            <span class="info-value">⭐ {{ profile()?.totalPoints | number }}</span>
          </div>
          <div class="info-row">
            <span class="info-label">Best Streak</span>
            <span class="info-value">🔥 {{ profile()?.longestStreak }} days</span>
          </div>
        </div>

        <!-- Audio Preference -->
        <div class="preference-card">
          <h3>Learning Preferences</h3>
          <label class="toggle-row">
            <span>🔊 Audio Narration</span>
            <div class="toggle" [class.on]="audioEnabled" (click)="toggleAudio()">
              <div class="toggle-thumb"></div>
            </div>
          </label>
        </div>

        <div class="save-banner" *ngIf="saved()">✅ Avatar updated!</div>
      </div>
    </div>
  `,
  styleUrls: ['./learner-profile.component.scss']
})
export class LearnerProfileComponent implements OnInit {
  private learnerService = inject(LearnerService);

  profile  = signal<LearnerProfile | null>(null);
  avatars  = signal<Avatar[]>([]);
  saved    = signal(false);
  audioEnabled = true;

  ngOnInit() {
    this.learnerService.getProfile().subscribe({ next: r => { if (r.success) this.profile.set(r.data); } });
    this.learnerService.getAvatars().subscribe({ next: r => { if (r.success) this.avatars.set(r.data); } });
  }

  setAvatar(code: string) {
    this.learnerService.setAvatar(code).subscribe({
      next: r => {
        if (r.success) {
          this.profile.update(p => p ? { ...p, avatarCode: code } : p);
          this.saved.set(true);
          setTimeout(() => this.saved.set(false), 2500);
        }
      }
    });
  }

  toggleAudio() { this.audioEnabled = !this.audioEnabled; }
}
