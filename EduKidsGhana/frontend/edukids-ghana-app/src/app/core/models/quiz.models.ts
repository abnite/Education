export interface GeneratedQuiz {
  id: string;
  title: string;
  topicId: string;
  topicName: string;
  difficulty: string;
  timeLimitSeconds: number;
  sourceType: string;
  questions: QuizQuestion[];
}

export interface QuizQuestion {
  id: string;
  questionText: string;
  audioUrl?: string;
  imageUrl?: string;
  questionType: string;
  points: number;
  sortOrder: number;
  options: QuizOption[];
}

export interface QuizOption {
  id: string;
  optionKey: string;
  optionText: string;
  imageUrl?: string;
  sortOrder: number;
}

export interface QuizSubmission {
  quizId: string;
  learnerId: string;
  durationSeconds: number;
  answers: AnswerSubmission[];
}

export interface AnswerSubmission {
  questionId: string;
  answerGiven: string;
  timeTakenSeconds: number;
  hintUsed: boolean;
}

export interface QuizResult {
  attemptId: string;
  score: number;
  totalPoints: number;
  correctAnswers: number;
  totalQuestions: number;
  percentage: number;
  isPassed: boolean;
  pointsEarned: number;
  encouragementMessage: string;
  newBadges: string[];
}

export interface QuizReview {
  attemptId: string;
  score: number;
  percentage: number;
  isPassed: boolean;
  answerReviews: AnswerReview[];
}

export interface AnswerReview {
  questionText: string;
  answerGiven: string;
  correctAnswer: string;
  isCorrect: boolean;
  explanation?: string;
  pointsEarned: number;
}
