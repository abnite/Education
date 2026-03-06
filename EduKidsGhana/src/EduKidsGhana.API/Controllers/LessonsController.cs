using EduKidsGhana.Application.DTOs.Curriculum;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService) => _lessonService = lessonService;

    [HttpGet("{lessonId:guid}")]
    public async Task<ActionResult<ApiResponse<LessonDetailDto>>> GetLesson(Guid lessonId, CancellationToken ct)
    {
        var lesson = await _lessonService.GetLessonAsync(lessonId, ct);
        if (lesson == null) return NotFound(ApiResponse<LessonDetailDto>.Fail("Lesson not found."));
        return Ok(ApiResponse<LessonDetailDto>.Ok(lesson));
    }

    [HttpGet("by-topic/{topicId:guid}")]
    public async Task<ActionResult<ApiResponse<List<LessonSummaryDto>>>> GetByTopic(Guid topicId, CancellationToken ct) =>
        Ok(ApiResponse<List<LessonSummaryDto>>.Ok(await _lessonService.GetLessonsByTopicAsync(topicId, ct)));

    [HttpPost]
    [Authorize(Roles = "Admin,ContentManager")]
    public async Task<ActionResult<ApiResponse<LessonDetailDto>>> Create([FromBody] CreateLessonDto dto, CancellationToken ct)
    {
        var lesson = await _lessonService.CreateLessonAsync(dto, ct);
        return CreatedAtAction(nameof(GetLesson), new { lessonId = lesson.Id }, ApiResponse<LessonDetailDto>.Ok(lesson));
    }

    [HttpPut("{lessonId:guid}")]
    [Authorize(Roles = "Admin,ContentManager")]
    public async Task<ActionResult<ApiResponse<LessonDetailDto>>> Update(Guid lessonId, [FromBody] CreateLessonDto dto, CancellationToken ct) =>
        Ok(ApiResponse<LessonDetailDto>.Ok(await _lessonService.UpdateLessonAsync(lessonId, dto, ct)));

    [HttpPost("{lessonId:guid}/publish")]
    [Authorize(Roles = "Admin,ContentManager")]
    public async Task<ActionResult<ApiResponse<object>>> Publish(Guid lessonId, CancellationToken ct)
    {
        await _lessonService.PublishLessonAsync(lessonId, ct);
        return Ok(ApiResponse<object>.Ok("Lesson published."));
    }

    [HttpDelete("{lessonId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid lessonId, CancellationToken ct)
    {
        await _lessonService.DeleteLessonAsync(lessonId, ct);
        return Ok(ApiResponse<object>.Ok("Lesson deleted."));
    }
}
