namespace EduKidsGhana.Domain.Entities.AI;

public class PromptTemplate : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string TemplateKey { get; set; } = string.Empty;
    public string TemplateText { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
