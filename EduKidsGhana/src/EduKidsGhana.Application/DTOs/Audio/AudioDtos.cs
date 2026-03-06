using EduKidsGhana.Domain.Enums;

namespace EduKidsGhana.Application.DTOs.Audio;

public class LessonAudioDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public AudioType AudioType { get; set; }
    public int? DurationSeconds { get; set; }
}

public class AudioUploadRequestDto
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public AudioType AudioType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
}

public class AudioUploadResultDto
{
    public Guid Id { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
}
