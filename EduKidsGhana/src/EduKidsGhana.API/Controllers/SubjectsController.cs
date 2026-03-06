using EduKidsGhana.Application.DTOs.Curriculum;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;
    private readonly ITopicService _topicService;

    public SubjectsController(ISubjectService subjectService, ITopicService topicService)
    {
        _subjectService = subjectService;
        _topicService = topicService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SubjectDto>>>> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<List<SubjectDto>>.Ok(await _subjectService.GetAllSubjectsAsync(ct)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> GetById(Guid id, CancellationToken ct)
    {
        var subject = await _subjectService.GetByIdAsync(id, ct);
        if (subject == null) return NotFound(ApiResponse<SubjectDto>.Fail("Subject not found."));
        return Ok(ApiResponse<SubjectDto>.Ok(subject));
    }

    [HttpGet("{subjectId:guid}/topics")]
    public async Task<ActionResult<ApiResponse<List<TopicDto>>>> GetTopics(Guid subjectId, [FromQuery] int gradeLevel, CancellationToken ct) =>
        Ok(ApiResponse<List<TopicDto>>.Ok(await _topicService.GetTopicsBySubjectAndGradeAsync(subjectId, gradeLevel, ct)));

    [HttpPost]
    [Authorize(Roles = "Admin,ContentManager")]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> Create([FromBody] CreateSubjectDto dto, CancellationToken ct)
    {
        var result = await _subjectService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<SubjectDto>.Ok(result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,ContentManager")]
    public async Task<ActionResult<ApiResponse<SubjectDto>>> Update(Guid id, [FromBody] CreateSubjectDto dto, CancellationToken ct) =>
        Ok(ApiResponse<SubjectDto>.Ok(await _subjectService.UpdateAsync(id, dto, ct)));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _subjectService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok("Subject deleted."));
    }
}
