import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import { LessonDetail, LessonPlayerPayload, LessonSummary, Subject, Topic } from '../models/curriculum.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CurriculumService {
  constructor(private http: HttpClient) {}

  getSubjects(): Observable<ApiResponse<Subject[]>> {
    return this.http.get<ApiResponse<Subject[]>>(`${environment.apiUrl}/subjects`);
  }

  getTopics(subjectId: string, gradeLevel: number): Observable<ApiResponse<Topic[]>> {
    return this.http.get<ApiResponse<Topic[]>>(`${environment.apiUrl}/subjects/${subjectId}/topics?gradeLevel=${gradeLevel}`);
  }

  getLessonsByTopic(topicId: string): Observable<ApiResponse<LessonSummary[]>> {
    return this.http.get<ApiResponse<LessonSummary[]>>(`${environment.apiUrl}/lessons/by-topic/${topicId}`);
  }

  getLessonDetail(lessonId: string): Observable<ApiResponse<LessonDetail>> {
    return this.http.get<ApiResponse<LessonDetail>>(`${environment.apiUrl}/lessons/${lessonId}`);
  }

  getLessonPlayer(learnerId: string, lessonId: string): Observable<ApiResponse<LessonPlayerPayload>> {
    return this.http.get<ApiResponse<LessonPlayerPayload>>(`${environment.apiUrl}/learning/lesson-player/${learnerId}/${lessonId}`);
  }
}
