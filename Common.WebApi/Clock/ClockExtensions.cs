using Microsoft.Extensions.DependencyInjection;

namespace Common.WebApi.Clock;
public static class ClockExtensions
{
    public static void AddClock(this IServiceCollection services)
        => _ = services.AddScoped<IClock, NodaTimeClock>();
}