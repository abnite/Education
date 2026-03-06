import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

function passwordMatch(control: AbstractControl) {
  const pw = control.get('password')?.value;
  const confirm = control.get('confirmPassword')?.value;
  return pw && confirm && pw !== confirm ? { mismatch: true } : null;
}

@Component({
  selector: 'app-register',
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
          <h1>Create Parent Account</h1>
          <p>Register to start your child's learning journey</p>
        </div>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="auth-form">
          <div class="name-row">
            <div class="form-group">
              <label for="firstName">First Name</label>
              <input id="firstName" formControlName="firstName" placeholder="Kwame"
                [class.invalid]="submitted && form.get('firstName')?.invalid" />
              <span class="error" *ngIf="submitted && form.get('firstName')?.errors?.['required']">Required</span>
            </div>
            <div class="form-group">
              <label for="lastName">Last Name</label>
              <input id="lastName" formControlName="lastName" placeholder="Mensah"
                [class.invalid]="submitted && form.get('lastName')?.invalid" />
              <span class="error" *ngIf="submitted && form.get('lastName')?.errors?.['required']">Required</span>
            </div>
          </div>

          <div class="form-group">
            <label for="email">Email Address</label>
            <input id="email" type="email" formControlName="email" placeholder="you@example.com"
              [class.invalid]="submitted && form.get('email')?.invalid" />
            <span class="error" *ngIf="submitted && form.get('email')?.errors?.['required']">Email is required</span>
            <span class="error" *ngIf="submitted && form.get('email')?.errors?.['email']">Enter a valid email</span>
          </div>

          <div class="form-group">
            <label for="phone">Phone Number <span class="optional">(optional)</span></label>
            <input id="phone" formControlName="phoneNumber" placeholder="+233 24 000 0000" />
          </div>

          <div formGroupName="passwords">
            <div class="form-group">
              <label for="password">Password</label>
              <div class="input-wrapper">
                <input id="password" [type]="showPw() ? 'text' : 'password'" formControlName="password"
                  placeholder="Min. 8 characters"
                  [class.invalid]="submitted && form.get('passwords.password')?.invalid" />
                <button type="button" class="toggle-pw" (click)="showPw.set(!showPw())">
                  {{ showPw() ? '🙈' : '👁️' }}
                </button>
              </div>
              <span class="error" *ngIf="submitted && form.get('passwords.password')?.errors?.['required']">Required</span>
              <span class="error" *ngIf="submitted && form.get('passwords.password')?.errors?.['minlength']">Min 8 characters</span>
            </div>

            <div class="form-group">
              <label for="confirmPassword">Confirm Password</label>
              <input id="confirmPassword" [type]="showPw() ? 'text' : 'password'" formControlName="confirmPassword"
                placeholder="Repeat your password"
                [class.invalid]="submitted && (form.get('passwords.confirmPassword')?.invalid || form.get('passwords')?.errors?.['mismatch'])" />
              <span class="error" *ngIf="submitted && form.get('passwords')?.errors?.['mismatch']">Passwords do not match</span>
            </div>
          </div>

          <div class="form-group">
            <label for="region">Region <span class="optional">(optional)</span></label>
            <select id="region" formControlName="region">
              <option value="">-- Select region --</option>
              <option *ngFor="let r of regions" [value]="r">{{ r }}</option>
            </select>
          </div>

          <div class="error-banner" *ngIf="errorMessage()">⚠️ {{ errorMessage() }}</div>

          <button type="submit" class="btn btn-primary btn-full" [disabled]="loading()">
            <span *ngIf="!loading()">Create Account</span>
            <span *ngIf="loading()">⏳ Creating…</span>
          </button>
        </form>

        <div class="auth-footer">
          <p>Already have an account? <a routerLink="/login">Sign in</a></p>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  form: FormGroup = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: [''],
    region: [''],
    passwords: this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordMatch })
  });

  submitted = false;
  loading = signal(false);
  errorMessage = signal('');
  showPw = signal(false);

  regions = [
    'Greater Accra', 'Ashanti', 'Western', 'Eastern', 'Central',
    'Volta', 'Northern', 'Upper East', 'Upper West', 'Brong-Ahafo',
    'Bono', 'Bono East', 'Ahafo', 'Savannah', 'North East', 'Oti'
  ];

  async onSubmit() {
    this.submitted = true;
    if (this.form.invalid) return;

    this.loading.set(true);
    this.errorMessage.set('');

    const val = this.form.value;
    const payload = {
      firstName: val.firstName,
      lastName: val.lastName,
      email: val.email,
      password: val.passwords.password,
      phoneNumber: val.phoneNumber,
      region: val.region
    };

    try {
      await this.authService.registerParent(payload).toPromise();
      this.router.navigate(['/parent/dashboard']);
    } catch (err: any) {
      this.errorMessage.set(err?.error?.message || 'Registration failed. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }
}
