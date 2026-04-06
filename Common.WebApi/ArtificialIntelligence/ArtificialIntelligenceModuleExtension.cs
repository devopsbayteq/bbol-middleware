using Common.WebApi.ArtificialIntelligence.Gemini;
using Common.WebApi.DelegatingHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Common.WebApi.ArtificialIntelligence;

public static class ArtificialIntelligenceModuleExtension
{
    public static void AddArtificialIntelligence(this IServiceCollection services)
    {
        services.AddTransient<LoggerHandler>();
        services.AddScoped<IArtificialIntelligence, GeminiArtificialIntelligence>();
        services.AddHttpClient($"{ArtificialIntelligenceImplementationType.Gemini}").AddHttpMessageHandler<LoggerHandler>();
    }
}