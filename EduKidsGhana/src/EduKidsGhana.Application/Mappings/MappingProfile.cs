using AutoMapper;
using EduKidsGhana.Application.DTOs.Curriculum;
using EduKidsGhana.Application.DTOs.Learner;
using EduKidsGhana.Application.DTOs.Progress;
using EduKidsGhana.Application.DTOs.Gamification;
using EduKidsGhana.Application.DTOs.Admin;
using EduKidsGhana.Domain.Entities.Curriculum;
using EduKidsGhana.Domain.Entities.Users;
using EduKidsGhana.Domain.Entities.Learning;
using EduKidsGhana.Domain.Entities.Gamification;
using EduKidsGhana.Domain.Entities.Identity;

namespace EduKidsGhana.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Curriculum
        CreateMap<Subject, SubjectDto>();
        CreateMap<CreateSubjectDto, Subject>();
        CreateMap<GradeLevel, GradeLevelDto>();
        CreateMap<Topic, TopicDto>();
        CreateMap<CreateTopicDto, Topic>();
        CreateMap<Lesson, LessonSummaryDto>();
        CreateMap<Lesson, LessonDetailDto>()
            .ForMember(d => d.TopicName, o => o.MapFrom(s => s.Topic.Name))
            .ForMember(d => d.SubjectName, o => o.MapFrom(s => s.Topic.Subject.Name));
        CreateMap<LessonSection, LessonSectionDto>();
        CreateMap<LessonObjective, LessonObjectiveDto>();
        CreateMap<LessonExample, LessonExampleDto>();
        CreateMap<LessonAudio, DTOs.Curriculum.LessonAudioDto>();
        CreateMap<CreateLessonDto, Lesson>();
        CreateMap<CreateLessonSectionDto, LessonSection>();
        CreateMap<CreateLessonExampleDto, LessonExample>();

        // Users
        CreateMap<LearnerProfile, LearnerProfileDto>();
        CreateMap<CreateLearnerDto, LearnerProfile>();

        // Progress
        CreateMap<TopicMastery, TopicMasteryDto>()
            .ForMember(d => d.TopicName, o => o.MapFrom(s => s.Topic.Name))
            .ForMember(d => d.SubjectName, o => o.MapFrom(s => s.Topic.Subject.Name))
            .ForMember(d => d.SubjectColour, o => o.MapFrom(s => s.Topic.Subject.ColourHex));

        // Gamification
        CreateMap<Achievement, AchievementDto>()
            .ForMember(d => d.IsEarned, o => o.Ignore())
            .ForMember(d => d.EarnedAt, o => o.Ignore());

        // Identity
        CreateMap<ApplicationUser, UserSummaryDto>()
            .ForMember(d => d.Roles, o => o.Ignore());

        // Avatar
        CreateMap<LearnerAvatar, AvatarDto>();
    }
}
