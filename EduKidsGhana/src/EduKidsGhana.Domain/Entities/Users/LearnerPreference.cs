namespace EduKidsGhana.Domain.Entities.Users;

public class LearnerPreference : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public bool AudioNarrationEnabled { get; set; } = true;
    public bool AnimationsEnabled { get; set; } = true;
    public int DailyGoalMinutes { get; set; } = 30;
    public string PreferredDifficulty { get; set; } = "Auto";
    public string FontSizePreference { get; set; } = "Medium";
    public virtual LearnerProfile Learner { get; set; } = null!;
}
