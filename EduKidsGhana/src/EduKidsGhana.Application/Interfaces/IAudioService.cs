using EduKidsGhana.Application.DTOs.Audio;

namespace EduKidsGhana.Application.Interfaces;

public interface IAudioService
{
    Task<List<LessonAudioDto>> GetLessonAudioAsync(Guid lessonId, CancellationToken ct = default);
    Task<string?> GetWordPronunciationUrlAsync(string word, CancellationToken ct = default);
    Task<AudioUploadResultDto> UploadAudioAsync(AudioUploadRequestDto request, CancellationToken ct = default);
}
