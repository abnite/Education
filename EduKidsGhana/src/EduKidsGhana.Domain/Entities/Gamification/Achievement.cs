using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Domain.Entities.Gamification;

public class Achievement : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BadgeCode { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public BadgeCategory Category { get; set; } = BadgeCategory.General;
    public int PointsRequired { get; set; } = 0;
    public string? Criteria { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<LearnerAchievement> LearnerAchievements { get; set; } = new List<LearnerAchievement>();
}
