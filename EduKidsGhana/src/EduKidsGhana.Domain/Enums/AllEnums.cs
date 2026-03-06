namespace EduKidsGhana.Domain.Enums;

public enum SubjectType { Mathematics = 1, English = 2, Science = 3, Programming = 4 }
public enum DifficultyLevel { Beginner = 1, Elementary = 2, Intermediate = 3, Advanced = 4, Challenge = 5 }
public enum LessonType { Core = 1, Revision = 2, Practice = 3, Extension = 4, CodingChallenge = 5 }
public enum QuizType { Practice = 1, Formal = 2, Revision = 3, Challenge = 4 }
public enum QuestionType { MultipleChoice = 1, TrueFalse = 2, FillBlank = 3, MatchItems = 4, SequenceOrder = 5, ImageBased = 6, AudioBased = 7, CodeBlock = 8 }
public enum AudioType { LessonNarration = 1, WordPronunciation = 2, QuestionNarration = 3, FeedbackCorrect = 4, FeedbackIncorrect = 5 }
public enum LearningMode { Guided = 1, Practice = 2, Challenge = 3, Revision = 4 }
public enum MasteryLevel { NotStarted = 0, Emerging = 1, Developing = 2, Proficient = 3, Mastered = 4 }
public enum RecommendationType { NewLesson = 1, Revision = 2, Challenge = 3, WeakAreaPractice = 4, DailyGoal = 5 }
public enum ChallengeType { DailyQuiz = 1, SpeedChallenge = 2, MasteryChallenge = 3, TopicCompletion = 4, StreakGoal = 5 }
public enum RewardType { QuizCompletion = 1, LessonCompletion = 2, StreakBonus = 3, AchievementUnlocked = 4, DailyChallengeBonus = 5, WeeklyBonus = 6, ImprovementBonus = 7 }
public enum AIProviderType { None = 0, OpenAI = 1, Anthropic = 2, GoogleGemini = 3, LocalLLM = 4 }
public enum GenerationSourceType { AIGenerated = 1, RuleBased = 2, TemplateBased = 3, Seeded = 4 }
public enum SessionStatus { Active = 1, Completed = 2, Abandoned = 3, Paused = 4 }
public enum BadgeCategory { General = 1, Mathematics = 2, English = 3, Science = 4, Programming = 5, Streak = 6, Improvement = 7, Speed = 8 }
