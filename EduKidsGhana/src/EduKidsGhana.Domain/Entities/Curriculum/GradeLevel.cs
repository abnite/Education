namespace EduKidsGhana.Domain.Entities.Curriculum;

public class GradeLevel : BaseAuditableEntity
{
    public int Grade { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
