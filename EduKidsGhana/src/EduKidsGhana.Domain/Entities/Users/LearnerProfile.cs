using EduKidsGhana.Domain.Entities.Identity;
using EduKidsGhana.Domain.Entities.Assessment;
using EduKidsGhana.Domain.Entities.Learning;
using EduKidsGhana.Domain.Entities.Gamification;

namespace EduKidsGhana.Domain.Entities.Users;

public class LearnerProfile : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Age { get; set; }
    public int GradeLevel { get; set; }
    public string? AvatarCode { get; set; }
    public int TotalPoints { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public DateTime? LastStudyDate { get; set; }
    public bool AudioEnabled { get; set; } = true;
    public string PreferredLanguage { get; set; } = "en";
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual LearnerPreference? Preference { get; set; }
    public virtual ICollection<ParentLearnerLink> ParentLinks { get; set; } = new List<ParentLearnerLink>();
    public virtual ICollection<ProgressRecord> ProgressRecords { get; set; } = new List<ProgressRecord>();
    public virtual ICollection<TopicMastery> TopicMasteries { get; set; } = new List<TopicMastery>();
    public virtual ICollection<LearnerAchievement> Achievements { get; set; } = new List<LearnerAchievement>();
    public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    public virtual ICollection<LearningSession> LearningSessions { get; set; } = new List<LearningSession>();
    public virtual ICollection<RewardTransaction> RewardTransactions { get; set; } = new List<RewardTransaction>();
}
