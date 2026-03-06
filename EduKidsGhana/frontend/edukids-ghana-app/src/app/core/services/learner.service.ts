import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { LearnerProfile, LearnerDashboard, Avatar, CreateLearnerRequest } from '../models/learner.models';

@Injectable({ providedIn: 'root' })
export class LearnerService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/learners`;

  getProfile(): Observable<ApiResponse<LearnerProfile>> {
    return this.http.get<ApiResponse<LearnerProfile>>(`${this.base}/profile`);
  }

  getDashboard(): Observable<ApiResponse<LearnerDashboard>> {
    return this.http.get<ApiResponse<LearnerDashboard>>(`${this.base}/dashboard`);
  }

  createLearner(dto: CreateLearnerRequest): Observable<ApiResponse<LearnerProfile>> {
    return this.http.post<ApiResponse<LearnerProfile>>(this.base, dto);
  }

  updateProfile(dto: Partial<LearnerProfile>): Observable<ApiResponse<LearnerProfile>> {
    return this.http.put<ApiResponse<LearnerProfile>>(`${this.base}/profile`, dto);
  }

  setAvatar(avatarCode: string): Observable<ApiResponse<LearnerProfile>> {
    return this.http.put<ApiResponse<LearnerProfile>>(`${this.base}/avatar`, { avatarCode });
  }

  getAvatars(): Observable<ApiResponse<Avatar[]>> {
    return this.http.get<ApiResponse<Avatar[]>>(`${this.base}/avatars`);
  }

  getRevisionQueue(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${environment.apiUrl}/learning/revision-queue`);
  }

  getAchievements(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${environment.apiUrl}/progress/achievements`);
  }

  getProgressSummary(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${environment.apiUrl}/progress/summary`);
  }
}
