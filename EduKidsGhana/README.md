# 🌟 EduKids Ghana — AI-Powered Educational Platform

> **Learn, Listen, Play, and Grow** — Autonomous AI education for Ghanaian children in Grades 1–6

---

## Overview

EduKids Ghana is a production-ready, autonomous educational platform designed specifically for Ghanaian children. The system teaches, explains, generates quizzes, and adapts difficulty **without any teacher involvement** — using AI enhancement with a robust rule-based fallback.

### Key Features

| Feature | Description |
|---|---|
| 🤖 Autonomous Teaching | AI-driven lessons + rule-based fallback always available |
| 🔊 Audio-First | Lesson narration, word pronunciation, audio feedback |
| 🎯 Adaptive Learning | Mastery tracking, spaced repetition, weak area detection |
| 🏆 Gamification | Points, badges, streaks, daily challenges |
| 🇬🇭 Ghana-First Content | GH₵, Kofi/Ama/Kwame, mangoes, Accra, Kumasi |
| 👨‍👩‍👧 Parent Dashboard | Progress monitoring, weak area alerts |
| 🛡️ Admin Panel | User management, AI settings, analytics |

---

## Architecture

```
EduKidsGhana/
├── src/
│   ├── EduKidsGhana.Domain/          # Entities, Enums, Constants
│   ├── EduKidsGhana.Shared/          # ApiResponse<T>, PagedResult
│   ├── EduKidsGhana.Application/     # DTOs, Interfaces, Validators, AutoMapper
│   ├── EduKidsGhana.Infrastructure/  # EF Core, Services, AI Providers, Seeder
│   └── EduKidsGhana.API/             # Controllers, Middleware, Program.cs
└── frontend/
    └── edukids-ghana-app/            # Angular 17 standalone components
```

**Clean Architecture** — Domain → Application → Infrastructure → API

---

## Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB works)
- Node.js 18+ & npm
- Angular CLI 17+

### Backend Setup

```bash
cd EduKidsGhana

# Restore packages
dotnet restore

# Apply migrations and seed data
dotnet ef database update --project src/EduKidsGhana.Infrastructure --startup-project src/EduKidsGhana.API

# Run API (auto-seeds on first start)
dotnet run --project src/EduKidsGhana.API
# API available at: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### Frontend Setup

```bash
cd frontend/edukids-ghana-app

npm install
ng serve
# App available at: http://localhost:4200
```

---

## Demo Accounts

| Role | Email | Password |
|---|---|---|
| Admin | admin@edukidsghana.com | Admin@EduKids2024! |
| Parent | parent@demo.edukidsghana.com | Demo@EduKids2024! |

The demo parent account is linked to a learner profile: **Kofi Mensah**, Age 9, Grade 4, 120 points.

---

## Subjects & Seed Content

| Subject | Code | Grade | Sample Lesson |
|---|---|---|---|
| Mathematics | MATHS | G1 | Counting Mangoes (1–10) |
| English Language | ENGLISH | G1 | The Alphabet: A, B, C |
| Science | SCIENCE | G2 | Living and Non-Living Things |
| Coding & Logic | CODING | G3 | Sequencing & Algorithms |
| Mathematics | MATHS | G4 | Multiplication: 2s and 5s |

---

## AI Configuration

The platform has a 3-tier question generation strategy:

1. **AI Provider** (OpenAI / Anthropic / Gemini) — Ghana-contextualized prompts
2. **Rule-Based Generator** — Deterministic math (mangoes/cedis), English phonics, Science, Coding
3. **Seeded DB Content** — Always available, no network required

To configure AI: Admin Panel → AI Settings → enter API key and model name.

**The platform works 100% without AI configured** — rule-based questions are just as educational.

---

## API Endpoints

| Controller | Base Path | Key Endpoints |
|---|---|---|
| Auth | `/api/auth` | POST login, register/parent, refresh, logout |
| Learners | `/api/learners` | GET profile, dashboard; PUT avatar |
| Subjects | `/api/subjects` | GET all, CRUD (admin) |
| Lessons | `/api/lessons` | GET, POST, PUT, DELETE, publish |
| Learning | `/api/learning` | GET lesson-player, recommendation; POST session |
| Quiz | `/api/quiz` | POST generate, submit; GET review, hint |
| Progress | `/api/progress` | GET summary, masteries, achievements |
| Parents | `/api/parents` | GET dashboard, children, child detail |
| Admin | `/api/admin` | Dashboard, users, AI settings, analytics |

All responses use `ApiResponse<T>` wrapper:
```json
{
  "success": true,
  "message": "OK",
  "data": { ... },
  "errors": [],
  "correlationId": "uuid",
  "timestamp": "2026-03-06T..."
}
```

---

## Gamification

| Event | Points |
|---|---|
| Complete a lesson | +20 pts |
| Pass a quiz | +15 pts |
| Maintain daily streak | +5 pts/day |
| Earn a badge | +25 bonus pts |

### Badges
`FIRST_QUIZ` · `QUIZ_10` · `STREAK_7` · `STREAK_30` · `MASTERED_1` · `MASTERED_5` · `POINTS_100` · `POINTS_500` · `POINTS_1000` · and more

---

## Adaptive Learning

- **Mastery Levels**: NotStarted → Emerging (0–30%) → Developing (30–55%) → Proficient (55–75%) → Mastered (75%+)
- **Weighted Rolling Average**: 60% old score + 40% new score
- **Spaced Repetition**: Failed topics queued with exponential delay (hours × failCount)
- **Weak Area Detection**: Auto-detected after 3+ failed attempts
- **Lesson Recommendation**: Revision first, then next unstarted lesson by grade

---

## Technology Stack

**Backend**
- ASP.NET Core 8 Web API
- Entity Framework Core 8 + SQL Server
- ASP.NET Core Identity + JWT Bearer
- AutoMapper 13 · FluentValidation 11 · Serilog

**Frontend**
- Angular 17 (Standalone Components)
- Angular Signals (`signal()`, `computed()`)
- Functional Guards & HTTP Interceptors
- SCSS with CSS custom properties
- Google Fonts: Nunito + Fredoka One

---

*Built with ❤️ for Ghana's children*
