import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterLink, CommonModule],
  template: `
    <div class="landing">
      <!-- Hero Section -->
      <header class="hero">
        <nav class="nav container">
          <div class="logo">
            <span class="logo-icon">🌟</span>
            <span class="logo-text">EduKids Ghana</span>
          </div>
          <div class="nav-links">
            <a routerLink="/login" class="btn btn-secondary">Login</a>
            <a routerLink="/register" class="btn btn-primary">Get Started</a>
          </div>
        </nav>
        <div class="hero-content container">
          <div class="hero-text">
            <h1 class="hero-title">Learn, Listen, Play, and Grow! 🌍</h1>
            <p class="hero-subtitle">
              The smart learning companion for Ghanaian children.
              Mathematics, English, Science and Coding — taught by an intelligent digital tutor that never stops teaching.
            </p>
            <div class="hero-buttons">
              <a routerLink="/register" class="btn btn-primary btn-xl">Start Learning Free!</a>
              <a routerLink="/login" class="btn btn-accent btn-xl">I Already Have an Account</a>
            </div>
            <div class="hero-badges">
              <span class="badge badge-success">✓ No Teacher Required</span>
              <span class="badge badge-info">✓ Grades 1-6</span>
              <span class="badge badge-warning">✓ Audio-First Learning</span>
            </div>
          </div>
          <div class="hero-illustration animate-float">
            <div class="mascot">🧒🏾</div>
            <div class="floating-elements">
              <span class="float-el" style="top: 10%; left: 80%">📐</span>
              <span class="float-el" style="top: 60%; left: 85%">🔬</span>
              <span class="float-el" style="top: 80%; left: 70%">💻</span>
              <span class="float-el" style="top: 30%; left: 5%">📚</span>
            </div>
          </div>
        </div>
      </header>

      <!-- Subjects Section -->
      <section class="subjects-section container">
        <h2 class="section-title text-center">What Will You Learn Today?</h2>
        <div class="subjects-grid">
          <div class="subject-card subject-maths" *ngFor="let subject of subjects">
            <div class="subject-icon" [innerHTML]="subject.icon"></div>
            <h3>{{ subject.name }}</h3>
            <p>{{ subject.description }}</p>
          </div>
        </div>
      </section>

      <!-- How It Works -->
      <section class="how-section">
        <div class="container">
          <h2 class="section-title text-center">How EduKids Ghana Works</h2>
          <div class="steps-grid">
            <div class="step" *ngFor="let step of steps; let i = index">
              <div class="step-number">{{ i + 1 }}</div>
              <div class="step-icon">{{ step.icon }}</div>
              <h3>{{ step.title }}</h3>
              <p>{{ step.description }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Features -->
      <section class="features-section container">
        <h2 class="section-title text-center">Built for Ghanaian Children</h2>
        <div class="features-grid">
          <div class="feature-card" *ngFor="let feature of features">
            <div class="feature-icon">{{ feature.icon }}</div>
            <h3>{{ feature.title }}</h3>
            <p>{{ feature.description }}</p>
          </div>
        </div>
      </section>

      <!-- CTA -->
      <section class="cta-section">
        <div class="container text-center">
          <h2>Ready to Start Learning?</h2>
          <p>Join thousands of children across Ghana who are learning every day!</p>
          <a routerLink="/register" class="btn btn-primary btn-xl">Create Free Account</a>
        </div>
      </section>

      <!-- Footer -->
      <footer class="footer">
        <div class="container">
          <p>🌟 EduKids Ghana — Learn, Listen, Play, and Grow</p>
          <p class="text-muted">Empowering children across Ghana through intelligent, autonomous learning.</p>
        </div>
      </footer>
    </div>
  `,
  styleUrls: ['./landing.component.scss']
})
export class LandingComponent {
  subjects = [
    { name: 'Mathematics', icon: '🔢', description: 'Numbers, shapes, money and problem solving with Ghanaian examples', colour: '#FF6B35' },
    { name: 'English', icon: '📖', description: 'Alphabet, reading, writing and comprehension in British English', colour: '#4ECDC4' },
    { name: 'Science', icon: '🔬', description: 'Explore living things, nature, and the world around you in Ghana', colour: '#45B7D1' },
    { name: 'Coding', icon: '💻', description: 'Learn to think like a computer with fun puzzles and sequences', colour: '#96CEB4' }
  ];

  steps = [
    { icon: '👤', title: 'Create Your Profile', description: 'Parents register and create a learner profile for their child in minutes.' },
    { icon: '🎯', title: 'The AI Tutor Guides You', description: 'Our intelligent system picks the perfect lesson for your child\'s level.' },
    { icon: '📚', title: 'Learn and Listen', description: 'Children learn through colourful lessons, examples and audio narration.' },
    { icon: '🎮', title: 'Play and Practise', description: 'Fun quizzes and challenges test knowledge and earn stars and badges.' },
    { icon: '📈', title: 'Track Progress', description: 'Parents see detailed progress, strengths and weak areas in real time.' }
  ];

  features = [
    { icon: '🇬🇭', title: 'Ghana-First Content', description: 'Examples use cedis, mangoes, Accra, Kumasi and familiar Ghanaian context.' },
    { icon: '🔊', title: 'Audio-First Learning', description: 'Every lesson can be heard. Perfect for early readers and audio learners.' },
    { icon: '🤖', title: 'AI-Powered or Offline', description: 'Smart AI enhancement when available, or reliable rule-based learning always.' },
    { icon: '🏆', title: 'Gamified Progress', description: 'Points, badges, streaks and daily challenges keep children motivated.' },
    { icon: '📊', title: 'Adaptive Difficulty', description: 'Content adjusts to each child\'s mastery level automatically.' },
    { icon: '👨‍👩‍👧', title: 'Parent Dashboard', description: 'Full visibility into your child\'s learning with simple, clear reports.' }
  ];
}
