namespace EduKidsGhana.Domain.Entities.AI;

public class FallbackContentTemplate : BaseAuditableEntity
{
    public string TemplateKey { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public string ContentTemplate { get; set; } = string.Empty;
    public string? QuestionTemplate { get; set; }
    public string? HintTemplate { get; set; }
    public bool IsActive { get; set; } = true;
}
