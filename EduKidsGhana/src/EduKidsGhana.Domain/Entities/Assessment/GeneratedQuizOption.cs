namespace EduKidsGhana.Domain.Entities.Assessment;

public class GeneratedQuizOption : BaseAuditableEntity
{
    public Guid QuestionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public string OptionKey { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; } = 0;
    public virtual GeneratedQuizQuestion Question { get; set; } = null!;
}
