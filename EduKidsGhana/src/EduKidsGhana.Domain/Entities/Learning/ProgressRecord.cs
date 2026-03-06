using EduKidsGhana.Domain.Entities.Users;
using EduKidsGhana.Domain.Entities.Curriculum;

namespace EduKidsGhana.Domain.Entities.Learning;

public class ProgressRecord : BaseAuditableEntity
{
    public Guid LearnerId { get; set; }
    public Guid TopicId { get; set; }
    public Guid? LessonId { get; set; }
    public int LessonsCompleted { get; set; } = 0;
    public int QuizzesCompleted { get; set; } = 0;
    public int TotalPointsEarned { get; set; } = 0;
    public double AverageScore { get; set; } = 0;
    public int TotalMinutesSpent { get; set; } = 0;
    public DateTime? LastActivityAt { get; set; }
    public virtual LearnerProfile Learner { get; set; } = null!;
    public virtual Topic Topic { get; set; } = null!;
}
