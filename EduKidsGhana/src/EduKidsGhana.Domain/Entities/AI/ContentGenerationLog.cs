using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Domain.Entities.AI;

public class ContentGenerationLog : BaseAuditableEntity
{
    public string PromptUsed { get; set; } = string.Empty;
    public string? GeneratedContent { get; set; }
    public GenerationSourceType SourceType { get; set; }
    public bool IsSuccess { get; set; } = false;
    public int? TokensUsed { get; set; }
    public int DurationMs { get; set; } = 0;
    public string? ErrorMessage { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
}
