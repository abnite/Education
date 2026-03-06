export interface Subject {
  id: string;
  name: string;
  code: string;
  description?: string;
  iconUrl?: string;
  colourHex: string;
  subjectType: number;
  sortOrder: number;
  isActive: boolean;
}

export interface Topic {
  id: string;
  subjectId: string;
  gradeLevelId: string;
  name: string;
  description?: string;
  sortOrder: number;
  defaultDifficulty: number;
  isActive: boolean;
}

export interface LessonSummary {
  id: string;
  topicId: string;
  title: string;
  summary?: string;
  lessonType: number;
  difficulty: number;
  estimatedMinutes: number;
  isPublished: boolean;
}

export interface LessonDetail {
  id: string;
  topicId: string;
  topicName: string;
  subjectName: string;
  title: string;
  summary?: string;
  lessonType: number;
  difficulty: number;
  estimatedMinutes: number;
  isPublished: boolean;
  objectives: LessonObjective[];
  sections: LessonSection[];
  examples: LessonExample[];
  audioFiles: LessonAudio[];
}

export interface LessonObjective {
  id: string;
  description: string;
  sortOrder: number;
}

export interface LessonSection {
  id: string;
  title: string;
  content: string;
  contentHtml?: string;
  sortOrder: number;
  audioUrl?: string;
  hasInteraction: boolean;
}

export interface LessonExample {
  id: string;
  title: string;
  content: string;
  solution?: string;
  explanation?: string;
  sortOrder: number;
}

export interface LessonAudio {
  id: string;
  title: string;
  fileUrl?: string;
  audioType: number;
  durationSeconds?: number;
}

export interface LessonPlayerPayload {
  lessonId: string;
  title: string;
  subjectName: string;
  topicName: string;
  difficulty: number;
  estimatedMinutes: number;
  objectives: LessonObjective[];
  sections: LessonSection[];
  examples: LessonExample[];
  audioFiles: LessonAudio[];
  aiEnhancedExplanation?: string;
  hasQuiz: boolean;
  quizTemplateId?: string;
}
