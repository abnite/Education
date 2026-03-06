using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Domain.Entities.Curriculum;

public class Subject : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string ColourHex { get; set; } = "#4CAF50";
    public SubjectType SubjectType { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
