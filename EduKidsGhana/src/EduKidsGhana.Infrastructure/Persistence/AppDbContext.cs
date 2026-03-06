using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduKidsGhana.Domain.Entities.Identity;
using EduKidsGhana.Domain.Entities.Users;
using EduKidsGhana.Domain.Entities.Curriculum;
using EduKidsGhana.Domain.Entities.Assessment;
using EduKidsGhana.Domain.Entities.Learning;
using EduKidsGhana.Domain.Entities.Gamification;
using EduKidsGhana.Domain.Entities.AI;
using EduKidsGhana.Domain.Entities.System;

namespace EduKidsGhana.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Identity
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Users
    public DbSet<ParentProfile> ParentProfiles => Set<ParentProfile>();
    public DbSet<LearnerProfile> LearnerProfiles => Set<LearnerProfile>();
    public DbSet<LearnerAvatar> LearnerAvatars => Set<LearnerAvatar>();
    public DbSet<LearnerPreference> LearnerPreferences => Set<LearnerPreference>();
    public DbSet<ParentLearnerLink> ParentLearnerLinks => Set<ParentLearnerLink>();

    // Curriculum
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<GradeLevel> GradeLevels => Set<GradeLevel>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonSection> LessonSections => Set<LessonSection>();
    public DbSet<LessonObjective> LessonObjectives => Set<LessonObjective>();
    public DbSet<LessonExample> LessonExamples => Set<LessonExample>();
    public DbSet<LessonAudio> LessonAudios => Set<LessonAudio>();
    public DbSet<LessonMedia> LessonMediaFiles => Set<LessonMedia>();
    public DbSet<CodingExercise> CodingExercises => Set<CodingExercise>();

    // Assessment
    public DbSet<QuizTemplate> QuizTemplates => Set<QuizTemplate>();
    public DbSet<GeneratedQuiz> GeneratedQuizzes => Set<GeneratedQuiz>();
    public DbSet<GeneratedQuizQuestion> GeneratedQuizQuestions => Set<GeneratedQuizQuestion>();
    public DbSet<GeneratedQuizOption> GeneratedQuizOptions => Set<GeneratedQuizOption>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<LearnerAnswer> LearnerAnswers => Set<LearnerAnswer>();
    public DbSet<HintRecord> HintRecords => Set<HintRecord>();

    // Learning
    public DbSet<ProgressRecord> ProgressRecords => Set<ProgressRecord>();
    public DbSet<TopicMastery> TopicMasteries => Set<TopicMastery>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();
    public DbSet<RevisionQueueItem> RevisionQueueItems => Set<RevisionQueueItem>();
    public DbSet<LearningSession> LearningSessions => Set<LearningSession>();
    public DbSet<WeakAreaRecord> WeakAreaRecords => Set<WeakAreaRecord>();

    // Gamification
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<LearnerAchievement> LearnerAchievements => Set<LearnerAchievement>();
    public DbSet<DailyChallenge> DailyChallenges => Set<DailyChallenge>();
    public DbSet<RewardTransaction> RewardTransactions => Set<RewardTransaction>();

    // AI
    public DbSet<AIProviderSetting> AIProviderSettings => Set<AIProviderSetting>();
    public DbSet<ContentGenerationLog> ContentGenerationLogs => Set<ContentGenerationLog>();
    public DbSet<PromptTemplate> PromptTemplates => Set<PromptTemplate>();
    public DbSet<FallbackContentTemplate> FallbackContentTemplates => Set<FallbackContentTemplate>();

    // System
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filters for soft delete
        builder.Entity<Subject>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Topic>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Lesson>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LearnerProfile>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ParentProfile>().HasQueryFilter(e => !e.IsDeleted);
    }
}
