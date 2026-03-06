import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LearnerService } from '../../../core/services/learner.service';

@Component({
  selector: 'app-add-child',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="add-child-page">
      <div class="page-header">
        <a routerLink="/parent/children" class="back-link">← Back</a>
        <h1>➕ Add Child</h1>
      </div>

      <div class="form-card">
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label>Child's Name</label>
            <input formControlName="displayName" placeholder="e.g. Kofi" [class.invalid]="sub && form.get('displayName')?.invalid" />
            <span class="error" *ngIf="sub && form.get('displayName')?.errors?.['required']">Name is required</span>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Age</label>
              <input type="number" formControlName="age" placeholder="7" min="4" max="14"
                [class.invalid]="sub && form.get('age')?.invalid" />
              <span class="error" *ngIf="sub && form.get('age')?.errors?.['required']">Required</span>
            </div>
            <div class="form-group">
              <label>Grade Level</label>
              <select formControlName="gradeLevel" [class.invalid]="sub && form.get('gradeLevel')?.invalid">
                <option value="">-- Grade --</option>
                <option *ngFor="let g of grades" [value]="g">Grade {{ g }}</option>
              </select>
              <span class="error" *ngIf="sub && form.get('gradeLevel')?.errors?.['required']">Required</span>
            </div>
          </div>

          <div class="error-banner" *ngIf="errorMsg()">⚠️ {{ errorMsg() }}</div>
          <div class="success-banner" *ngIf="success()">✅ Child added successfully! Redirecting…</div>

          <div class="form-actions">
            <a routerLink="/parent/children" class="btn btn-secondary">Cancel</a>
            <button type="submit" class="btn btn-primary" [disabled]="loading()">
              {{ loading() ? '⏳ Adding…' : 'Add Child' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styleUrls: ['./add-child.component.scss']
})
export class AddChildComponent {
  private fb = inject(FormBuilder);
  private learnerService = inject(LearnerService);
  private router = inject(Router);

  grades = [1, 2, 3, 4, 5, 6];
  sub = false;
  loading = signal(false);
  errorMsg = signal('');
  success = signal(false);

  form = this.fb.group({
    displayName: ['', Validators.required],
    age: [null, [Validators.required, Validators.min(4), Validators.max(14)]],
    gradeLevel: ['', Validators.required]
  });

  onSubmit() {
    this.sub = true;
    if (this.form.invalid) return;
    this.loading.set(true);
    this.learnerService.createLearner(this.form.value as any).subscribe({
      next: res => {
        if (res.success) {
          this.success.set(true);
          setTimeout(() => this.router.navigate(['/parent/children']), 1500);
        } else {
          this.errorMsg.set(res.message);
        }
        this.loading.set(false);
      },
      error: err => {
        this.errorMsg.set(err?.error?.message || 'Failed to add child.');
        this.loading.set(false);
      }
    });
  }
}
