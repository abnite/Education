using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Domain.Entities.Curriculum;

public class CodingExercise : BaseAuditableEntity
{
    public Guid TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string? StarterCode { get; set; }
    public string? SolutionCode { get; set; }
    public string? ExpectedOutput { get; set; }
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public int GradeLevel { get; set; }
    public int Points { get; set; } = 10;
    public bool IsActive { get; set; } = true;
    public virtual Topic Topic { get; set; } = null!;
}
