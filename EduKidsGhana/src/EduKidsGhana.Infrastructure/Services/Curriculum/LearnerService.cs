using AutoMapper;
using EduKidsGhana.Application.DTOs.Learner;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Constants;
using EduKidsGhana.Domain.Entities.Identity;
using EduKidsGhana.Domain.Entities.Users;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.Curriculum;

public class LearnerService : ILearnerService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<LearnerService> _logger;

    public LearnerService(AppDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<LearnerService> logger)
    {
        _db = db;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LearnerProfileDto?> GetProfileAsync(Guid learnerId, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        return learner == null ? null : _mapper.Map<LearnerProfileDto>(learner);
    }

    public async Task<LearnerProfileDto> CreateLearnerAsync(CreateLearnerDto dto, Guid parentUserId, CancellationToken ct = default)
    {
        var parent = await _db.ParentProfiles.FirstOrDefaultAsync(p => p.UserId == parentUserId, ct)
            ?? throw new InvalidOperationException("Parent profile not found.");

        // Create identity user for learner
        var parentUser = await _userManager.FindByIdAsync(parentUserId.ToString())
            ?? throw new InvalidOperationException("Parent user not found.");

        var learnerEmail = $"learner_{Guid.NewGuid():N}@edukidsghana.internal";
        var learnerUser = new ApplicationUser
        {
            FirstName = dto.DisplayName,
            LastName = string.Empty,
            Email = learnerEmail,
            UserName = learnerEmail,
            IsActive = true
        };
        await _userManager.CreateAsync(learnerUser, $"Learner{Guid.NewGuid():N}!");
        await _userManager.AddToRoleAsync(learnerUser, DomainConstants.Roles.Learner);

        var learner = new LearnerProfile
        {
            UserId = learnerUser.Id,
            DisplayName = dto.DisplayName,
            Age = dto.Age,
            GradeLevel = dto.GradeLevel,
            AvatarCode = dto.AvatarCode ?? "avatar_1",
            AudioEnabled = dto.AudioEnabled
        };
        _db.LearnerProfiles.Add(learner);

        _db.LearnerPreferences.Add(new LearnerPreference { LearnerId = learner.Id, AudioNarrationEnabled = dto.AudioEnabled });
        _db.ParentLearnerLinks.Add(new ParentLearnerLink { ParentProfileId = parent.Id, LearnerProfileId = learner.Id });

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<LearnerProfileDto>(learner);
    }

    public async Task<LearnerProfileDto> UpdateProfileAsync(Guid learnerId, UpdateLearnerDto dto, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct)
            ?? throw new InvalidOperationException("Learner not found.");
        learner.DisplayName = dto.DisplayName;
        learner.Age = dto.Age;
        learner.GradeLevel = dto.GradeLevel;
        learner.AvatarCode = dto.AvatarCode;
        learner.AudioEnabled = dto.AudioEnabled;
        learner.PreferredLanguage = dto.PreferredLanguage;
        learner.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<LearnerProfileDto>(learner);
    }

    public async Task<LearnerDashboardDto> GetDashboardAsync(Guid learnerId, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct)
            ?? throw new InvalidOperationException("Learner not found.");

        var subjects = await _db.Subjects.Include(s => s.Topics).Where(s => s.IsActive).ToListAsync(ct);
        var topicIds = subjects.SelectMany(s => s.Topics.Select(t => t.Id)).ToList();
        var masteries = await _db.TopicMasteries.Where(m => m.LearnerId == learnerId).ToListAsync(ct);
        var todaySessions = await _db.LearningSessions
            .Where(s => s.LearnerId == learnerId && s.StartedAt.Date == DateTime.UtcNow.Date)
            .SumAsync(s => s.DurationMinutes, ct);

        var subjectProgress = subjects.Select(s =>
        {
            var topicCount = s.Topics.Count;
            var completedTopics = s.Topics.Count(t => masteries.Any(m => m.TopicId == t.Id && m.MasteryLevel >= MasteryLevel.Proficient));
            return new SubjectProgressDto
            {
                SubjectId = s.Id,
                SubjectName = s.Name,
                ColourHex = s.ColourHex,
                IconUrl = s.IconUrl,
                TotalTopics = topicCount,
                CompletedTopics = completedTopics,
                ProgressPercent = topicCount > 0 ? (double)completedTopics / topicCount * 100 : 0
            };
        }).ToList();

        return new LearnerDashboardDto
        {
            Profile = _mapper.Map<LearnerProfileDto>(learner),
            TodayMinutes = todaySessions,
            DailyGoalMinutes = 30,
            CurrentStreak = learner.CurrentStreak,
            SubjectProgress = subjectProgress,
            RecentActivity = new List<RecentActivityDto>(),
            NewBadges = new List<NewBadgeDto>()
        };
    }

    public async Task<bool> SetAvatarAsync(Guid learnerId, string avatarCode, CancellationToken ct = default)
    {
        var learner = await _db.LearnerProfiles.FindAsync(new object[] { learnerId }, ct);
        if (learner == null) return false;
        learner.AvatarCode = avatarCode;
        learner.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<AvatarDto>> GetAvailableAvatarsAsync(CancellationToken ct = default)
    {
        var avatars = await _db.LearnerAvatars.Where(a => a.IsActive).ToListAsync(ct);
        return _mapper.Map<List<AvatarDto>>(avatars);
    }
}
