import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { QuizService } from '../../../core/services/quiz.service';
import { GeneratedQuiz, QuizQuestion, QuizSubmission, AnswerSubmission, QuizResult } from '../../../core/models/quiz.models';

@Component({
  selector: 'app-quiz-player',
  standalone: true,
  imports: [CommonModule],
  template: `
    <!-- Loading -->
    <div class="loading-screen" *ngIf="loading()">
      <span class="loading-icon">🎯</span>
      <p>Preparing your quiz…</p>
    </div>

    <!-- Quiz in progress -->
    <div class="quiz-container" *ngIf="!loading() && !result() && quiz()">
      <div class="quiz-header">
        <div class="quiz-info">
          <h2>🎯 Quiz Time!</h2>
          <span class="question-count">{{ currentIndex() + 1 }} of {{ quiz()?.questions?.length }}</span>
        </div>
        <div class="quiz-timer" *ngIf="timeLeft() > 0">
          ⏱ {{ formatTime(timeLeft()) }}
        </div>
      </div>

      <!-- Progress bar -->
      <div class="quiz-progress-bar">
        <div class="quiz-progress-fill" [style.width.%]="progressPercent()"></div>
      </div>

      <!-- Question -->
      <div class="question-card" *ngIf="currentQuestion()">
        <div class="question-points">+{{ currentQuestion()?.points }} pts</div>
        <p class="question-text">{{ currentQuestion()?.questionText }}</p>

        <div class="options-grid">
          <button
            class="option-btn"
            *ngFor="let opt of currentQuestion()?.options"
            [class.selected]="selectedAnswer() === opt.optionKey"
            [class.correct]="answered() && opt.isCorrect"
            [class.wrong]="answered() && selectedAnswer() === opt.optionKey && !opt.isCorrect"
            [disabled]="answered()"
            (click)="selectAnswer(opt.optionKey)">
            <span class="option-key">{{ opt.optionKey }}</span>
            <span class="option-text">{{ opt.optionText }}</span>
          </button>
        </div>

        <!-- Feedback after answering -->
        <div class="feedback-card correct-fb" *ngIf="answered() && lastCorrect()">
          ✅ Correct! Well done, {{ greetingName() }}!
        </div>
        <div class="feedback-card wrong-fb" *ngIf="answered() && !lastCorrect()">
          ❌ Not quite. Keep going!
          <p class="hint-text" *ngIf="currentQuestion()?.explanation">{{ currentQuestion()?.explanation }}</p>
        </div>

        <!-- Hint -->
        <div class="hint-section" *ngIf="!answered()">
          <button class="btn btn-secondary btn-sm hint-btn" (click)="getHint()" [disabled]="hintLoading()">
            💡 {{ hintLoading() ? 'Getting hint…' : 'Hint' }}
          </button>
          <p class="hint-text" *ngIf="hintText()">{{ hintText() }}</p>
        </div>
      </div>

      <!-- Navigation -->
      <div class="quiz-nav">
        <button class="btn btn-primary btn-lg" *ngIf="answered() && currentIndex() < (quiz()?.questions?.length ?? 1) - 1"
          (click)="nextQuestion()">
          Next Question →
        </button>
        <button class="btn btn-success btn-lg" *ngIf="answered() && currentIndex() === (quiz()?.questions?.length ?? 1) - 1"
          (click)="submitQuiz()">
          Submit Quiz ✅
        </button>
      </div>
    </div>

    <!-- Result Screen -->
    <div class="result-screen" *ngIf="result()">
      <div class="result-card" [class.passed]="result()?.isPassed" [class.failed]="!result()?.isPassed">
        <div class="result-emoji">{{ result()?.isPassed ? '🎉' : '💪' }}</div>
        <h2>{{ result()?.message }}</h2>
        <div class="result-score">
          <span class="score-number">{{ result()?.score | number:'1.0-0' }}</span>
          <span class="score-label">%</span>
        </div>
        <p class="result-detail">{{ result()?.correctAnswers }} / {{ result()?.totalQuestions }} correct</p>
        <div class="result-points" *ngIf="result()?.pointsEarned">
          ⭐ +{{ result()?.pointsEarned }} points earned!
        </div>
        <div class="result-badges" *ngIf="result()?.newBadges?.length">
          <h3>🏆 New Badges!</h3>
          <div class="badges-row">
            <span class="badge-chip" *ngFor="let b of result()?.newBadges">{{ b }}</span>
          </div>
        </div>
        <div class="result-actions">
          <button class="btn btn-primary btn-lg" (click)="goToDashboard()">🏠 Dashboard</button>
          <button class="btn btn-secondary" (click)="retryQuiz()" *ngIf="!result()?.isPassed">🔄 Try Again</button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./quiz-player.component.scss']
})
export class QuizPlayerComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private quizService = inject(QuizService);

  quiz = signal<GeneratedQuiz | null>(null);
  result = signal<QuizResult | null>(null);
  loading = signal(true);
  currentIndex = signal(0);
  selectedAnswer = signal<string>('');
  answered = signal(false);
  lastCorrect = signal(false);
  hintText = signal('');
  hintLoading = signal(false);
  timeLeft = signal(0);

  answers: AnswerSubmission[] = [];
  private lessonId = '';
  private timerInterval?: ReturnType<typeof setInterval>;

  progressPercent = computed(() => {
    const total = this.quiz()?.questions?.length ?? 1;
    return ((this.currentIndex() + 1) / total) * 100;
  });

  currentQuestion = computed(() => this.quiz()?.questions?.[this.currentIndex()]);

  greetingName = signal('learner');

  ngOnInit() {
    this.lessonId = this.route.snapshot.paramMap.get('id')!;
    this.quizService.generateQuiz({ lessonId: this.lessonId }).subscribe({
      next: res => {
        if (res.success) {
          this.quiz.set(res.data);
          if (res.data.timeLimitSeconds) {
            this.timeLeft.set(res.data.timeLimitSeconds);
            this.startTimer();
          }
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  startTimer() {
    this.timerInterval = setInterval(() => {
      this.timeLeft.update(t => {
        if (t <= 1) { clearInterval(this.timerInterval); this.submitQuiz(); return 0; }
        return t - 1;
      });
    }, 1000);
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  selectAnswer(key: string) {
    if (this.answered()) return;
    this.selectedAnswer.set(key);
    this.answered.set(true);
    const correct = this.currentQuestion()?.options?.find(o => o.optionKey === key)?.isCorrect ?? false;
    this.lastCorrect.set(correct);

    this.answers.push({
      questionId: this.currentQuestion()!.id,
      answerGiven: key,
      timeTakenSeconds: 0,
      hintUsed: !!this.hintText()
    });
  }

  nextQuestion() {
    this.currentIndex.update(n => n + 1);
    this.selectedAnswer.set('');
    this.answered.set(false);
    this.lastCorrect.set(false);
    this.hintText.set('');
  }

  getHint() {
    const qId = this.currentQuestion()?.id;
    if (!qId) return;
    this.hintLoading.set(true);
    this.quizService.getHint({ questionId: qId, learnerId: '' }).subscribe({
      next: res => { if (res.success) this.hintText.set(res.data.hint); this.hintLoading.set(false); },
      error: () => this.hintLoading.set(false)
    });
  }

  submitQuiz() {
    clearInterval(this.timerInterval);
    const sub: QuizSubmission = { quizId: this.quiz()!.id, answers: this.answers };
    this.quizService.submitQuiz(sub).subscribe({
      next: res => { if (res.success) this.result.set(res.data); }
    });
  }

  goToDashboard() { this.router.navigate(['/learner/dashboard']); }
  retryQuiz() { this.router.navigate(['/learner/quiz', this.lessonId]); }
}
