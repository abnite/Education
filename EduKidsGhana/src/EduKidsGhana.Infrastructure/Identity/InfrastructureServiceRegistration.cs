using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Entities.Identity;
using EduKidsGhana.Infrastructure.Persistence;
using EduKidsGhana.Infrastructure.Persistence.Seeders;
using EduKidsGhana.Infrastructure.Services.AI;
using EduKidsGhana.Infrastructure.Services.Auth;
using EduKidsGhana.Infrastructure.Services.Curriculum;
using EduKidsGhana.Infrastructure.Services.Gamification;
using EduKidsGhana.Infrastructure.Services.Learning;
using EduKidsGhana.Infrastructure.Services.Progress;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduKidsGhana.Infrastructure.Identity;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Core Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILearnerService, LearnerService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<ILearningOrchestrator, LearningOrchestrator>();
        services.AddScoped<IAdaptiveLearningService, AdaptiveLearningService>();
        services.AddScoped<IQuizGenerationService, QuizGenerationService>();
        services.AddScoped<IRuleBasedQuestionGenerator, RuleBasedQuestionGenerator>();
        services.AddScoped<IProgressTrackingService, ProgressTrackingService>();
        services.AddScoped<IRewardService, RewardService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<ILessonRecommendationService, LessonRecommendationService>();

        // AI
        services.AddScoped<DummyAIProvider>();
        services.AddScoped<AIProviderFactory>();

        // Seeder
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
