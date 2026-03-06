namespace EduKidsGhana.Domain.Entities.System;

public class AppSetting : BaseAuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Group { get; set; }
    public bool IsPublic { get; set; } = false;
}
