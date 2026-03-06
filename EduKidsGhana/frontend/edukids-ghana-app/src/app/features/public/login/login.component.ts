import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-page">
      <div class="auth-card">
        <div class="auth-header">
          <a routerLink="/" class="back-link">← Back</a>
          <div class="auth-logo">
            <span class="logo-icon">🌟</span>
            <span class="logo-text">EduKids Ghana</span>
          </div>
          <h1>Welcome Back!</h1>
          <p>Sign in to continue your learning journey</p>
        </div>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label for="email">Email Address</label>
            <input
              id="email"
              type="email"
              formControlName="email"
              placeholder="parent@example.com"
              [class.invalid]="submitted && form.get('email')?.invalid"
            />
            <span class="error" *ngIf="submitted && form.get('email')?.errors?.['required']">Email is required</span>
            <span class="error" *ngIf="submitted && form.get('email')?.errors?.['email']">Enter a valid email</span>
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <div class="input-wrapper">
              <input
                id="password"
                [type]="showPassword() ? 'text' : 'password'"
                formControlName="password"
                placeholder="Your password"
                [class.invalid]="submitted && form.get('password')?.invalid"
              />
              <button type="button" class="toggle-pw" (click)="showPassword.set(!showPassword())">
                {{ showPassword() ? '🙈' : '👁️' }}
              </button>
            </div>
            <span class="error" *ngIf="submitted && form.get('password')?.errors?.['required']">Password is required</span>
          </div>

          <div class="error-banner" *ngIf="errorMessage()">
            ⚠️ {{ errorMessage() }}
          </div>

          <button type="submit" class="btn btn-primary btn-full" [disabled]="loading()">
            <span *ngIf="!loading()">Sign In</span>
            <span *ngIf="loading()" class="spinner">⏳ Signing in…</span>
          </button>
        </form>

        <div class="auth-footer">
          <p>Don't have an account? <a routerLink="/register">Register here</a></p>
          <div class="demo-hint">
            <strong>Demo:</strong> parent&#64;demo.edukidsghana.com / Demo@EduKids2024!
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  submitted = false;
  loading = signal(false);
  errorMessage = signal('');
  showPassword = signal(false);

  async onSubmit() {
    this.submitted = true;
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    try {
      await this.authService.login(this.form.value).toPromise();
      const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
      const user = this.authService.currentUser();
      if (user?.roles?.includes('Admin')) {
        this.router.navigate(['/admin/dashboard']);
      } else if (user?.roles?.includes('Parent')) {
        this.router.navigate([returnUrl || '/parent/dashboard']);
      } else {
        this.router.navigate([returnUrl || '/learner/dashboard']);
      }
    } catch (err: any) {
      this.errorMessage.set(err?.error?.message || 'Invalid email or password. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }
}
