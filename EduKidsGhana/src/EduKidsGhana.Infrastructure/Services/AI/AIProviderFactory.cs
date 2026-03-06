using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Enums;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EduKidsGhana.Infrastructure.Services.AI;

public class AIProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AppDbContext _db;

    public AIProviderFactory(IServiceProvider serviceProvider, AppDbContext db)
    {
        _serviceProvider = serviceProvider;
        _db = db;
    }

    public async Task<IAIProvider> GetProviderAsync(CancellationToken ct = default)
    {
        var setting = await _db.AIProviderSettings
            .Where(s => s.IsEnabled && s.IsDefault)
            .FirstOrDefaultAsync(ct);

        if (setting == null || !setting.IsEnabled)
            return _serviceProvider.GetRequiredService<DummyAIProvider>();

        return setting.ProviderType switch
        {
            AIProviderType.OpenAI => new OpenAIProvider(
                new HttpClient(),
                setting,
                _serviceProvider.GetRequiredService<ILogger<OpenAIProvider>>()),
            _ => _serviceProvider.GetRequiredService<DummyAIProvider>()
        };
    }
}
