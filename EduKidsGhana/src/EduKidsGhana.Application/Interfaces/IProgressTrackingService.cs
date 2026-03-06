using EduKidsGhana.Application.DTOs.Progress;

namespace EduKidsGhana.Application.Interfaces;

public interface IProgressTrackingService
{
    Task<LearnerProgressSummaryDto> GetProgressSummaryAsync(Guid learnerId, CancellationToken ct = default);
    Task<List<TopicMasteryDto>> GetTopicMasteriesAsync(Guid learnerId, CancellationToken ct = default);
    Task RecordLessonCompletionAsync(Guid learnerId, Guid lessonId, int minutesSpent, CancellationToken ct = default);
    Task RecordQuizResultAsync(Guid learnerId, Guid topicId, double scorePercent, int pointsEarned, CancellationToken ct = default);
    Task UpdateStreakAsync(Guid learnerId, CancellationToken ct = default);
}
