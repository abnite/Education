using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Domain.Entities.AI;

public class AIProviderSetting : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public AIProviderType ProviderType { get; set; }
    public bool IsEnabled { get; set; } = false;
    public bool IsDefault { get; set; } = false;
    public string? ApiKey { get; set; }
    public string? ApiEndpoint { get; set; }
    public string? ModelName { get; set; }
    public int MaxTokens { get; set; } = 500;
    public double Temperature { get; set; } = 0.7;
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
}
