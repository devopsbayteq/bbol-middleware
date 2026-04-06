namespace ApiCore.Config;
public static class MediatrExtension
{
    /// <summary>
    /// Extensión para Api
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="IUserMessages"></typeparam>
    /// <typeparam name="UserMessages"></typeparam>
    public static void AddMediatrTypes(this IServiceCollection services, params Type[] types)
    {
        services.AddMediatR(cfg =>
        {
            foreach (var type in types)
                cfg.RegisterServicesFromAssembly(type.Assembly);
        });
    }
}