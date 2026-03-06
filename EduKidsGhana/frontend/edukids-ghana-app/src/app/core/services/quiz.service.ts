import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import { GeneratedQuiz, QuizResult, QuizReview, QuizSubmission } from '../models/quiz.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class QuizService {
  private base = `${environment.apiUrl}/quiz`;

  constructor(private http: HttpClient) {}

  generateQuiz(learnerId: string, topicId: string, questionCount = 5): Observable<ApiResponse<GeneratedQuiz>> {
    return this.http.post<ApiResponse<GeneratedQuiz>>(`${this.base}/generate`, {
      learnerId, topicId, questionCount, useAI: true, quizType: 1
    });
  }

  submitQuiz(submission: QuizSubmission): Observable<ApiResponse<QuizResult>> {
    return this.http.post<ApiResponse<QuizResult>>(`${this.base}/submit`, submission);
  }

  getReview(attemptId: string): Observable<ApiResponse<QuizReview>> {
    return this.http.get<ApiResponse<QuizReview>>(`${this.base}/review/${attemptId}`);
  }

  getHint(questionId: string, learnerId: string): Observable<ApiResponse<{ hintText: string; sourceType: string }>> {
    return this.http.post<ApiResponse<{ hintText: string; sourceType: string }>>(`${this.base}/hint`, { questionId, learnerId });
  }
}
